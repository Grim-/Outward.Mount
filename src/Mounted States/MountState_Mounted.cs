using EmoMount.Mounted_States;
using SideLoader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace EmoMount
{
    public class MountState_Mounted : BaseMountState
    {
        private Character MountedCharacter;
        private Vector3 StartPosition;
        private Vector3 AutoMoveTarget = Vector3.zero;
        private bool IsInManualMode = true;
        private int CurrentAreaSwitch = -1;
        private float CurrentStaminaAsPercent => MountedCharacter.Stats.CurrentStamina / MountedCharacter.Stats.MaxStamina;
        public MountState_Mounted(Character mountedCharacter)
        {
            MountedCharacter = mountedCharacter;
        }

        public override void OnEnter(BasicMountController MountController)
        {
            if (EmoMountMod.Debug)
            {
                MountController.DisplayNotification($"{MountController.MountName}, entered Mounted State");
            }

            MountController.DisableNavMeshAgent();
            MountController.SetIsMounted(true);
            StartPosition = MountController.transform.position;
        }

        public override void OnExit(BasicMountController MountController)
        {
            if (EmoMountMod.Debug)
            {
                MountController.DisplayNotification($"{MountController.MountName}, left Mounted State");
            }
        }
        public override void OnFixedUpdate(BasicMountController MountController)
        {

        }
        public override void OnUpdate(BasicMountController MountController)
        {
            if (MountedCharacter == null)
            {
                EmoMountMod.Log.LogMessage($"we somehow ended up in the MountedState without a Mounted Character dismounting character owner and popping state.");
                MountController.DismountCharacter(MountController.CharacterOwner);
                //if we somehow ended up in the MountedState without a Mounted Character
                Parent.PopState();
                return;
            }

            base.OnUpdate(MountController);


            float DistanceBetweenStartAndEnd = Vector3.Distance(StartPosition, MountController.transform.position);


            if (MountController.IsTransform)
            {
                if (CurrentStaminaAsPercent <= 0.1f)
                {
                    MountController.DisplayNotification($"You cannot sustain that form without restoring your stamina");
                    MountController.DismountCharacter(MountedCharacter);
                    Parent.PopState();
                }
  
                if (DistanceBetweenStartAndEnd > MountController.MountFood.TravelDistanceThreshold)
                {
                    OnTravelDistanceThreshold(MountController);
                }
               
            }
            else
            {
                if (DistanceBetweenStartAndEnd > MountController.MountFood.TravelDistanceThreshold)
                {
                    OnTravelDistanceThreshold(MountController);
                }               
            }





            if (ControlsInput.Interact(MountedCharacter.OwnerPlayerSys.PlayerID))
            {
                MountController.DismountCharacter(MountedCharacter);
                Parent.PopState();
            }

            if (MountedCharacter == null)
            {
                Parent.PopState();
            }

            if (IsInManualMode)
            {
                if (ControlsInput.Sprint(MountedCharacter.OwnerPlayerSys.PlayerID))
                {
                    MountController.IsSprinting = true;
                }
                else
                {
                    MountController.IsSprinting = false;
                }

                MountController.transform.forward = Vector3.RotateTowards(MountController.transform.forward, MountController.transform.forward + MountController.CameraRelativeInputNoY, MountController.RotateSpeed * Time.deltaTime, 6f);
                MountController.Controller.SimpleMove(Physics.gravity + MountController.CameraRelativeInput.normalized * MountController.ActualMoveSpeed);
            }
            else
            {
                if (MountController.MountFood.FoodAsNormalizedPercent <= 0.2f && MountController.IsSprinting)
                {
                    MountController.IsSprinting = false;
                    MountController.SetNavMeshMoveSpeed(MountController.ActualMoveSpeed);
                    MountController.DisplayImportantNotification($"{MountController.MountName} is too tired to sprint!");
                }




                if (AutoMoveTarget != Vector3.zero)
                {
                    float distance = Vector3.Distance(MountController.transform.position, AutoMoveTarget);

                    if (distance <= 7f)
                    {
                        MountController.PlayMountAnimation(MountAnimations.MOUNT_HAPPY);
                        SwitchToManual(MountController);
                        MountController.DisplayImportantNotification($"Ease up..{MountController.MountName}");
                    }
                }


                if (MountController.BaseInput != Vector3.zero)
                {
                    SwitchToManual(MountController);
                }
            }

            UpdateMenuInputs(MountController);       
        }
        public override void OnAttack1(BasicMountController MountController)
        {
            base.OnAttack1(MountController);


            if (EmoMountMod.EnableTestFeatures.Value)
            {

                List<InteractionSwitchArea> AreaSwitches = GetAreaSwitches();

                if (AreaSwitches != null)
                {
                    if (CurrentAreaSwitch >= AreaSwitches.Count)
                    {
                        CurrentAreaSwitch = -1;
                    }
                    else
                    {
                        CurrentAreaSwitch++;
                        MountController.DisplayImportantNotification($"Travel To {GetAreaSwitches()[CurrentAreaSwitch].m_area.DefaultName}?");
                    }
                }
            }
        }

        public override void OnSprint(BasicMountController MountController)
        {
            base.OnSprint(MountController);

            if (!IsInManualMode)
            {
                MountController.IsSprinting = !MountController.IsSprinting;
                MountController.SetNavMeshMoveSpeed(MountController.ActualMoveSpeed);
            }

        }
        public override void OnDodge(BasicMountController MountController)
        {
            base.OnDodge(MountController);

            MountController.PlayMountAnimation(MountAnimations.MOUNT_HAPPY);
        }
        public override void OnBlockReleased(BasicMountController MountController)
        {
            base.OnBlockReleased(MountController);



            if (EmoMountMod.EnableTestFeatures.Value)
            {
                List<InteractionSwitchArea> AreaSwitches = GetAreaSwitches();

                if (AreaSwitches != null)
                {
                    if (CurrentAreaSwitch >= 0)
                    {
                        if (CurrentAreaSwitch > AreaSwitches.Count)
                        {
                            CurrentAreaSwitch = AreaSwitches.Count;
                        }

                        SwitchAutoMove(MountController, TryGetNavMeshPositionOnTerrain(MountController, GetAreaSwitchPosition(AreaSwitches, CurrentAreaSwitch), 30f), AreaSwitches[CurrentAreaSwitch].m_area);
                    }
                }
            }



        }

        private Vector3 GetAreaSwitchPosition(List<InteractionSwitchArea> Switches, int CurrentAreaIndex)
        {
            return Switches[CurrentAreaIndex].transform.position;
        }


        public virtual void OnTravelDistanceThreshold(BasicMountController MountController)
        {
            if (EmoMountMod.EnableFoodNeed.Value && MountController.MountFood.RequiresFood)
            {
                MountController.MountFood.Remove(MountController.MountFood.FoodLostPerTravelDistance);
            }
            else if(MountController.IsTransform)
            {
                float PercentOfMax = MountController.CharacterOwner.Stats.MaxStamina * 0.05f;
                MountController.CharacterOwner.Stats.UseStamina(PercentOfMax, 1.1f);
            }

            StartPosition = MountController.transform.position;
        }
        private void SwitchAutoMove(BasicMountController MountController, Vector3 Target, Area Area)
        {
            if (Target == Vector3.zero)
            {
                return;
            }

            MountController.DisplayImportantNotification($"{MountController.MountName}, To {Area.DefaultName}!");
            MountController.Controller.enabled = false;

            MountController.EnableNavMeshAgent();
            Target.y = MountController.transform.position.y;

            MountController.NavMesh.SetDestination(Target);
            AutoMoveTarget = Target;
            IsInManualMode = false;
        }
        private void SwitchToManual(BasicMountController controller)
        {
            controller.DisplayImportantNotification($"Easy there..");
            controller.DisableNavMeshAgent();
            controller.Controller.enabled = true;
            AutoMoveTarget = Vector3.zero;
            IsInManualMode = true;
        }
        private void UpdateMenuInputs(BasicMountController MountController)
        {
            bool flag = false;
            int playerID = MountController.CharacterOwner.OwnerPlayerSys.PlayerID;
            if (MountController.CharacterOwner != null && MountController.CharacterOwner.CharacterUI != null && !MenuManager.Instance.InFade)
            {
                if ((MountController.CharacterOwner.CharacterUI.IsMenuFocused || MountController.CharacterOwner.CharacterUI.IsDialogueInProgress) && ControlsInput.MenuCancel(playerID))
                {
                    MountController.CharacterOwner.CharacterUI.CancelMenu();
                }
                if (!MountController.CharacterOwner.CharacterUI.IgnoreMenuInputs)
                {
                    if (!MountController.CharacterOwner.CurrentlyChargingAttack && !MountController.CharacterOwner.CharacterUI.IsDialogueInProgress && !MountController.CharacterOwner.CharacterUI.IsMenuJustToggled && !MountController.CharacterOwner.CharacterUI.IsOptionPanelDisplayed)
                    {
                        if (ControlsInput.ToggleInventory(playerID))
                        {
                            MountController.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.Inventory, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleEquipment(playerID))
                        {
                            MountController.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.Equipment, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleQuestLog(playerID))
                        {
                            MountController.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.QuestLog, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleSkillMenu(playerID))
                        {
                            MountController.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.Skills, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleCharacterStatusMenu(playerID))
                        {
                            MountController.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.CharacterStatus, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleEffectMenu(playerID))
                        {
                            MountController.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.PlayerEffects, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleQuickSlotMenu(playerID))
                        {
                            MountController.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.QuickSlotAssignation, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleCraftingMenu(playerID))
                        {
                            MountController.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.Crafting, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleMap(playerID) && (!ControlsInput.IsLastActionGamepad(playerID) || !MountController.CharacterOwner.CharacterUI.IsMenuFocused || MountController.CharacterOwner.CharacterUI.IsMapDisplayed))
                        {
                            MenuManager.Instance.ToggleMap(MountController.CharacterOwner.CharacterUI);
                        }
                    }
                    if (ControlsInput.ExitContainer(playerID))
                    {
                        MountController.CharacterOwner.CharacterUI.CloseContainer();
                    }
                    if (ControlsInput.TakeAll(playerID))
                    {
                        MountController.CharacterOwner.CharacterUI.TakeAllItemsInput();
                    }
                    if (MountController.CharacterOwner.CharacterUI.IsMenuFocused)
                    {
                        if (!MountController.CharacterOwner.CharacterUI.IsMenuJustToggled)
                        {
                            if (ControlsInput.InfoInput(playerID))
                            {
                                MountController.CharacterOwner.CharacterUI.InfoInputMenu();
                            }
                            if (ControlsInput.MenuShowDetails(playerID))
                            {
                                MountController.CharacterOwner.CharacterUI.OptionInputMenu();
                            }
                        }
                        if (ControlsInput.GoToPreviousMenu(playerID))
                        {
                            MountController.CharacterOwner.CharacterUI.GoToPreviousTab();
                        }
                        if (ControlsInput.GoToNextMenu(playerID))
                        {
                            MountController.CharacterOwner.CharacterUI.GoToNextTab();
                        }
                    }
                }
                if (ControlsInput.ToggleChatMenu(playerID) && !MountController.CharacterOwner.CurrentlyChargingAttack && !MountController.CharacterOwner.CharacterUI.IsMenuFocused && !MountController.CharacterOwner.CharacterUI.IsDialogueInProgress && !MountController.CharacterOwner.CharacterUI.ChatPanel.IsChatFocused && !MountController.CharacterOwner.CharacterUI.ChatPanel.JustUnfocused)
                {
                    MountController.CharacterOwner.CharacterUI.ShowAndFocusChat();
                    flag = true;
                }
                if (ControlsInput.ToggleHelp(playerID) && !MenuManager.Instance.IsConnectionScreenDisplayed && !MountController.CharacterOwner.CharacterUI.IsMenuJustToggled && !MountController.CharacterOwner.CharacterUI.IsInputFieldJustUnfocused && !MountController.CharacterOwner.Deploying && ((!MountController.CharacterOwner.CharacterUI.IsMenuFocused && !MountController.CharacterOwner.CharacterUI.IsDialogueInProgress) || MountController.CharacterOwner.CharacterUI.GetIsMenuDisplayed(CharacterUI.MenuScreens.PauseMenu)))
                {
                    MountController.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.PauseMenu, true);
                }
            }
            if (flag && MountController.CharacterOwner.Deploying)
            {
                MountController.CharacterOwner.DeployInput(-1);
            }
        }

        private Vector3 TryGetNavMeshPositionOnTerrain(BasicMountController MountController, Vector3 OriginTarget, float Distance = 10f)
        {
            if (NavMesh.SamplePosition(OriginTarget, out NavMeshHit hit, Distance, NavMesh.AllAreas))
            {
                return hit.position;
            }


            MountController.DisplayImportantNotification("Cannot find a path to Target.");
            return Vector3.zero;
        }
        private List<InteractionSwitchArea> GetAreaSwitches()
        {
            List<InteractionSwitchArea> AreaSwitches = new List<InteractionSwitchArea>();

            if (SpawnPointManager.Instance == null)
            {
                return null;
            }


            foreach (var item in SpawnPointManager.Instance.SpawnPoints)
            {
                InteractionSwitchArea interactionSwitchArea = item.GetComponent<InteractionSwitchArea>();

                if (interactionSwitchArea)
                {
                    if (item.name.Contains("Cierzo") || item.name.Contains("HallowedMarsh") || item.name.Contains("Levant") || item.name.Contains("Berg")
                        || item.name.Contains("Abrassar") || item.name.Contains("Monsoon") || item.name.Contains("Harmattan"))
                    {
                        AreaSwitches.Add(interactionSwitchArea);
                    }
                }
            }

            return AreaSwitches;
        }
    }


    public class MountState_MountedManual : MountState_Mounted
    {
        public MountState_MountedManual(Character mountedCharacter) : base(mountedCharacter)
        {

        }


        public override void OnTravelDistanceThreshold(BasicMountController MountController)
        {
            base.OnTravelDistanceThreshold(MountController);
        }

    }

    public class MountState_MountedAuto : MountState_Mounted
    {
        public MountState_MountedAuto(Character mountedCharacter) : base(mountedCharacter)
        {

        }


    }
}