﻿using SideLoader;
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
                    //dodge if moving in any way, otherwise mountcontroll will handle animation
                    if (ControlsInput.MoveHorizontal(_affectedCharacter.OwnerPlayerSys.PlayerID) != 0 || ControlsInput.MoveVertical(_affectedCharacter.OwnerPlayerSys.PlayerID) != 0)
                    {
                        _affectedCharacter.DodgeInput();
                    }

                    
                    OutwardHelpers.SpawnTransformVFX(_affectedCharacter.Visuals.ActiveVisualsBody.Renderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);
                    OutwardHelpers.SpawnTransformVFX(_affectedCharacter.Visuals.ActiveVisualsFoot.Renderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);
                    OutwardHelpers.DelayDo(() =>
                    {  
                        BasicMountController basicMountController = EmoMountMod.MountManager.CreateMountForCharacter(_affectedCharacter, SpeciesName, _affectedCharacter.transform.position, _affectedCharacter.transform.eulerAngles);
                        basicMountController.IsTransform = true;
                        basicMountController.MountFood.RequiresFood = false;

                        OutwardHelpers.SpawnTransformVFX(basicMountController.SkinnedMeshRenderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);

                        basicMountController.EventComp.OnMounted += OnMountedHandler;

                        basicMountController.EventComp.OnUnMounted += OnUnMountedHandler;

                        basicMountController.MountCharacter(_affectedCharacter);

                    }, 0.3f);
                }
            }

        }


        private void OnMountedHandler(BasicMountController MountController, Character Character)
        {
            Character.VisualHolderTrans.gameObject.SetActive(false);
            Character.CharacterUI.NotificationPanel.ShowNotification("You can revert by pressing the interact key");
        }

        private void OnUnMountedHandler(BasicMountController MountController, Character Character)
        {
            OutwardHelpers.SpawnTransformVFX(Character.Visuals.ActiveVisualsBody.Renderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);
            OutwardHelpers.SpawnTransformVFX(Character.Visuals.ActiveVisualsFoot.Renderer, 3, TransformVFX, ParticleSystemSimulationSpace.World);
            Character.VisualHolderTrans.gameObject.SetActive(true);
            EmoMountMod.MountManager.DestroyMount(Character, MountController);
        }
    }
}