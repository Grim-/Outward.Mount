using SideLoader;
using System;
using System.Collections.Generic;
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

        public override void ApplyToComponent<T>(T component)
        {
            SLEx_SpawnMount comp = component as SLEx_SpawnMount;
            comp.SpeciesName = SpeciesName;
        }

        public override void SerializeEffect<T>(T effect)
        {

        }
    }

    public class SLEx_SpawnMount : Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_SpawnMount);
        public Type GameModel => typeof(SLEx_SpawnMount);

        public string SpeciesName;

        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            if (!EmoMountMod.MountManager.CharacterHasMount(_affectedCharacter))
            {
                MountSpecies mountSpecies = EmoMountMod.MountManager.GetSpeciesDefinitionByName(SpeciesName);

                if (mountSpecies != null)
                {
                   BasicMountController basicMountController = EmoMountMod.MountManager.CreateMountFromSpecies(_affectedCharacter, mountSpecies, OutwardHelpers.GetPositionAroundCharacter(_affectedCharacter), Vector3.zero);
                   basicMountController.SetMountUI(MountCanvasManager.Instance.RegisterMount(basicMountController));
                }
                else
                {
                    EmoMountMod.Log.LogMessage($"SLEx_SpawnMount Could not find species with Species Name : {SpeciesName}, in the list of defintions.");
                }
            }
            else
            {
                EmoMountMod.Log.LogMessage($"SLEx_SpawnMount {_affectedCharacter.Name} already has an active mount.");
            }
        }
    }
}

