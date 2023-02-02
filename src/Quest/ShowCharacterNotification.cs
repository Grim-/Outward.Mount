using NodeCanvas.Framework;

public class ShowCharacterNotification : ActionTask<QuestProgress>
{
    public string NotificationText;

    public override void OnExecute()
    {
        base.OnExecute();

        if (agent.ParentQuest.OwnerCharacter.CharacterUI.NotificationPanel != null)
        {
            agent.ParentQuest.OwnerCharacter.CharacterUI.NotificationPanel.ShowNotification(NotificationText);

            EndAction(true);
        }

        EndAction(false);
    }
}