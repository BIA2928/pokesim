using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactive
{
    [SerializeField] Dialogue dialogue;

    [Header("Quests")]
    [SerializeField] QuestBase questToGive;
    [SerializeField] QuestBase questToComplete;

    [Header("Movement")]
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPatterns;

    Quest activeQuest;

    float idleTimer = 0f;
    NPCState state;

    int currMovementIndex = 0;

    Character character;
    ItemGiver itemGiver;
    PokemonGiver pokemonGiver;
    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        pokemonGiver = GetComponent<PokemonGiver>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        //Debug.Log("Interacted with NPC");
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialogue;
            character.LookTowards(initiator.position);

            if (questToComplete != null)
            {
                var newQuest = new Quest(questToComplete);
                yield return newQuest.CompleteQuest();
                questToComplete = null;
                Debug.Log("Quest completed");
            }

            if (itemGiver != null && itemGiver.CanGive())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if (pokemonGiver != null && pokemonGiver.CanGive())
            {
                yield return pokemonGiver.GivePokemon(initiator.GetComponent<PlayerController>());
            }
            else if (questToGive != null)
            {
                activeQuest = new Quest(questToGive);
                yield return activeQuest.StartQuest();
                
                questToGive = null;

                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest();
                    activeQuest = null;
                }
            }
            else if (activeQuest != null)
            {
                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest();
                    activeQuest = null;
                }
                else
                {
                    yield return DialogueManager.Instance.ShowDialogue(activeQuest.Base.InProgressDialogue);
                }
            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogue(dialogue);
            }
            
            idleTimer = 0f;
            state = NPCState.Idle;
        }
            
    }

    private void Update()
    {
        if (state == NPCState.Idle)
        {
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
