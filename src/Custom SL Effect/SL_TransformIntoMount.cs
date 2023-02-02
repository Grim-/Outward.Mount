using SideLoader;
using System;
using UnityEngine;

namespace EmoMount.Custom_SL_Effect
{
    public class SL_TransformIntoMount : SL_Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_TransformIntoMount);
        public Type GameModel => typeof(TransformIntoMount);

        public string SpeciesName;

        public override void ApplyToComponent<T>(T component)
        {
            TransformIntoMount comp = component as TransformIntoMount;
            comp.SpeciesName = SpeciesName;
        }

        public override void SerializeEffect<T>(T effect)
        {

        }
    }

    public class TransformIntoMount : Effect
    {
        public string SpeciesName;

        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            if (!EmoMountMod.MountManager.CharacterHasMount(_affectedCharacter))
            {
                MountSpecies mountSpecies = EmoMountMod.MountManager.GetSpeciesDefinitionByName(SpeciesName);

                if (mountSpecies != null)
                {


                    BasicMountController basicMountController = EmoMountMod.MountManager.CreateMountFromSpecies(_affectedCharacter, SpeciesName, _affectedCharacter.transform.position, _affectedCharacter.transform.eulerAngles);
                    basicMountController.IsTransform = true;

                    OutwardHelpers.SpawnSmokeTransformVFX(basicMountController.gameObject);

                    basicMountController.OnPlayerMounted += (Character Character) =>
                    {
                        Character.VisualHolderTrans.gameObject.SetActive(false);
                        Character.CharacterUI.NotificationPanel.ShowNotification("You can revert by pressing the interact key");
                    };

                    basicMountController.OnPlayerUnMounted += (Character Character) =>
                    {
                        OutwardHelpers.SpawnSmokeTransformVFX(Character.VisualHolderTrans.gameObject);
                        Character.VisualHolderTrans.gameObject.SetActive(true);
                        //Character.Teleport(basicMountController.transform.position, basicMountController.transform.rotation);

                        EmoMountMod.MountManager.DestroyMount(Character, basicMountController);
                    };

                    basicMountController.MountCharacter(_affectedCharacter);
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
