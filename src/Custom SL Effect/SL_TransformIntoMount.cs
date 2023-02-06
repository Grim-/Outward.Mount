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
        public string TransformVFX;

        public override void ApplyToComponent<T>(T component)
        {
            TransformIntoMount comp = component as TransformIntoMount;
            comp.SpeciesName = SpeciesName;
            comp.TransformVFX = TransformVFX;
        }

        public override void SerializeEffect<T>(T effect)
        {

        }
    }

    public class TransformIntoMount : Effect
    {
        public string SpeciesName;
        public string TransformVFX;

        public int Sheath;
        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            if (!EmoMountMod.MountManager.CharacterHasMount(_affectedCharacter))
            {
                if (_affectedCharacter.InCombat && EmoMountMod.EnableCombatTransforming.Value == false)
                {
                    return;
                }

                MountSpecies mountSpecies = EmoMountMod.MountManager.GetSpeciesDefinitionByName(SpeciesName);

                if (mountSpecies != null)
                {
                    _affectedCharacter.DodgeInput();
                    OutwardHelpers.SpawnTransformVFX(_affectedCharacter.Visuals.ActiveVisualsBody.Renderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);
                    OutwardHelpers.SpawnTransformVFX(_affectedCharacter.Visuals.ActiveVisualsFoot.Renderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);
                    OutwardHelpers.DelayDo(() =>
                    {  
                        BasicMountController basicMountController = EmoMountMod.MountManager.CreateMountFromSpecies(_affectedCharacter, SpeciesName, _affectedCharacter.transform.position, _affectedCharacter.transform.eulerAngles);
                        basicMountController.IsTransform = true;
                        basicMountController.MountFood.RequiresFood = false;

                        OutwardHelpers.SpawnTransformVFX(basicMountController.SkinnedMeshRenderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);

                        basicMountController.EventComp.OnMounted += (Character Character) =>
                        {
                            Character.VisualHolderTrans.gameObject.SetActive(false);
                            Character.CharacterUI.NotificationPanel.ShowNotification("You can revert by pressing the interact key");
                        };

                        basicMountController.EventComp.OnUnMounted += (Character Character) =>
                        {
                            OutwardHelpers.SpawnTransformVFX(_affectedCharacter.Visuals.ActiveVisualsBody.Renderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);
                            OutwardHelpers.SpawnTransformVFX(_affectedCharacter.Visuals.ActiveVisualsFoot.Renderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);
                            Character.VisualHolderTrans.gameObject.SetActive(true);
                            EmoMountMod.MountManager.DestroyMount(Character, basicMountController);
                        };

                        basicMountController.MountCharacter(_affectedCharacter);

                    }, 0.3f);
                }
            }

        }
    }
}
