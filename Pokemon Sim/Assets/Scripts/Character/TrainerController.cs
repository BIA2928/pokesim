using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainerController : MonoBehaviour, Interactive, ISavable
{
    [SerializeField] new string name;
    [SerializeField] Dialogue dialogue;
    [SerializeField] Dialogue defeatDialogue;
    [SerializeField] Dialogue beatenDialogue;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] Sprite battleSprite;
    
    [SerializeField] AudioClip battleMusic;
    [SerializeField] AudioClip victoryMusic;
    [SerializeField] AudioClip approachMusic;
    Character character;

    bool hasBeenBeaten = false;

    public void Awake()
    {
        character = GetComponent<Character>();
    }
    
    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
    }

    private void Update()
    {
        character.HandleUpdate();
    }

    public IEnumerator Interact(Transform initiator)
    {

        // Look towards player
        character.LookTowards(initiator.position);

        // Play dialogue
        if (!hasBeenBeaten)
        {
            AudioManager.i.PlayMusic(approachMusic, fade: false);
            yield return DialogueManager.Instance.ShowDialogue(dialogue);
            yield return GameController.i.StartTrainerBattle(this);
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogue(beatenDialogue);
        }
        
       
    }

    public void Beat()
    {
        fov.gameObject.SetActive(false);
        hasBeenBeaten = true;
    }


    public IEnumerator TriggerBattle(PlayerController player)
    {
        var playerCharacter = player.GetComponent<Character>();
        GameController.i.StateMachine.Push(CutsceneState.i);
        playerCharacter.Stop();
        AudioManager.i.PlayMusic(approachMusic, fade: false);
        exclamation.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        exclamation.SetActive(false);
        

        var diff = (player.transform.position - transform.position);
        var moveVec = diff.normalized;
        diff -= moveVec;
        diff = new Vector2(Mathf.Round(diff.x), Mathf.Round(diff.y));
        yield return character.Move(diff);

        
        
        playerCharacter.LookTowards(transform.position);
        yield return DialogueManager.Instance.ShowDialogue(dialogue);

        //GameController.i.StateMachine.Pop();
        //Handled in StartTrainerBattle
        yield return GameController.i.StartTrainerBattle(this);
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Left)
            angle = 270f;
        else if (dir == FacingDirection.Up)
            angle = 180f;

        fov.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public object CaptureState()
    {
        return hasBeenBeaten;
    }

    public void RestoreState(object state)
    {
        hasBeenBeaten = (bool)state;
        if (hasBeenBeaten)
            fov.gameObject.SetActive(false);
    }

    public string Name
    {
        get { return name; }
    }

    public Sprite BattleSprite
    {
        get { return battleSprite; }
    }

    public AudioClip ApproachMusic => approachMusic;
    public AudioClip BattleMusic => battleMusic;
    public AudioClip VictoryMusic => victoryMusic;
}
