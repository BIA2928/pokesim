using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    Character character;

    public void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
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

        StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue, () =>
        {
            Debug.Log("Trainer battle started!");
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
}
