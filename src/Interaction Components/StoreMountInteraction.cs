namespace EmoMount
{
    public class StoreMountInteraction : InteractionBase
    {
        private BasicMountController MountController => GetComponentInParent<BasicMountController>();
        public override string DefaultHoldText => MountController != null ? $"Dismiss {MountController.MountName}" : "Dismiss Mount";
        public override string DefaultHoldLocKey => base.DefaultHoldLocKey;
        public override void Activate(Character _character)
        {
            CharacterMount characterMount = _character.GetComponent<CharacterMount>();

            if (characterMount != null && _character.IsLocalPlayer && _character == MountController.CharacterOwner)
            {
                characterMount.StoreMount(MountController);
            }

        }
    }
}
