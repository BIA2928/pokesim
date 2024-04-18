using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ledge : MonoBehaviour
{
    [SerializeField] int xDir;
    [SerializeField] int yDir;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
    public bool TryToJump(Character character, Vector2 moveDir)
    {
        Debug.Log("Checkpoint 1");
        if (moveDir.x == xDir || moveDir.y == yDir)
        {
            Debug.Log("Checkpoint2");
            StartCoroutine(Jump(character));
            return true;
        }
        return false;
    }

    IEnumerator Jump(Character character)
    {
        GameController.i.PauseGame(true);
        character.Animator.IsJumping = true;
        Vector3 destination = character.transform.position + new Vector3(xDir, yDir) * 2;
        AudioManager.i.PlaySFX(AudioID.Jump);
        yield return character.transform.DOJump(destination, 0.25f, 1, 0.75f).WaitForCompletion();
        character.Animator.IsJumping = false;
        GameController.i.PauseGame(false);
    }
}
