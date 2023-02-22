using EmoMount;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;

public class HasQuestEventTriggered : ConditionTask
{
    public string EventUID;

    public override bool OnCheck()
    {
        if (string.IsNullOrEmpty(EventUID))
        {
            EmoMountMod.Log.LogMessage($"EventUID : {EventUID} is null or empty");
            return false;
        }

        QuestEventData questEventData = QuestEventManager.Instance.GetQuestEvent(EventUID);
        bool Result = QuestEventManager.Instance.GetEventCurrentStack(EventUID) >= 1;
        EmoMountMod.Log.LogMessage($"Checking EventUID : {EventUID} Result : {Result}");
        return Result;
    }
}

public class HasCompleteQuest : ConditionTask
{
    public int QuestID = -1;

    public override bool OnCheck()
    {
        if (QuestID == -1)
        {
            EmoMountMod.Log.LogMessage($"QuestID : {QuestID} is not set.");
            return false;
        }

        Character Character = (Character)ownerAgent.GetComponent<Character>();
        bool Result = Character.Inventory.QuestKnowledge.IsQuestCompleted(QuestID);
        EmoMountMod.Log.LogMessage($"QuestID : {QuestID} Result : {Result}");
        return Result;
    }
}