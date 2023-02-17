using NodeCanvas.Framework;

public class HasQuestEventTriggered : ConditionTask
{
    public string EventUID;

    public override bool OnCheck()
    {
        if (QuestEventManager.Instance.HasQuestEvent(EventUID))
        {
            return true;
        }

        return false;
    }
}