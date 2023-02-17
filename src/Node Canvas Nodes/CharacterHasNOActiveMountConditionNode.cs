using NodeCanvas.Framework;

namespace EmoMount
{
    public class CharacterHasNOActiveMountConditionNode : ConditionTask
    {
        public override bool OnCheck()
        {
            Character PlayerTalking = blackboard.GetVariable<Character>("gInstigator").GetValue();

            CharacterMount characterMount = PlayerTalking.GetComponentInChildren<CharacterMount>();

            if (characterMount != null && characterMount.HasActiveMount)
            {
                return false;
            }

            return true;
        }
    }
}
