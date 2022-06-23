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
        public override string DefaultHoldText => MountController != null ? $"Feed {MountController.MountName}" : "Feed Mount";

        public override void Activate(Character _character)
        {
            if (_character.IsLocalPlayer && _character == MountController.CharacterOwner)
            {
                List<Item> foundFoodFavourites = new List<Item>();

                foreach (var favFood in MountController.MountFood.FavouriteFoods)
                {
                    foundFoodFavourites.AddRange(_character.Inventory.GetOwnedItems(favFood.Key));
                }

                //if (foundFoodFavourites.Count > 0)
                //{
                //    _character.Inventory.RemoveItem(foundFoodFavourites[0].ItemID, 1);
                //    MountController.PlayMountAnimation(MountAnimations.MOUNT_HAPPY);
                //    MountController.MountFood.Feed(foundFoodFavourites[0], MountController.MountFood.FavouriteFoods[foundFoodFavourites[0].ItemID]);
                //}
            }
        }
    }
}
