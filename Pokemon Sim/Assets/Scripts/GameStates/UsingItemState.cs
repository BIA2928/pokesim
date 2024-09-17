using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class UsingItemState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;
    public static UsingItemState i;

    public bool ItemUsed { get; private set; }
    private void Awake()
    {
        i = this;
        inventory = Inventory.GetInventory();
    }

    GameController gC;
    Inventory inventory;
    public override void EnterState(GameController owner)
    {
        gC = owner;
        StartCoroutine(UseItem());
        ItemUsed = false;
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    IEnumerator UseItem()
    {

        var currItem = inventoryUI.SelectedItem;

        if (currItem is TmItem)
        {
            yield return HandleTM();
            gC.StateMachine.Pop();
            yield break;
        }
            
        if (currItem is EvolutionItem)
        {
            var evo = partyScreen.SelectedMember.CheckForEvolution(currItem);
            if (evo != null)
            {
                yield return EvolutionManager.i.Evolve(partyScreen.SelectedMember, evo);
            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogue("It won't have any effect.");
                gC.StateMachine.Pop();
                yield break;
            }
        }
        // Use item on selected poke
        var usedItem = inventory.UseItem(currItem, partyScreen.SelectedMember);
        if (usedItem == null)
        {
            
            if (!(usedItem is KeyItem))
                yield return DialogueManager.Instance.ShowDialogue($"It won't have any effect.");
            else
                yield return DialogueManager.Instance.ShowCantUseDialogue();
        }
        else
        {
            ItemUsed = true;
            if (usedItem is MedicineItem)
                yield return DialogueManager.Instance.ShowDialogue($"One {usedItem.Name} was used.");
        }

        gC.StateMachine.Pop();
    }

    IEnumerator HandleTM()
    {
        var item = inventoryUI.SelectedItem as TmItem;

        if (item == null)
            yield break;

        Pokemon pokemon = partyScreen.SelectedMember;
        if (pokemon.HasMove(item.Move))
        {
            yield return DialogueManager.Instance.ShowDialogue($"{pokemon.Base.Name} already knows {item.Move.Name}.");
            yield break;
        }

        if (!pokemon.Base.CanLearnByTm(item.Move))
        {
            yield return DialogueManager.Instance.ShowDialogue($"{pokemon.Base.Name} cannot learn {item.Move.Name}.");
            yield break;
        }

        if (pokemon.Moves.Count == PokemonBase.MaxNMoves)
        {

            Dialogue dialogue = new Dialogue();
            dialogue.Lines.Add($"{pokemon.Base.Name} wants to learn {item.Move.Name}.");
            dialogue.Lines.Add($"But {pokemon.Base.Name} already knows four moves.");
            dialogue.Lines.Add($"Select a move for {pokemon.Base.Name} to forget.");
            yield return DialogueManager.Instance.ShowDialogue(dialogue);
            ForgettingMoveState.i.MoveToLearn = item.Move;
            ForgettingMoveState.i.Moves = pokemon.Moves;
            yield return gC.StateMachine.PushAndWait(ForgettingMoveState.i);

            int moveIndex = ForgettingMoveState.i.Selection;
            if (moveIndex == -1 || moveIndex >= pokemon.Moves.Count )
            {
                yield return DialogueManager.Instance.ShowDialogue($"{pokemon.Base.Name} did not learn {item.Move.Name}");
                yield break;
            }
            var replacedMove = pokemon.Moves[moveIndex].Base.Name;
            pokemon.ReplaceMove(pokemon.Moves[moveIndex], new Move(item.Move));
            yield return DialogueManager.Instance.ShowDialogue($"{pokemon.Base.Name} forgot {replacedMove} and learned {item.Move.Name}!");
            AudioManager.i.PlaySFX(AudioID.LvlUp);
        }
        else
        {
            pokemon.LearnMove(item.Move);
            AudioManager.i.PlaySFX(AudioID.LvlUp);
            yield return DialogueManager.Instance.ShowDialogue($"{pokemon.Base.Name} learned {item.Move.Name}!");
        }

    }
}
