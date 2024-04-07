using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Create new questline")]
public class QuestBase : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] string description;

    [SerializeField] Dialogue startQuestDialogue;
    [SerializeField] Dialogue inProgresstDialogue;
    [SerializeField] Dialogue completedDialogue;

    [SerializeField] ItemBase requiredItem;
    [SerializeField] ItemBase rewardItem;

    public string Name => name;
    public string Description => description;
    public Dialogue StartDialogue => startQuestDialogue;
    public Dialogue InProgressDialogue => inProgresstDialogue?.Lines?.Count > 0 ? inProgresstDialogue : startQuestDialogue;
    public Dialogue CompletedDialogue => completedDialogue;

    public ItemBase RequiredItem => requiredItem;
    public ItemBase RewardItem => rewardItem;

}
