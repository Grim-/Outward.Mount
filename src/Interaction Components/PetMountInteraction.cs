using UnityEngine;

namespace EmoMount
{
    public class PetMountInteraction : InteractionBase
    {
        private BasicMountController MountController => GetComponentInParent<BasicMountController>();
        public override string DefaultHoldText => MountController != null ? $"Pet {MountController.MountName}" : "Pet Mount";

        public override void Activate(Character _character)
        {
            CharacterMount characterMount = _character.GetComponent<CharacterMount>();

            int rand = Random.Range(0, 10);

            if (rand > 8)
            {
                MountController.PlayMountAnimation(MountAnimations.MOUNT_SPECIAL);
            }
            else
            {
                MountController.PlayMountAnimation(MountAnimations.MOUNT_HAPPY);
            }
           
        }
    }
}
