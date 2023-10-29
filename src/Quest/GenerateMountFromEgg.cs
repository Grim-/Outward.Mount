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
            EmoMountMod.LogMessage($"GenerateMountFromEgg Task : Character is null");
            return;
        }

        CharacterMount CharacterMount = Character.GetComponent<CharacterMount>();

        if (CharacterMount == null)
        {
            EmoMountMod.LogMessage($"GenerateMountFromEgg Task : CharacterMount is null");
            return;
        }

        MountSpecies mountSpecies = EmoMountMod.MountManager.GetSpeciesDefinitionByName(SpeciesID);

        if (mountSpecies == null)
        {
            EmoMountMod.LogMessage($"GenerateMountFromEgg Task : MountSpecies is null");
            return;
        }

        BasicMountController ActiveMountForCharacter = EmoMountMod.MountManager.GetActiveMountForCharacter(Character);
        BasicMountController newMount = null;


        if (CharacterMount.HasActiveMount)
        {
                EmoMountMod.LogMessage($"GenerateMountFromEgg Task : Character has an active mount");
            newMount = EmoMountMod.MountManager.CreateMountFromSpecies(SpeciesID, Vector3.zero, Vector3.zero, Color.clear, Color.clear);
            SetTintColour(newMount, mountSpecies);
            Character.CharacterUI.ShowInfoNotification($"Your newly hatched {mountSpecies.SpeciesName} was sent to the stables!");
            CharacterMount.StoreMount(newMount);

        }
        else
        {
            EmoMountMod.LogMessage($"GenerateMountFromEgg Task : Character has no active mount");
            newMount = EmoMountMod.MountManager.CreateMountForCharacter(Character, SpeciesID, Character.transform.position, Character.transform.eulerAngles);
            Character.CharacterUI.ShowInfoNotification($"{mountSpecies.SpeciesName} has just hatched!");
            SetTintColour(newMount, mountSpecies);
        }


        if (newMount == null)
        {
            EmoMountMod.LogMessage($"GenerateMountFromEgg Task : Failed to Generate Mount From Egg.");
            EndAction(false);
            return;
        }

        EndAction(true);
    }


    private void SetTintColour(BasicMountController newMount, MountSpecies mountSpecies)
    {
        if (OverrideColor != Color.clear)
        {
            newMount.SetTintColor(OverrideColor);
        }
        else if (GenerateRandomTint)
        {
            newMount.SetTintColor(mountSpecies.GetRandomColor());
        }
    }
}
