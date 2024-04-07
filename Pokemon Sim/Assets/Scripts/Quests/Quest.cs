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

    public Quest(QuestSaveData saveData)
    {
        Base = QuestDB.LookupByName(saveData.name);
        Status = saveData.status;
    }

    public QuestSaveData GetQuestSaveData()
    {
        return new QuestSaveData()
        {
            name = Base.name,
            status = Status,
        };
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


[System.Serializable]
public class QuestSaveData
{
    public string name;
    public QuestStatus status;
}

public enum QuestStatus { None, Started, Completed }