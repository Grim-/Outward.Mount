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
                Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();

                CharacterMount characterMount = PlayerTalking.GetComponentInChildren<CharacterMount>();

                if (characterMount != null && !characterMount.HasActiveMount)
                {
                    EmoMountMod.MainCanvasManager.DisplayStorageForCharacter(PlayerTalking);
                    return Status.Success;
                }
                else
                {
                    return Status.Failure;
                }       
            }
        }
    }
}
