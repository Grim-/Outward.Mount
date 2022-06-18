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
        public float ActualMoveSpeed => WeightAsNormalizedPercent > WeightEncumberenceLimit ? MoveSpeed * EncumberenceSpeedModifier : MoveSpeed;
        public float RotateSpeed { get; private set; }
        public float LeashDistance = 6f;
        //A Point is randomly chosen in LeashPointRadius around player to leash to.
        public float LeashPointRadius = 2.3f;
        public float TargetStopDistance = 1.4f;
        public float MoveToRayCastDistance = 20f;
        public LayerMask MoveToLayerMask => LayerMask.GetMask("LargeTerrainEnvironment", "WorldItems");

        //weight
        public float CurrentCarryWeight = 0;
        //no idea on a reasonable number for any of this
        public float MaxCarryWeight = 90f;
        public float WeightEncumberenceLimit = 0.75f;
        public float EncumberenceSpeedModifier = 0.5f;
        public float WeightAsNormalizedPercent => CurrentCarryWeight / MaxCarryWeight;


        private Vector3 OriginalPlayerCameraOffset;
        private Vector3 MountedCameraOffset = new Vector3(0, 0, -4);


        //how often is food taken while mounted
        private float MountedHungerTickTime = 30f;
        private float MountedHungerTimer = 0;




        private string[] Actions = new string[]
        {
                "DoMountSpecial",
                "DoMountAttack",
                "HitReact"
        };


        public StackBasedStateMachine<BasicMountController> MountFSM
        {
            get; private set;
        }


        public float MountTotalWeight => BagContainer != null && BagContainer.ParentContainer != null ? BagContainer.ParentContainer.TotalContentWeight : 0;

        //Input - tidy up doesnt need this many calls or new v3s
        public Vector3 BaseInput => new Vector3(ControlsInput.MoveHorizontal(CharacterOwner.OwnerPlayerSys.PlayerID), 0, ControlsInput.MoveVertical(CharacterOwner.OwnerPlayerSys.PlayerID));
        public Vector3 CameraRelativeInput => Camera.main.transform.TransformDirection(BaseInput);
        public Vector3 CameraRelativeInputNoY => new Vector3(CameraRelativeInput.x, 0, CameraRelativeInput.z);
        public float DistanceToOwner => CharacterOwner != null ? Vector3.Distance(transform.position, CharacterOwner.transform.position) : 0f;

        public void Awake()
        {
            Animator = GetComponent<Animator>();
            Controller = GetComponent<CharacterController>();
            NavMesh = gameObject.AddComponent<NavMeshAgent>();
            MountFood = gameObject.AddComponent<Emo_MountFood>();
            MountFood.Init();

            NavMesh.stoppingDistance = 1f;
            NavMesh.enabled = false;
            MountUID = Guid.NewGuid().ToString();
            SetupFSM();
        }

        /// <summary>
        /// moved state stuff into a state machine so its cleaner
        /// </summary>
        private void SetupFSM()
        {
            MountFSM = new StackBasedStateMachine<BasicMountController>(this);
            MountFSM.AddState("Base", new UnMountedState());
            MountFSM.PushState("Base");
        }

        public void SetOwner(Character mountTarget)
        {
            CharacterOwner = mountTarget;
        }

        public void SetInventory(Item bag)
        {
            BagContainer = bag;
        
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
            if (EmoMountMod.Debug)
            {
                EmoMountMod.Log.LogMessage($"Setting up Bag For {MountName} uid: {MountUID}");
            }


            ///lol seriously
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;


            Transform mountPointTransform = transform.FindInAllChildren("SL_BAGPOINT");
            Transform ItemHighlight = bag.gameObject.transform.FindInAllChildren("ItemHighlight");

            if (ItemHighlight)
            {
                if (EmoMountMod.Debug) EmoMountMod.Log.LogMessage($"Item Highlight found, disabling");
                

                ItemHighlight.gameObject.SetActive(false);
            }

            if (mountPointTransform != null)
            {
                BagContainer.transform.parent = mountPointTransform;
            }
            else
            {
                BagContainer.transform.parent = transform;
            }

            Bag itemBag = bag as Bag;


            if (EmoMountMod.Debug) EmoMountMod.Log.LogMessage($"Updating Bag Position");
            
            ///default bag position move later
            bag.transform.localPosition = new Vector3(-0.0291f, 0.11f, -0.13f);
            bag.transform.localEulerAngles = new Vector3(2.3891f, 358.9489f, 285.6735f);

            if (bag.m_rigidBody)
            {
                bag.m_rigidBody.isKinematic = true;
                bag.m_rigidBody.useGravity = false;
                bag.m_rigidBody.freezeRotation = true;
            }

            yield break;
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

            //todo move to state machine, probably stack based
            if (MountFSM != null)
            {
                MountFSM.Update();
            }

            ///Bag takes way too long to instantiate and set up (multiple frames) resulting in IsKinematic being disabled by the RigidbodySuspender, so for now, force the settings.
            if (BagContainer)
            {
                BagContainer.transform.localPosition = new Vector3(-0.0291f, 0.11f, -0.13f);
                BagContainer.transform.localEulerAngles = new Vector3(2.3891f, 358.9489f, 285.6735f);

                if (BagContainer.m_rigidBody)
                {
                    BagContainer.m_rigidBody.isKinematic = true;
                    BagContainer.m_rigidBody.useGravity = false;
                    BagContainer.m_rigidBody.freezeRotation = true;
                }
            }
        }

        public void FixedUpdate()
        {
            if (MountFSM != null)
            {
                //never likely to be used but you never know..
                MountFSM.FixedUpdate();
            }
        }

        public void MountCharacter(Character _affectedCharacter)
        {
            PrepareCharacter(_affectedCharacter);
            DisableNavMeshAgent();
            UpdateCurrentWeight(_affectedCharacter.Inventory.TotalWeight);
            MountFSM.PushDynamicState(new BaseMountedState(_affectedCharacter));
        }
        private void PrepareCharacter(Character character)
        {
            character.CharMoveBlockCollider.enabled = false;
            character.CharacterController.enabled = false;
            character.CharacterControl.enabled = false;
            //cancel movement in animator
            character.SetAnimMove(0, 0);
            character.SpellCastAnim(Character.SpellCastType.Sit, Character.SpellCastModifier.Immobilized, 1);
            TryToParent(character, gameObject);
            OriginalPlayerCameraOffset = character.CharacterCamera.Offset;
            SetCharacterCameraOffset(character, OriginalPlayerCameraOffset + MountedCameraOffset);
        }


        public bool CanMount(Character character)
        {
            if (MountFood.FoodAsNormalizedPercent < 0.3f)
            {
                DisplayNotification($"{MountName} is too hungry!");
                return false;
            }

            if (!CanCarryWeight(character.Inventory.TotalWeight))
            {
                DisplayNotification($"You are carrying too much weight to mount {MountName}");
                return false;
            }

            return true;
        }
        

        /// <summary>
        /// Can the mount carry weightToCarry as well as it's own current weight
        /// </summary>
        /// <param name="weightToCarry"></param>
        /// <returns></returns>
        public bool CanCarryWeight(float weightToCarry)
        {
            return this.MountTotalWeight + weightToCarry < MaxCarryWeight;
        }

        public void DismountCharacter(Character _affectedCharacter)
        {
            _affectedCharacter.enabled = true;
            _affectedCharacter.CharMoveBlockCollider.enabled = true;
            _affectedCharacter.CharacterController.enabled = true;
            _affectedCharacter.CharacterControl.enabled = true;
            _affectedCharacter.Animator.enabled = true;
            _affectedCharacter.Animator.Update(Time.deltaTime);

            _affectedCharacter.transform.parent = null;
            _affectedCharacter.transform.position = transform.position;
            _affectedCharacter.transform.eulerAngles = Vector3.zero;

            SetCharacterCameraOffset(_affectedCharacter, OriginalPlayerCameraOffset);
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

        public void AddItemToBag(Item item)
        {
            if (BagContainer != null)
            {
                BagContainer.ParentContainer.AddItem(item);
                //GameObject.Destroy(item.gameObject);
            }
            else
            {
                //has no bag
            }
        }

        public void PlayTriggerAnimation(string name)
        {
            Animator.SetTrigger(name);
        }


        public void PlayMountAnimation(MountAnimations animation)
        {
            switch (animation)
            {
                case MountAnimations.MOUNT_HAPPY:
                    PlayTriggerAnimation("DoMountHappy");
                    break;
                case MountAnimations.MOUNT_ANGRY:
                    PlayTriggerAnimation("DoMountAngry");
                    break;
                case MountAnimations.MOUNT_SPECIAL:
                    PlayTriggerAnimation("DoMountSpecial");
                    break;
                case MountAnimations.MOUNT_ATTACK:
                    PlayTriggerAnimation("DoMountAttack");
                    break;
                case MountAnimations.MOUNT_HITREACT:
                    PlayTriggerAnimation("DoMountHitReact");
                    break;
            }
        }

        private void UpdateCurrentWeight(float newWeight)
        {
            CurrentCarryWeight = newWeight;
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

    public enum MountAnimations
    {
        MOUNT_HAPPY,
        MOUNT_ANGRY,
        MOUNT_SPECIAL,
        MOUNT_ATTACK,
        MOUNT_HITREACT
    }
}
