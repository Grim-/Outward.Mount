using SideLoader;
using UnityEngine;

namespace EmoMount
{
    public class BaseMountedState : BaseState<BasicMountController>
    {
        private Character MountedCharacter;


        private Vector3 StartPosition;
        private Vector3 EndPosition;
        private float Timer = 0;


        public BaseMountedState(Character mountedCharacter)
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
            MountController.IsMounted = true;
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
            float DistanceBetweenStartAndEnd = Vector3.Distance(StartPosition, MountController.transform.position);
            

            if (DistanceBetweenStartAndEnd > MountController.MountedDistanceFoodThreshold)
            {
                MountController.DisplayNotification($"{DistanceBetweenStartAndEnd} Distance between start and End");
                StartPosition = MountController.transform.position;
                MountController.MountFood.Remove(MountController.FoodLostPerMountedDistance);
            }


            if (CustomKeybindings.GetKeyDown(EmoMountMod.MOUNT_DISMOUNT_KEY))
            {
                MountController.DismountCharacter(MountedCharacter);
                Parent.PopState();
            }

            if (MountedCharacter == null)
            {
                Parent.PopState();
            }


            MountController.transform.forward = Vector3.RotateTowards(MountController.transform.forward, MountController.transform.forward + MountController.CameraRelativeInputNoY, MountController.RotateSpeed * Time.deltaTime, 6f);
            MountController.Controller.SimpleMove(MountController.CameraRelativeInput.normalized * MountController.ActualMoveSpeed);

            UpdateAnimator(MountController);
            UpdateMenuInputs(MountController);
        }

        public void UpdateAnimator(BasicMountController MountController)
        {
            MountController.Animator.SetFloat("Move X", MountController.BaseInput.x, 5f, 5f);
            MountController.Animator.SetFloat("Move Z", MountController.BaseInput.z != 0 ? 1f : 0f, 5f, 5f);
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
    }
}
