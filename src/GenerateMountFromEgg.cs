using EmoMount;
using NodeCanvas.Framework;

public class GenerateMountFromEgg : ActionTask
{  
    public string SpeciesID;
    public override void OnExecute()
    {
        Character Character = this.blackboard.GetValue<Character>("gQuestOwner");

        if (Character == null)
        {
            EmoMountMod.Log.LogMessage($"GenerateMountFromEgg Task : Character is null");
            return;
        }

        CharacterMount CharacterMount = Character.GetComponent<CharacterMount>();


        if (CharacterMount == null)
        {
            EmoMountMod.Log.LogMessage($"GenerateMountFromEgg Task : CharacterMount is null");
            return;
        }

        MountSpecies mountSpecies = EmoMountMod.MountManager.GetSpeciesDefinitionByName(SpeciesID);

        if (mountSpecies == null)
        {
            EmoMountMod.Log.LogMessage($"GenerateMountFromEgg Task : MountSpecies is null");
            return;
        }

        BasicMountController basicMountController = EmoMountMod.MountManager.GetActiveMountForCharacter(Character);

        if (basicMountController != null)
        {
            CharacterMount.StoreMount(basicMountController);
        }

        BasicMountController newMount = EmoMountMod.MountManager.CreateMountFromSpecies(Character, SpeciesID, Character.transform.position + new UnityEngine.Vector3(0, 0, 1), Character.transform.eulerAngles, mountSpecies.GetRandomColor());

        EmoMountMod.MainCanvasManager.RegisterMount(newMount);
    }
}