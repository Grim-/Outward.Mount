using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using UnityEngine;

namespace EmoMount
{
    public partial class EmoMountMod
    {
        public class DismissMountActionNode : ActionNode
        {

            public override Status OnExecute(Component agent, IBlackboard bb)
            {
                //EmoMountMod.Log.LogMessage("Dismiss action called");

                Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();

                CharacterMount characterMount = PlayerTalking.GetComponentInChildren<CharacterMount>();

                if (characterMount != null && characterMount.HasActiveMount)
                {
                    characterMount.StoreMount(characterMount.ActiveMount);
                }

                return Status.Success;
            }
        }

        public class LearnMountSkillsNode : ActionNode
        {

            public override Status OnExecute(Component agent, IBlackboard bb)
            {
                Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();

                if (PlayerTalking.Inventory.SkillKnowledge.GetItemFromItemID(-26200) == null)
                {
                    PlayerTalking.Inventory.SkillKnowledge.AddItem(ItemManager.Instance.GenerateItem(-26200));
                }

                if (PlayerTalking.Inventory.SkillKnowledge.GetItemFromItemID(-26201) == null)
                {
                    PlayerTalking.Inventory.SkillKnowledge.AddItem(ItemManager.Instance.GenerateItem(-26201));
                }

                return Status.Success;
            }
        }
    }
}
