using SideLoader;
using System;
using System.ComponentModel;
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
            TransformIntoMount comp = effect as TransformIntoMount;
            this.SpeciesName = comp.SpeciesName;
            this.TransformVFX = comp.TransformVFX;
        }
    }



    public class TransformIntoMount : Effect
    {
        public string SpeciesName;
        public string TransformVFX;
        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            if (_affectedCharacter.InCombat && EmoMountMod.EnableCombatTransforming.Value == false)
            {
                return;
            }



            MountSpecies mountSpecies = EmoMountMod.MountManager.GetSpeciesDefinitionByName(SpeciesName);

            if (mountSpecies != null)
            {
                //dodge if moving in any way, otherwise mountcontroll will handle animation
                if (ControlsInput.MoveHorizontal(_affectedCharacter.OwnerPlayerSys.PlayerID) != 0 || ControlsInput.MoveVertical(_affectedCharacter.OwnerPlayerSys.PlayerID) != 0)
                {
                    _affectedCharacter.DodgeInput();
                }

                    
                OutwardHelpers.SpawnTransformVFX(_affectedCharacter.Visuals.ActiveVisualsBody.Renderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);
                OutwardHelpers.SpawnTransformVFX(_affectedCharacter.Visuals.ActiveVisualsFoot.Renderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);

                OutwardHelpers.DelayDo(() =>
                {  
                    BasicMountController basicMountController = EmoMountMod.MountManager.CreateMountForCharacter(_affectedCharacter, SpeciesName, _affectedCharacter.transform.position, _affectedCharacter.transform.eulerAngles, Color.clear, Color.clear, false);
                    basicMountController.IsTransform = true;
                    basicMountController.MountFood.RequiresFood = false;

                    OutwardHelpers.SpawnTransformVFX(basicMountController.SkinnedMeshRenderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);

                    basicMountController.EventComp.OnMounted += (BasicMountController MountController, Character Character) =>
                    {
                        //EmoMountMod.Log.LogMessage("Character : : " + Character);
                        MountController.CurrentlyMounted.IsTransformed = true;
                        MountController.CurrentlyMounted.SetActiveMount(MountController);
                        Character.VisualHolderTrans.gameObject.SetActive(false);
                        MountController.SetHitboxOwner(Character);
                        Character.CharacterUI.NotificationPanel.ShowNotification("You can revert by pressing the interact key");
                    };

                    basicMountController.EventComp.OnUnMounted += (BasicMountController MountController, Character Character) =>
                    {
                        MountController.CurrentlyMounted.IsTransformed = false;
                        MountController.CurrentlyMounted.SetActiveMount(null);
                        OutwardHelpers.SpawnTransformVFX(Character.Visuals.ActiveVisualsBody.Renderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);
                        OutwardHelpers.SpawnTransformVFX(Character.Visuals.ActiveVisualsFoot.Renderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);
                        Character.VisualHolderTrans.gameObject.SetActive(true);
                        EmoMountMod.MountManager.DestroyMount(Character, MountController);
                    };

                    basicMountController.MountCharacter(_affectedCharacter);

                }, 0.3f);
            }
            else EmoMountMod.Log.LogMessage($"SL_TransformIntoMount : : Could not find MountSpecies Definition for SpeciesName [{SpeciesName}]");
        }

    }
}