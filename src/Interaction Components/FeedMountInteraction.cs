using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoMount
{
    public class FeedMountInteraction : InteractionBase
    {
        private BasicMountController MountController => GetComponentInParent<BasicMountController>();
        public override string DefaultHoldText => $"Feed {MountController.MountName}";

        public override void Activate(Character _character)
        {
            //4000010 - gabberries
            //is player
            if (_character.IsLocalPlayer && _character == MountController.CharacterOwner)
            {

                //todo loop inventory use 'best' food first

                List<Item> foundFood = _character.Inventory.GetOwnedItems(4000010);

                if (foundFood.Count > 0)
                {
                    _character.Inventory.RemoveItem(4000010, 1);
                    MountController.PlayTriggerAnimation("DoMountSpecial");
                    //MountController.Owner.CharacterUI.NotificationPanel.ShowNotification("You fed your mount");
                    MountController.MountFood.Feed(4000010, 5f);
                }
            }
        }
    }
}
