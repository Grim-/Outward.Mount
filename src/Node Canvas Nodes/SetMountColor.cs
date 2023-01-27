using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using UnityEngine;

namespace EmoMount
{
    public partial class EmoMountMod
    {
        public class SetMountColor : ActionNode
        {
            public Color TargetColor;

            public SetMountColor(Color targetColor)
            {
                TargetColor = targetColor;
            }

            public override Status OnExecute(Component agent, IBlackboard bb)
            {
                Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();
                CharacterMount characterMount = PlayerTalking.GetComponent<CharacterMount>();

                if (characterMount && characterMount.HasActiveMount)
                {
                    characterMount.ActiveMount.SetTintColor(TargetColor);
                    return Status.Success;
                }

                return Status.Failure;
            }
        }
    }
}
