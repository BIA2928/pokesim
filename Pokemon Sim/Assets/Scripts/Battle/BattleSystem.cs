using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils.StateMachine;

public enum BattleAction
{
    Move, SwitchPoke, UseItem, Run
}


public enum BattleEnvironment { LongGrass, Water, Cave}

public class BattleSystem : MonoBehaviour
{ 
    [Header("Battle Imagery")]
    [SerializeField] Image battleBackground;
    [SerializeField] Sprite grassEnvironment;
    [SerializeField] Sprite waterEnvironment;
    [SerializeField] Sprite caveEnvironment;
    [SerializeField] Sprite stadiumEnvironment;

    [Header("Battle Components")]
    [SerializeField] BattleUnit playerPoke;
    [SerializeField] BattleUnit enemyPoke;
    [SerializeField] BattleDialogue dialogueBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveForgetScreen moveForgetScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image enemyImage;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] InventoryUI inventoryUI;

    [Header("Music")]
    [SerializeField] AudioClip wildBattleMusic;
    [SerializeField] AudioClip wildVictoryMusic;

    // Private fields
    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController enemyTrainer;
    BattleEnvironment battleEnvironment;
    public event Action<bool> OnBattleOver;

    // Properties
    public StateMachine<BattleSystem> StateMachine { get; private set; }
    public PokemonParty PlayerParty { get; private set; } 
    public PokemonParty EnemyParty { get; private set; }
    public Pokemon WildPokemon { get; private set; }
    public int SelectedMove { get; set; }
    public BattleAction SelectedAction { get; set; }
    public Pokemon SelectedSwitchPokemon { get; set; }
    public ItemBase SelectedItem { get; set; }

    public bool IsBattleOver { get; private set; }

    public int NumEscapesTried { get; set; }
    public BattleDialogue DialogueBox => dialogueBox;
    public PartyScreen PartyScreen => partyScreen;
    public Pokemon CurrentAllyPokemon => playerPoke.Pokemon;

    public BattleUnit AllyUnit => playerPoke;
    public BattleUnit EnemyUnit => enemyPoke;
    public bool IsTrainerBattle => isTrainerBattle;

    public TrainerController EnemyTrainer => enemyTrainer;
    public AudioClip WildBattleMusic => wildBattleMusic;
    public AudioClip WildVictoryMusic => wildVictoryMusic;
    public void StartBattle(PokemonParty party, Pokemon wildPokemon, 
        BattleEnvironment battleEnvironment = BattleEnvironment.LongGrass)
    {
        isTrainerBattle = false;
        PlayerParty = party;
        player = PlayerParty.GetComponent<PlayerController>();
        this.WildPokemon = wildPokemon;

        this.battleEnvironment = battleEnvironment;

        dialogueBox.ClearDialogue();
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty party, PokemonParty enemyParty, 
        BattleEnvironment battleEnvironment = BattleEnvironment.LongGrass)
    {
        PlayerParty = party;
        this.EnemyParty = enemyParty;

        isTrainerBattle = true;
        player = PlayerParty.GetComponent<PlayerController>();
        enemyTrainer = enemyParty.GetComponent<TrainerController>();

        this.battleEnvironment = battleEnvironment;
        dialogueBox.ClearDialogue();
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        
        IsBattleOver = false;
        StateMachine = new StateMachine<BattleSystem>(this);
        
        partyScreen.Init();
        NumEscapesTried = 0;
        playerPoke.Clear();
        enemyPoke.Clear();
        SetBackground();
        if (!isTrainerBattle)
        {
            playerImage.gameObject.SetActive(true);
            playerPoke.EnableImage(false);
            playerImage.sprite = player.BattleSprite;
            enemyPoke.Setup(WildPokemon);
            yield return Fader.instance.BattleFader.FadeToColour(0.8f);
            yield return enemyPoke.PlayEnterAnimation2(true);
            yield return dialogueBox.TypeDialogue($"A wild {enemyPoke.Pokemon.Base.Name} appeared!");
            
            


            playerImage.gameObject.SetActive(false);
            playerPoke.Setup(PlayerParty.GetFirstHealthy());
            yield return dialogueBox.TypeDialogue($"Go {playerPoke.Pokemon.Base.Name}!");
            yield return playerPoke.PlayEnterAnimation2();
            dialogueBox.SetMoveNames(playerPoke.Pokemon.Moves);
        }
        else
        {
            // Trainer battle

            // Hide information images
            playerPoke.EnableImage(false);
            enemyPoke.EnableImage(false);


            // Show battle sprites
            playerImage.gameObject.SetActive(true);
            enemyImage.gameObject.SetActive(true);

            playerImage.sprite = player.BattleSprite;
            enemyImage.sprite = enemyTrainer.BattleSprite;
            yield return Fader.instance.BattleFader.FadeToColour(0.8f);
            yield return new WaitForSeconds(0.3f);
            yield return dialogueBox.TypeDialogue($"You are challenged by {enemyTrainer.Name}!");

            // Send out enemy first Poke
            enemyImage.gameObject.SetActive(false);
            var enemyPokemon = EnemyParty.GetFirstHealthy();
                      
            enemyPoke.Setup(enemyPokemon);
            yield return dialogueBox.TypeDialogue($"{enemyTrainer.Name} sent out {enemyPokemon.Base.Name}.");
            yield return enemyPoke.PlayEnterAnimation2();


            // Send out player first Poke
            playerImage.gameObject.SetActive(false);
            var playerPokemon = PlayerParty.GetFirstHealthy();
              
            playerPoke.Setup(playerPokemon);
            yield return dialogueBox.TypeDialogue($"Go {playerPokemon.Base.Name}!");
            yield return playerPoke.PlayEnterAnimation2();

            dialogueBox.SetMoveNames(playerPokemon.Moves);

        }

        dialogueBox.HideActions(false);
        StateMachine.ChangeState(ActionSelectionState.i);
    }

    public void SetBackground()
    {
        battleBackground.sprite = battleEnvironment switch
        {
            BattleEnvironment.LongGrass => grassEnvironment,
            BattleEnvironment.Water => waterEnvironment,
            BattleEnvironment.Cave => caveEnvironment,
            _ => grassEnvironment,
        };
    }

    public void HandleUpdate()
    {
        if (StateMachine == null)
            return;
        StateMachine.Execute();
    }

    public void ResetTrainer()
    {
        enemyTrainer = null;
        enemyImage.gameObject.SetActive(false);
    }


    public IEnumerator BattleOver(bool playerWin)
    {
        IsBattleOver = true;

        PlayerParty.PokemonList.ForEach(p => p.OnBattleOver());
        playerPoke.BattleHUD.ClearData();
        enemyPoke.BattleHUD.ClearData();
        
        if (playerWin)
        {
            if (IsTrainerBattle)
            {
                AudioManager.i.PlayMusic(enemyTrainer.VictoryMusic, fade:false);
            }
            else
            {
                AudioManager.i.PlayMusic(wildVictoryMusic, fade: false);
            }
                
            yield return new WaitForSeconds(0.5f);
            yield return PlayTrainerDefeatAnimation();
            enemyPoke.ResetAfterFaint();
            OnBattleOver(playerWin);
        }
        else
        {
            playerPoke.ResetAfterFaint();
            OnBattleOver(!playerWin);
        }
    }

    IEnumerator PlayTrainerDefeatAnimation()
    {
        enemyImage.gameObject.SetActive(true);
        yield return DialogueBox.ShowDialogue($"You defeated {enemyTrainer.Name}!", hideActions: true);
        yield return new WaitForSeconds(0.3f);
        yield return DialogueBox.ShowDialogue(enemyTrainer.DefeatDialogue, true, true);
        yield return new WaitForSeconds(0.3f);
        Wallet.i.AddMoney(enemyTrainer.Payout);
        yield return DialogueBox.ShowDialogue($"You got ${enemyTrainer.Payout} for winning!", hideActions: true);
        yield return new WaitForSeconds(0.2f);
        
    }

    public IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        playerPoke.Pokemon.CureVolatileCondition();
        bool pokemonFainted = !(playerPoke.Pokemon.HP > 0);
        dialogueBox.HideActions(true);
        if (!pokemonFainted)
        {
            yield return dialogueBox.TypeDialogue(
                $"That's enough for now {playerPoke.Pokemon.Base.Name}, come back!");
            yield return playerPoke.PlayFaintAnimation();
            yield return new WaitForSeconds(0.7f);
        }


        playerPoke.Setup(newPokemon);
        yield return dialogueBox.TypeDialogue($"Go {newPokemon.Base.Name}!");
        yield return playerPoke.PlayEnterAnimation2();
        dialogueBox.SetMoveNames(newPokemon.Moves);
        
    }

    public IEnumerator SendNextTrainerPokemon()
    {

        var nextPoke = EnemyParty.GetFirstHealthy();

        enemyPoke.Setup(nextPoke);
        yield return dialogueBox.TypeDialogue($"{enemyTrainer.Name} sent out {nextPoke.Base.Name}.");
        yield return enemyPoke.PlayEnterAnimation2();

    }

    public IEnumerator ThrowPokeball(PokeballItem pokeballItem)
    {
        if (isTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue($"Cannot capture pokemon in a trainer battle!");
            yield break;
        }


        dialogueBox.HideActions(true);
        yield return dialogueBox.TypeDialogue($"{player.Name} used a {pokeballItem.Name}!");

        var pokeballObj = Instantiate(pokeballSprite, playerPoke.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = pokeballItem.BagIcon;
        // Animations

        // Throw and spin
        Vector3 buffer = new Vector3(0, 1f);
        var sequence = DOTween.Sequence();
        AudioManager.i.PlaySFX(AudioID.PokeballThrow);
        sequence.Append(pokeball.transform.DOJump(enemyPoke.transform.position + buffer, 2f, 1, 1.2f));
        sequence.Join(pokeball.transform.DORotate(new Vector3(0, 0, -1080f), 1.2f, RotateMode.FastBeyond360));
        yield return sequence.WaitForCompletion();
        AudioManager.i.PlaySFX(AudioID.PokemonOut);
        // Shrink into ball
        yield return enemyPoke.PlayCaptureAnimation();

        // Drop to floor
        yield return pokeball.transform.DOMoveY(enemyPoke.transform.position.y - 1.25f, 0.4f).WaitForCompletion();
        AudioManager.i.PlaySFX(AudioID.PokeballBounce);
        
        //Shake
        int shakeCount = TryCatchPokemon(enemyPoke.Pokemon, pokeballItem);
        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(.5f);
            AudioManager.i.PlaySFX(AudioID.PokeballShake);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 8f), 0.8f).WaitForCompletion();
            
        }

        if (shakeCount == 4)
        {
            AudioManager.i.PlaySFX(AudioID.PokeballClick);
            AudioManager.i.PlayMusic(WildVictoryMusic);
            yield return pokeball.DOColor(Color.gray, 0.1f).WaitForCompletion();
            yield return dialogueBox.TypeDialogue($"{enemyPoke.Pokemon.Base.Name} was successfully caught!");
            yield return pokeball.DOFade(0, 1f).WaitForCompletion();

            var inParty = PlayerParty.AddPokemon(enemyPoke.Pokemon);
            if (inParty)
                yield return dialogueBox.TypeDialogue($"{enemyPoke.Pokemon.Base.Name} was added to the party.");
            else
                yield return dialogueBox.TypeDialogue($"{enemyPoke.Pokemon.Base.Name} was sent to the PC.");

            Destroy(pokeball);
            yield return BattleOver(true);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            AudioManager.i.PlaySFX(AudioID.PokemonOut);
            pokeball.DOFade(0, 0.3f);
            yield return enemyPoke.PlayBreakOutAnimation();
            if (shakeCount == 3)
            {
                yield return dialogueBox.TypeDialogue($"The wild {enemyPoke.Pokemon.Base.Name} broke free.");
                yield return dialogueBox.TypeDialogue($"It was so close too!");
            }
            else
            {
                yield return dialogueBox.TypeDialogue($"The wild {enemyPoke.Pokemon.Base.Name} broke out!");
            }

            Destroy(pokeball);

        }

        //dialogueBox.HideActions(false);
    }

    /// <summary>
    /// Returns shake count. ShakeCount = 4 means caught
    /// </summary>
    /// <param name="pokemon">Pokemon attempting to be caught</param>
    /// <returns></returns>
    int TryCatchPokemon(Pokemon pokemon, PokeballItem pokeballItem)
    {

        float statusBonus = ConditionsDB.GetStatusBonus(pokemon.Cnd);

        float a = (3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * pokeballItem.CatchRateModifier * statusBonus / (3 * pokemon.MaxHP);

        if (a >= 255)
            return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
            {
                break;
            }

            shakeCount++;
        }

        return shakeCount;

    }

}
