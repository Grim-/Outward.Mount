using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using UnityEngine;

namespace EmoMount
{
    public partial class EmoMountMod
    {
        public class SetMountEmissionColor : ActionNode
        {
            public Color TargetColor;

            public SetMountEmissionColor(Color targetColor)
            {
                TargetColor = targetColor;
            }

            public override Status OnExecute(Component agent, IBlackboard bb)
            {
                Character PlayerTalking = bb.GetVariable<Character>("gInstigator").GetValue();
                CharacterMount characterMount = PlayerTalking.GetComponent<CharacterMount>();

                if (characterMount && characterMount.HasActiveMount)
                {
                    characterMount.ActiveMount.SetEmissionColor(TargetColor);
                    return Status.Success;
                }

                return Status.Failure;
            }
        }
    }
}
