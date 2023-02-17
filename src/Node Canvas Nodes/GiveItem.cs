using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using UnityEngine;

namespace EmoMount
{
    public partial class EmoMountMod
    {
        public class GiveItem : ActionNode
        {
            public int ItemID;

            public GiveItem()
            {

            }

            public GiveItem(int itemID)
            {
                ItemID = itemID;
            }

            public override Status OnExecute(Component agent, IBlackboard bb)
            {
                Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();

                if (PlayerTalking)
                {
                    PlayerTalking.Inventory.ReceiveItemReward(ItemID, 1, false);
                }

                return Status.Success;
            }
        }
    }
}
