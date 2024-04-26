using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactive, ISavable
{
    [SerializeField] Dialogue dialogue;

    [Header("Quests")]
    [SerializeField] QuestBase questToGive;
    [SerializeField] QuestBase questToComplete;

    [Header("Movement")]
    [SerializeReference] [SerializeField] List<Movement> movements;
    [SerializeField] float timeBetweenPatterns;

    Quest activeQuest;

    float idleTimer = 0f;
    NPCState state;

    int currMovementIndex = 0;

    Character character;
    ItemGiver itemGiver;
    PokemonGiver pokemonGiver;
    Healer healer;
    Merchant merchant;
    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        pokemonGiver = GetComponent<PokemonGiver>();
        healer = GetComponent<Healer>();
        merchant = GetComponent<Merchant>();
    }

    public void AddMovement(Movement movement)
    {
        movements.Add(movement);
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
            else if (healer != null)
            {
                yield return healer.Heal(initiator, dialogue);
            }
            else if (merchant != null)
            {
                yield return merchant.Trade();
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
                if (movements.Count > 0)
                {
                    if (movements[currMovementIndex] is MoveVector)
                        StartCoroutine(Walk());
                    else
                    {
                        state = NPCState.Turning;
                        Turn turn = (movements[currMovementIndex++] as Turn);
                        character.Turn(turn.Direction);
                        state = NPCState.Idle;
                    }

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
        Vector2 movement = (movements[currMovementIndex] as MoveVector).Move;
        yield return character.Move(movement);

        if (transform.position != prevPos)
            // only increment pattern if the character actually walks
            currMovementIndex = (currMovementIndex + 1) % movements.Count;

        state = NPCState.Idle;
    }



    public object CaptureState()
    {
        var retVal = new NPCQuestSaveData();
        retVal.activeQuest = activeQuest?.GetQuestSaveData();
        if (questToComplete != null)
        {
            retVal.questToComplete = (new Quest(questToComplete)).GetQuestSaveData();
        }
        if (questToGive != null)
        {
            retVal.questToStart = (new Quest(questToGive)).GetQuestSaveData();
        }
        return retVal;
    }

    public void RestoreState(object state)
    {
        var sD = state as NPCQuestSaveData;
        if (sD != null)
        {
            activeQuest = (sD.activeQuest != null)? (new Quest(sD.activeQuest)) : null;
            questToGive = (sD.questToStart != null) ? (QuestDB.LookupByName(sD.questToStart.name)) : null;
            questToComplete = (sD.questToComplete != null) ? (QuestDB.LookupByName(sD.questToComplete.name)) : null;
        }

    }
}

[System.Serializable]
public class NPCQuestSaveData
{
    public QuestSaveData activeQuest;
    public QuestSaveData questToStart;
    public QuestSaveData questToComplete;
}

[System.Serializable]
public class Movement
{
    
}

[System.Serializable]
public class MoveVector : Movement
{
    [SerializeField] Vector2 move;
    public Vector2 Move => move;
}

[System.Serializable]
public class Turn : Movement
{
    [SerializeField] FacingDirection direction;
    public FacingDirection Direction => direction;
}

public enum NPCState { Idle, Walking, Dialogue, Turning }
