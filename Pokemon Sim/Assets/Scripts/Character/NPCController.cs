using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactive
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPatterns;

    float idleTimer = 0f;
    NPCState state;

    int currMovementIndex = 0;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact(Transform initiator)
    {
        //Debug.Log("Interacted with NPC");
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialogue;
            character.LookTowards(initiator.position);
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue, () =>
            {
                idleTimer = 0f;
                state = NPCState.Idle;
            }));
        }
            
    }

    private void Update()
    {
        if (state == NPCState.Idle)
        {
            Debug.Log("Incrementing time");
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPatterns)
            {
                if (movementPattern.Count > 0)
                {
                    StartCoroutine(Walk()) ;
                }
                
                idleTimer = 0f;
            }
        }
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var prevPos = transform.position;
        //Debug.Log($"Executing move {currMovementIndex} of {movementPattern.Count}");
        yield return character.Move(movementPattern[currMovementIndex]);

        if (transform.position != prevPos)
            // only increment pattern if the character actually walks
            currMovementIndex = (currMovementIndex + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }
}

public enum NPCState { Idle, Walking, Dialogue }
