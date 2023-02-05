using EmoMount;
using NodeCanvas.Framework;
using UnityEngine;

public class GenerateMountFromEgg : ActionTask<QuestProgress>
{  
    public string SpeciesID;

    public bool GenerateRandomTint = false;
    public Color OverrideColor = Color.clear;

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

        //store current if exists?
        if (basicMountController != null)
        {
            CharacterMount.StoreMount(basicMountController);
        }

        BasicMountController newMount = EmoMountMod.MountManager.CreateMountFromSpecies(Character, SpeciesID, Character.transform.position + new Vector3(0, 0, 1), Character.transform.eulerAngles);


        if (OverrideColor != Color.clear)
        {
            newMount.SetTintColor(OverrideColor);
        }
        else if(GenerateRandomTint)
        {
            newMount.SetTintColor(mountSpecies.GetRandomColor());
        }

        EmoMountMod.MainCanvasManager.RegisterMount(newMount);

        EndAction(true);
    }
}
