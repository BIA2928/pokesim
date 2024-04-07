using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest 
{
    public QuestStatus Status { get; private set; }
    public QuestBase Base { get; private set; }

    public Quest(QuestBase questBase)
    {
        Base = questBase;
        Status = QuestStatus.None;
    }

    public IEnumerator StartQuest()
    {
        Status = QuestStatus.Started;
        yield return DialogueManager.Instance.ShowDialogue(Base.StartDialogue);

        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);
    }

    public IEnumerator CompleteQuest()
    {
        var inventory = Inventory.GetInventory();
        if (Base.RequiredItem)
        {
            if (Base.RequiredItem is KeyItem)
            {
                yield return DialogueManager.Instance.ShowItemUsedDialogue(Base.RequiredItem);
            }
            else
            {
                yield return DialogueManager.Instance.ShowItemGivenDialogue(Base.RequiredItem);
                inventory.RemoveOneOfItem(Base.RequiredItem);
            }
        }
        Status = QuestStatus.Completed;
        yield return DialogueManager.Instance.ShowDialogue(Base.CompletedDialogue);
        
        if (Base.RewardItem != null)
        {
            inventory.AddItem(Base.RewardItem);
            yield return DialogueManager.Instance.ShowItemReceivedDialogue(Base.RewardItem);
        }

        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);
    }

    public bool CanBeCompleted()
    {
        if (Base.RewardItem != null)
        {
            var inventory = Inventory.GetInventory();
            return inventory.HasItem(Base.RequiredItem);
        }
        return false;
    }
}

public enum QuestStatus { None, Started, Completed }