using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using UnityEngine;

namespace EmoMount
{
    public partial class EmoMountMod
    {
        public class DisplayMountStorageListNode : ActionNode
        {
            public override Status OnExecute(Component agent, IBlackboard bb)
            {
                //EmoMountMod.Log.LogMessage("Summon Mount action called");


                Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();

                CharacterMount characterMount = PlayerTalking.GetComponentInChildren<CharacterMount>();

                if (characterMount != null && !characterMount.HasActiveMount)
                {
                    EmoMountMod.MainCanvasManager.DisplayStorageForCharacter(PlayerTalking);
                }

                return Status.Success;
            }
        }
    }
}
