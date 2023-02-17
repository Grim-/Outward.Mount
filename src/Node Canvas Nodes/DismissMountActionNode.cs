using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using UnityEngine;

namespace EmoMount
{
    public class DismissMountActionNode : ActionNode
    {

        public override Status OnExecute(Component agent, IBlackboard bb)
        {
            Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();

            CharacterMount characterMount = PlayerTalking.GetComponentInChildren<CharacterMount>();

            if (characterMount != null && characterMount.HasActiveMount)
            {
                characterMount.StoreMount(characterMount.ActiveMount);
            }

            return Status.Success;
        }
    } 
}
