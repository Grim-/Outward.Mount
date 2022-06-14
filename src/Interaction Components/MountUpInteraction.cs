using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoMount
{
    public class MountUpInteraction : InteractionBase
    {
        private BasicMountController MountController => GetComponentInParent<BasicMountController>();
        public override bool CanBeActivated => true;
        public override string DefaultPressText => $"Mount {MountController.MountName}";
        public override void Activate(Character _character)
        {
            if (MountController != null)
            {
                if (MountController.MountFood.FoodAsNormalizedPercent > 0.1f)
                {
                    MountController.MountCharacter(_character);
                }
                else
                {
                    _character.CharacterUI.NotificationPanel.ShowNotification($"{MountController.MountName} refuses, it is too hungry.");
                }

            }
        }

    }
}
