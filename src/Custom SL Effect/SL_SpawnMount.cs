using SideLoader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoMount
{
    /// <summary>
    /// Maybe skip using an SL_Effect all together.
    /// </summary>
    public class SL_SpawnMount : SL_Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_SpawnMount);
        public Type GameModel => typeof(SLEx_SpawnMount);

        public string SpeciesName;
        public bool GenerateRandomTint = false;
        public bool GenerateRandomEmission = false;
        public override void ApplyToComponent<T>(T component)
        {
            SLEx_SpawnMount comp = component as SLEx_SpawnMount;
            comp.SpeciesName = SpeciesName;
            comp.GenerateRandomTint = GenerateRandomTint;
            comp.GenerateRandomEmission = GenerateRandomEmission;
        }

        public override void SerializeEffect<T>(T effect)
        {
            SLEx_SpawnMount comp = effect as SLEx_SpawnMount;
            this.SpeciesName = comp.SpeciesName;
            this.GenerateRandomTint = comp.GenerateRandomTint;
            this.GenerateRandomEmission =  comp.GenerateRandomEmission;
        }
    }

    public class SLEx_SpawnMount : Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_SpawnMount);
        public Type GameModel => typeof(SLEx_SpawnMount);

        public string SpeciesName;

        public bool GenerateRandomTint = false;
        public bool GenerateRandomEmission = false;

        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            if (!EmoMountMod.MountManager.CharacterHasMount(_affectedCharacter))
            {
                MountSpecies mountSpecies = EmoMountMod.MountManager.GetSpeciesDefinitionByName(SpeciesName);

                if (mountSpecies != null)
                {
                    BasicMountController basicMountController = EmoMountMod.MountManager.CreateMountForCharacter(_affectedCharacter, SpeciesName, OutwardHelpers.GetPositionAroundCharacter(_affectedCharacter), Vector3.zero);

                    if (mountSpecies.GenerateRandomTint || GenerateRandomTint)
                    {
                        Color ChosenColor = WeightedItem<Color>.GetWeightedRandomValueFromList(OutwardHelpers.ConvertToWeightedItemList(mountSpecies.MountColors));
                        basicMountController.SetTintColor(ChosenColor);
                    }

                    if (mountSpecies.GenerateRandomEmission || GenerateRandomEmission)
                    {
                        Color ChosenColor = WeightedItem<Color>.GetWeightedRandomValueFromList(OutwardHelpers.ConvertToWeightedItemList(mountSpecies.MountEmissionColors)) * 3f;
                        basicMountController.SetEmissionColor(ChosenColor);
                    }
                
                    basicMountController.SetMountUI(MountCanvasManager.Instance.RegisterMount(basicMountController));
                }
                else
                {
                    EmoMountMod.LogMessage($"SLEx_SpawnMount Could not find species with Species Name : {SpeciesName}, in the list of defintions.");
                }
            }
            else
            {
                EmoMountMod.LogMessage($"SLEx_SpawnMount {_affectedCharacter.Name} already has an active mount.");
            }
        }
    }
}

