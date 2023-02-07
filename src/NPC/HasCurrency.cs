using NodeCanvas.Framework;

public class HasCurrency : ConditionTask
{
    public int AmountRequired;

    public override bool OnCheck()
    {
        Character PlayerTalking = blackboard.GetVariable<Character>("gInstigator").GetValue();

        if (PlayerTalking && PlayerTalking.Inventory.AvailableMoney >= AmountRequired)
        {
            return true;
        }

        return false;
    }
}