using SideLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace EmoMount
{
    public class BasicMountController : MonoBehaviour
    {
        //Character

        #region Properties
        public Character CharacterOwner
        {
            get; private set;
        }
        public Animator Animator
        {
            get; private set;
        }
        public CharacterController Controller
        {
            get; private set;
        }
        public NavMeshAgent NavMesh
        {
            get; private set;
        }
        public Item BagContainer
        {
            get; private set;
        }
        public Emo_MountFood MountFood
        {
            get; private set;
        }

        public string MountName
        {
            get; set;
        }

        public string SLPackName
        {
            get; set;
        }

        public string AssetBundleName
        {
            get; set;
        }

        public string PrefabName
        {
            get; set;
        }
       
        public MountUI MountUI
        {
            get; private set;
        }


        public string MountUID
        {
            get; set;
        }

        #endregion

        //Mount Movement Settings
        public float MoveSpeed { get; private set; }
        private float ActualMoveSpeed => WeightAsNormalizedPercent > WeightEncumberenceLimit ? MoveSpeed * EncumberenceModifier : MoveSpeed;
        public float RotateSpeed { get; private set; }
        private float LeashDistance = 6f;
        //A Point is randomly chosen in LeashPointRadius around player to leash to.
        private float LeashPointRadius = 2.3f;
        private float TargetStopDistance = 1.4f;
        private float MoveToRayCastDistance = 20f;
        private LayerMask MoveToLayerMask => LayerMask.GetMask("LargeTerrainEnvironment");

        //weight
        private float CurrentCarryWeight = 0;
        //no idea on a reasonable number for any of this
        private float MaxCarryWeight = 90f;
        private float WeightEncumberenceLimit = 0.75f;
        private float EncumberenceModifier = 0.5f;
        private float WeightAsNormalizedPercent => CurrentCarryWeight / MaxCarryWeight;


        private Vector3 OriginalPlayerCameraOffset;
        private Vector3 MountedCameraOffset = new Vector3(0, 0, -4);


        //state variables
        private MountState MountState = MountState.UNMOUNTED;
        private bool StayPut = false;
        private Vector3 MoveToTarget = Vector3.zero;
        //how often is food taken while mounted
        private float MountedHungerTickTime = 30f;
        private float MountedHungerTimer = 0;

        private string[] Names = new string[]
        {
                "Buddy",
                "Maisie",
                "Taff",
                "Lola",
                "Mooch",
                "Ebony"
        };


        private string[] Actions = new string[]
        {
                "DoMountSpecial",
                "DoMountAttack",
                "HitReact"
        };




        public bool IsMoving
        {
            get
            {
                if (MountState == MountState.MOUNTED)
                {
                    return BaseInput.z != 0;
                }
                else if (MountState == MountState.UNMOUNTED)
                {
                    return NavMesh.velocity != Vector3.zero;
                }

                return false;
            }
        }


        //Input - tidy up doesnt need this many calls or new v3s
        private Vector3 BaseInput => new Vector3(ControlsInput.MoveHorizontal(CharacterOwner.OwnerPlayerSys.PlayerID), 0, ControlsInput.MoveVertical(CharacterOwner.OwnerPlayerSys.PlayerID));
        private Vector3 CameraRelativeInput => Camera.main.transform.TransformDirection(BaseInput);
        private Vector3 CameraRelativeInputNoY => new Vector3(CameraRelativeInput.x, 0, CameraRelativeInput.z);

        public void Awake()
        {
            Animator = GetComponent<Animator>();
            Controller = GetComponent<CharacterController>();
            NavMesh = gameObject.AddComponent<NavMeshAgent>();
            MountFood = gameObject.AddComponent<Emo_MountFood>();
            MountFood.Init();

            NavMesh.stoppingDistance = 1f;
            MountName = Names[Random.Range(0, Names.Length)];
            MountUID = Guid.NewGuid().ToString();
        }


        public void SetOwner(Character mountTarget)
        {
            CharacterOwner = mountTarget;
        }

        public void SetInventory(Item bag)
        {
            BagContainer = bag;
            EmoMountMod.Log.LogMessage(BagContainer);
        
            if (BagContainer != null)
            {
                StartCoroutine(SetUpBag(BagContainer));
            }           
        }

        public void SetMountUI(MountUI mountUI)
        {
            MountUI = mountUI;
        }


        private IEnumerator SetUpBag(Item bag)
        {
            yield return null;

            Transform mountPointTransform = transform.FindInAllChildren("SL_BAGPOINT");

            if (mountPointTransform != null)
            {
                BagContainer.transform.parent = mountPointTransform;
            }
            else
            {
                BagContainer.transform.parent = transform;
            }

            Bag itemBag = bag as Bag;

            bag.transform.localPosition = Vector3.zero;
            bag.transform.localEulerAngles = Vector3.zero;

            if (bag.m_rigidBody)
            {

                bag.m_rigidBody.isKinematic = true;
                bag.m_rigidBody.useGravity = false;
                bag.m_rigidBody.freezeRotation = true;
            }


            InteractionActivator interactionActivator = bag.gameObject.GetComponentInChildren<InteractionActivator>();


            yield break;
        }

        private void DebugInteractionStuff(Bag bag)
        {
            InteractionOpenContainer interactionOpenContainer = bag.gameObject.GetComponentInChildren<InteractionOpenContainer>();

            InteractionActivator interactionActivator = bag.gameObject.GetComponentInChildren<InteractionActivator>();

            EmoMountMod.Log.LogMessage($"Default Interaction {interactionActivator.m_defaultBasicInteraction}");
            EmoMountMod.Log.LogMessage($"Default Hold Interaction {interactionActivator.m_defaultHoldInteraction}");

            EmoMountMod.Log.LogMessage($"Basic Interaction Overrides {interactionActivator.m_basicInteractionOverrides.Count}");

            foreach (var item in interactionActivator.m_basicInteractionOverrides)
            {
                EmoMountMod.Log.LogMessage($"Basic Interaction Overrides");
                EmoMountMod.Log.LogMessage($"{item.Text}");
                EmoMountMod.Log.LogMessage($"{item.ShortText}");
                EmoMountMod.Log.LogMessage($"{item.GetType()}");
            }


            EmoMountMod.Log.LogMessage($"Default Hold Interaction {interactionActivator.m_holdInteractionOverrides}");

            foreach (var item in interactionActivator.m_holdInteractionOverrides)
            {
                EmoMountMod.Log.LogMessage($"Hold Interaction Overrides");
                EmoMountMod.Log.LogMessage($"{item.Text}");
                EmoMountMod.Log.LogMessage($"{item.ShortText}");
                EmoMountMod.Log.LogMessage($"{item.GetType()}");
            }


            EmoMountMod.Log.LogMessage($"Basic Interaction Default Basic Interaction");
            EmoMountMod.Log.LogMessage($"{interactionActivator.m_defaultBasicInteraction.Text}");

            EmoMountMod.Log.LogMessage($"Basic Interaction Default Scene Basic Interaction");
            EmoMountMod.Log.LogMessage($"{interactionActivator.m_sceneBasicInteraction}");

            EmoMountMod.Log.LogMessage($"Basic Interaction Default Scene Basic Interaction");
            EmoMountMod.Log.LogMessage($"{interactionActivator.m_sceneBasicInteraction}");


            foreach (var item in bag.gameObject.GetComponentsInChildren<IInteraction>())
            {
                EmoMountMod.Log.LogMessage($"IInteractions");
                EmoMountMod.Log.LogMessage($"{item.Text}");
                EmoMountMod.Log.LogMessage($"{item.GetType()}");
                EmoMountMod.Log.LogMessage($"{item.ShortText}");
                EmoMountMod.Log.LogMessage($"{item.ToString()}");
            }

            foreach (var item in bag.gameObject.GetComponentsInChildren<IItemInteraction>())
            {
                EmoMountMod.Log.LogMessage($"IItemInteractions");
                EmoMountMod.Log.LogMessage($"{item.Item}");
                EmoMountMod.Log.LogMessage($"{item.GetType()}");
                EmoMountMod.Log.LogMessage($"{item.ShowItemPreview}");
            }

        }

        public void SetPrefabDetails(string SLPackName, string AssetBundleName, string PrefabName)
        {
            this.SLPackName = SLPackName;
            this.AssetBundleName = AssetBundleName;
            this.PrefabName = PrefabName;
        }

        public void SetNavMeshMoveSpeed(float newSpeed)
        {
            if (NavMesh != null)
            {
                NavMesh.speed = newSpeed;
                NavMesh.acceleration = newSpeed * 0.5f;
            }
        }
        public void SetMoveSpeed(float newSpeed)
        {
            MoveSpeed = newSpeed;
        }
        public void SetRotationSpeed(float newSpeed)
        {
            RotateSpeed = newSpeed;
        }
        public void SetCameraOffset(Vector3 newOffset)
        {
            MountedCameraOffset = newOffset;
        }
        public void SetCharacterCameraOffset(Character _affectedCharacter, Vector3 NewOffset)
        {
            _affectedCharacter.CharacterCamera.Offset = NewOffset;
        }

        //commenting this so I can understand it in the future, very complex statemachine
        public void Update()
        {
            if (BagContainer != null)
            {
                BagContainer.transform.localPosition = Vector3.zero;
                BagContainer.transform.localEulerAngles = Vector3.zero;

                if (BagContainer.m_rigidBody)
                {

                    BagContainer.m_rigidBody.isKinematic = true;
                    BagContainer.m_rigidBody.useGravity = false;
                    BagContainer.m_rigidBody.freezeRotation = true;
                }
            }

            switch (MountState)
            {
                case MountState.UNMOUNTED:
                    UpdateUnMountedState();
                    break;
                case MountState.MOUNTED:
                    UpdateMountedState();
                    UpdateMenuInputs();
                    UpdateWeapon();
                    break;
            }
        }

        //ripped from localcharactercontrol works if I re-enable the animator and turn off root motion
        private void UpdateWeapon()
        {
            int playerID = this.CharacterOwner.OwnerPlayerSys.PlayerID;
            bool flag = ControlsInput.QuickSlotToggled(playerID);
            if (ControlsInput.Sheathe(playerID) && !this.CharacterOwner.Blocking)
            {
                this.CharacterOwner.SheatheInput();
            }


            bool flag2 = (this.CharacterOwner.CurrentWeapon != null && this.CharacterOwner.CurrentWeapon.AttackOnRelease && ControlsInput.AttackWhenZoomed(playerID)) || ControlsInput.Attack1(playerID);
            bool flag3 = false;
            if (this.CharacterOwner.CurrentWeapon == null || !this.CharacterOwner.CurrentWeapon.SpecialIsZoom)
            {
                flag3 = ControlsInput.Attack2(playerID);
            }
            if (!flag && (flag2 || flag3))
            {
                if (this.CharacterOwner.Sheathed)
                {
                    this.CharacterOwner.SheatheInput();
                }
                else
                {
                    this.CharacterOwner.AttackInput(flag2 ? 0 : 1, 0);
                   // this.StopAutoRun();
                }
            }
            bool flag4 = false;
            if (this.CharacterOwner.CurrentWeapon == null || !this.CharacterOwner.CurrentWeapon.AttackOnRelease)
            {
                flag4 = ControlsInput.Attack1Release(playerID);
            }
            else if (!ControlsInput.Attack1Press(playerID) && !ControlsInput.AttackWhenZoomedPress(playerID))
            {
                flag4 = (ControlsInput.Attack1Release(playerID) || ControlsInput.AttackWhenZoomedRelease(playerID));
            }
            bool flag5 = false;
            if (this.CharacterOwner.CurrentWeapon == null || !this.CharacterOwner.CurrentWeapon.SpecialIsZoom)
            {
                flag5 = ControlsInput.Attack2Release(playerID);
            }
        }
        //ripped from localcharactercontrol works
        private void UpdateMenuInputs()
        {
            bool flag = false;
            int playerID = this.CharacterOwner.OwnerPlayerSys.PlayerID;
            if (this.CharacterOwner != null && this.CharacterOwner.CharacterUI != null && !MenuManager.Instance.InFade)
            {
                if ((this.CharacterOwner.CharacterUI.IsMenuFocused || this.CharacterOwner.CharacterUI.IsDialogueInProgress) && ControlsInput.MenuCancel(playerID))
                {
                    this.CharacterOwner.CharacterUI.CancelMenu();
                }
                if (!this.CharacterOwner.CharacterUI.IgnoreMenuInputs)
                {
                    if (!this.CharacterOwner.CurrentlyChargingAttack && !this.CharacterOwner.CharacterUI.IsDialogueInProgress && !this.CharacterOwner.CharacterUI.IsMenuJustToggled && !this.CharacterOwner.CharacterUI.IsOptionPanelDisplayed)
                    {
                        if (ControlsInput.ToggleInventory(playerID))
                        {
                            this.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.Inventory, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleEquipment(playerID))
                        {
                            this.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.Equipment, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleQuestLog(playerID))
                        {
                            this.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.QuestLog, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleSkillMenu(playerID))
                        {
                            this.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.Skills, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleCharacterStatusMenu(playerID))
                        {
                            this.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.CharacterStatus, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleEffectMenu(playerID))
                        {
                            this.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.PlayerEffects, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleQuickSlotMenu(playerID))
                        {
                            this.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.QuickSlotAssignation, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleCraftingMenu(playerID))
                        {
                            this.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.Crafting, true);
                            flag = true;
                        }
                        if (ControlsInput.ToggleMap(playerID) && (!ControlsInput.IsLastActionGamepad(playerID) || !this.CharacterOwner.CharacterUI.IsMenuFocused || this.CharacterOwner.CharacterUI.IsMapDisplayed))
                        {
                            MenuManager.Instance.ToggleMap(this.CharacterOwner.CharacterUI);
                        }
                    }
                    if (ControlsInput.ExitContainer(playerID))
                    {
                        this.CharacterOwner.CharacterUI.CloseContainer();
                    }
                    if (ControlsInput.TakeAll(playerID))
                    {
                        this.CharacterOwner.CharacterUI.TakeAllItemsInput();
                    }
                    if (this.CharacterOwner.CharacterUI.IsMenuFocused)
                    {
                        if (!this.CharacterOwner.CharacterUI.IsMenuJustToggled)
                        {
                            if (ControlsInput.InfoInput(playerID))
                            {
                                this.CharacterOwner.CharacterUI.InfoInputMenu();
                            }
                            if (ControlsInput.MenuShowDetails(playerID))
                            {
                                this.CharacterOwner.CharacterUI.OptionInputMenu();
                            }
                        }
                        if (ControlsInput.GoToPreviousMenu(playerID))
                        {
                            this.CharacterOwner.CharacterUI.GoToPreviousTab();
                        }
                        if (ControlsInput.GoToNextMenu(playerID))
                        {
                            this.CharacterOwner.CharacterUI.GoToNextTab();
                        }
                    }
                }
                if (ControlsInput.ToggleChatMenu(playerID) && !this.CharacterOwner.CurrentlyChargingAttack && !this.CharacterOwner.CharacterUI.IsMenuFocused && !this.CharacterOwner.CharacterUI.IsDialogueInProgress && !this.CharacterOwner.CharacterUI.ChatPanel.IsChatFocused && !this.CharacterOwner.CharacterUI.ChatPanel.JustUnfocused)
                {
                    this.CharacterOwner.CharacterUI.ShowAndFocusChat();
                    flag = true;
                }
                if (ControlsInput.ToggleHelp(playerID) && !MenuManager.Instance.IsConnectionScreenDisplayed && !this.CharacterOwner.CharacterUI.IsMenuJustToggled && !this.CharacterOwner.CharacterUI.IsInputFieldJustUnfocused && !this.CharacterOwner.Deploying && ((!this.CharacterOwner.CharacterUI.IsMenuFocused && !this.CharacterOwner.CharacterUI.IsDialogueInProgress) || this.CharacterOwner.CharacterUI.GetIsMenuDisplayed(CharacterUI.MenuScreens.PauseMenu)))
                {
                    this.CharacterOwner.CharacterUI.ToggleMenu(CharacterUI.MenuScreens.PauseMenu, true);
                }
            }
            if (flag && this.CharacterOwner.Deploying)
            {
                this.CharacterOwner.DeployInput(-1);
            }
        }
        public void MountCharacter(Character _affectedCharacter)
        {
            float PlayerTotalWeight = _affectedCharacter.Inventory.TotalWeight;
            //todo own totalweight + playerTotal weight

            if (!WillObey())
            {
                DisplayNotification($"{MountName} is too hungry and refuses.");
            }


            if (PlayerTotalWeight < MaxCarryWeight)
            {
                _affectedCharacter.CharMoveBlockCollider.enabled = false;
                _affectedCharacter.CharacterController.enabled = false;
                _affectedCharacter.CharacterControl.enabled = false;
                //cancel movement in animator
                _affectedCharacter.SetAnimMove(0, 0);
                _affectedCharacter.SpellCastAnim(Character.SpellCastType.Sit, Character.SpellCastModifier.Immobilized, 1);
                TryToParent(_affectedCharacter, gameObject);
                OriginalPlayerCameraOffset = _affectedCharacter.CharacterCamera.Offset;
                SetCharacterCameraOffset(_affectedCharacter, OriginalPlayerCameraOffset + MountedCameraOffset);
                DisableNavMeshAgent();
                MountState = MountState.MOUNTED;
                UpdateCurrentWeight(PlayerTotalWeight);
            }
            else
            {
                DisplayNotification($"You are carrying too much weight to mount {MountName}");
            }



        }
        public void DismountCharacter(Character _affectedCharacter)
        {
            //_affectedCharacter.enabled = true;
            _affectedCharacter.CharMoveBlockCollider.enabled = true;
            _affectedCharacter.CharacterController.enabled = true;
            _affectedCharacter.CharacterControl.enabled = true;
            _affectedCharacter.Animator.enabled = true;
            _affectedCharacter.Animator.Update(Time.deltaTime);

            _affectedCharacter.transform.parent = null;
            _affectedCharacter.transform.position = transform.position;
            _affectedCharacter.transform.eulerAngles = Vector3.zero;

            SetCharacterCameraOffset(_affectedCharacter, OriginalPlayerCameraOffset);
            MountState = MountState.UNMOUNTED;
        }
        private void TryToParent(Character _affectedCharacter, GameObject MountInstance)
        {
            //probably insanely inefficient, or uses some bizzare form of windings to find the transform, who knows with extension methods :shrug:
            Transform mountPointTransform = transform.FindInAllChildren("SL_MOUNTPOINT");

            if (mountPointTransform != null)
            {
                _affectedCharacter.transform.parent = mountPointTransform;
                _affectedCharacter.transform.localPosition = Vector3.zero;
                _affectedCharacter.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                _affectedCharacter.transform.parent = MountInstance.transform;
                _affectedCharacter.transform.localPosition = Vector3.zero;
                _affectedCharacter.transform.localEulerAngles = Vector3.zero;

            }
        }
        //redo this
        private void UpdateMountedState()
        {
            DisableNavMeshAgent();

            if (CustomKeybindings.GetKeyDown("TEST_DISMOUNT_BUTTON") && CharacterOwner != null)
            {
                DismountCharacter(CharacterOwner);
            }


            transform.forward = Vector3.RotateTowards(transform.forward, transform.forward + CameraRelativeInputNoY, RotateSpeed * Time.deltaTime, 6f);

            Controller.SimpleMove(CameraRelativeInput.normalized * ActualMoveSpeed);

            Animator.SetFloat("Move X", BaseInput.x, 5f, 5f);
            Animator.SetFloat("Move Z", BaseInput.z != 0 ? 1f : 0f, 5f, 5f);

            UpdateMountedHunger();
        }


        //Im not sure if this should be distance travelled between ticks or just a regular tick
        private void UpdateMountedHunger()
        {
            MountedHungerTimer += Time.deltaTime;

            if (MountedHungerTimer > MountedHungerTickTime)
            {
                MountedHungerTimer = 0;
                OnMountedHungerTick();
            }
        }

        private void OnMountedHungerTick()
        {

            if (IsMoving)
            {

                EmoMountMod.Log.LogMessage("IS moving");

                //if at 75% weight capaicty
                if (WeightAsNormalizedPercent > .75f)
                {
                    MountFood.Remove(5f);
                }
                //if above 60 but below 75
                else if (WeightAsNormalizedPercent < .75 && WeightAsNormalizedPercent > .6f)
                {
                    MountFood.Remove(3.5f);
                }
                else
                {
                    MountFood.Remove(2f);
                }
            }
            else
            {
                EmoMountMod.Log.LogMessage("IS not moving");
                MountFood.Remove(0.25f);
            }
        }

        private void UpdateUnMountedState()
        {
            if (CharacterOwner != null)
            {
                if (CustomKeybindings.GetKeyDown("TEST_FOLLOWWAIT_TOGGLE_BUTTON"))
                {
                    StayPut = !StayPut;

                    if (StayPut)
                    {
                        DisplayNotification($"{MountName}, stay.");
                    }
                    else
                    {
                        DisplayNotification($"{MountName}, to me.");
                    }
                }

                if (!StayPut)
                {

                    //leash if no move target
                    if (MoveToTarget == Vector3.zero)
                    {
                        float distanceToOwner = Vector3.Distance(transform.position, CharacterOwner.transform.position);
                        if (distanceToOwner > LeashDistance)
                        {
                            if (NavMesh != null)
                            {
                                MoveTo(CharacterOwner.transform.position + (Vector3)Random.insideUnitCircle * LeashPointRadius);
                            }
                        }
                    }
                    else
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, MoveToTarget);
                        if (distanceToTarget <= TargetStopDistance)
                        {
                            //this + NavMesh.stoppingDistance stop the navmesh agent fidgeting
                            NavMesh.isStopped = true;
                        }
                        else
                        {

                            MoveTo(MoveToTarget);
                        }
                    }

                    if (CustomKeybindings.GetKeyDown("TEST_MOVETO_BUTTON"))
                    {
                        ///middle of screen into world space
                        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

                        if (Physics.Raycast(ray, out RaycastHit hit, MoveToRayCastDistance, MoveToLayerMask))
                        {
                            MoveToTarget = hit.point;
                            DisplayNotification($"{MountName}! Go!");
                        }
                    }
                }
                else
                {
                    //users can just toggle follow/wait in order to get mount following again after a MoveTo order
                    NavMesh.isStopped = true;
                    MoveToTarget = Vector3.zero;
                }

            }

            float forwardVel = Vector3.Dot(NavMesh.velocity.normalized, transform.forward);
            float sideVel = Vector3.Dot(NavMesh.velocity.normalized, transform.right);

            Animator.SetFloat("Move X", sideVel, 5f, 5f);
            Animator.SetFloat("Move Z", forwardVel, 5f, 5f);
        }

        public void DisplayNotification(string text)
        {
            if (CharacterOwner != null)
            {
                CharacterOwner.CharacterUI.ShowInfoNotification(text);
            }
        }

        public void DisplayImportantNotification(string text)
        {
            if (CharacterOwner != null)
            {
                CharacterOwner.CharacterUI.NotificationPanel.ShowNotification(text);
            }
        }

        private void MoveTo(Vector3 Position)
        {
            EnableNavMeshAgent();
            NavMesh.SetDestination(Position);
            NavMesh.isStopped = false;
        }

        public void PlayTriggerAnimation(string name)
        {
            Animator.SetTrigger(name);
        }

        private void UpdateCurrentWeight(float newWeight)
        {
            CurrentCarryWeight = newWeight;
        }

        public bool WillObey()
        {
            return MountFood.FoodAsNormalizedPercent > 0.3f;
        }

        public void EnableNavMeshAgent()
        {
            NavMesh.enabled = true;
        }

        public void DisableNavMeshAgent()
        {
            NavMesh.enabled = false;
        }
    }

    public enum MountState
    {
        UNMOUNTED,
        MOUNTED
    }
}
