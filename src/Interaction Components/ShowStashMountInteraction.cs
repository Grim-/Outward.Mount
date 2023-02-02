namespace EmoMount
{
    public class ShowStashMountInteraction : InteractionBase
    {
        public override string DefaultHoldText => $"Open Stash";

        public override void Activate(Character _character)
        {
            _character.CharacterUI.StashPanel.SetStash(_character.Stash);
            _character.CharacterUI.StashPanel.Show(true);
        }
    }
}
