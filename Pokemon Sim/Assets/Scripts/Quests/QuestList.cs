
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestList : MonoBehaviour, ISavable
{
    List<Quest> quests = new List<Quest>();
    public event System.Action OnUpdated;

    public void AddQuest(Quest quest)
    {
        if (!quests.Contains(quest))
            quests.Add(quest);
        OnUpdated?.Invoke();
    }

    public static QuestList GetQuestList()
    {
        return FindObjectOfType<PlayerController>().GetComponent<QuestList>();
    }

    public bool IsStarted(string questName)
    {
        var matchingQuestStatus = quests.FirstOrDefault(q => q.Base.Name == questName)?.Status;
        return matchingQuestStatus == QuestStatus.Started || matchingQuestStatus == QuestStatus.Completed;
    }

    public bool IsCompleted(string questName)
    {
        var matchingQuestStatus = quests.FirstOrDefault(q => q.Base.Name == questName)?.Status;
        return matchingQuestStatus == QuestStatus.Completed;
    }

    public object CaptureState()
    {
        return quests.Select(q => q.GetQuestSaveData()).ToList();
    }

    public void RestoreState(object state)
    {
        var saveData = state as List<QuestSaveData>;
        if (saveData != null)
        {
            quests = saveData.Select(q => new Quest(q)).ToList();
        }
        OnUpdated?.Invoke();
    }
}
