using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainerController : MonoBehaviour, Interactive
{
    [SerializeField] string name;
    [SerializeField] Dialogue dialogue;
    [SerializeField] Dialogue beatenDialogue;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] Sprite battleSprite;
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

    public void Interact(Transform initiator)
    {

        // Look towards player
        character.LookTowards(initiator.position);

        // Play dialogue
        if (!hasBeenBeaten)
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue, () =>
            {
                GameController.i.StartTrainerBattle(this);
            }));
        }
        else
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogue(beatenDialogue));
        }
        
       
    }

    public void Beat()
    {
        fov.gameObject.SetActive(false);
        hasBeenBeaten = true;
    }


    public IEnumerator TriggerBattle(PlayerController player)
    {
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        var diff = (player.transform.position - transform.position);
        var moveVec = diff.normalized;
        diff -= moveVec;
        Debug.Log($"Trying originally to reach {diff}");
        //moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));
        diff = new Vector2(Mathf.Round(diff.x), Mathf.Round(diff.y));
        Debug.Log($"Trying to reach {diff}");
        yield return character.Move(diff);

        player.GetComponent<Character>().LookTowards(transform.position);
        StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue, () =>
        {
            Debug.Log("Trainer battle started!");
            GameController.i.StartTrainerBattle(this);
        }));
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

    

    public string Name
    {
        get { return name; }
    }

    public Sprite BattleSprite
    {
        get { return battleSprite; }
    }
}
