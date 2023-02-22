using SideLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using EmoMount.Mount_Components;
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

        public MountEventComp EventComp
        {
            get; private set;
        }

        public NavMeshAgent NavMesh
        {
            get; private set;
        }

        public MountFood MountFood
        {
            get; private set;
        }

        //public MountSpecies MountSpecies
        //{
        //    get; private set;
        //}

        public string SpeciesName
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

        public Character CurrentlyMountedCharacter
        {
            get; private set;
        }

        public bool Initalized
        {
            get; private set;
        }


        public bool IsMounted 
        { 
            get; private set; 
        }
        public bool IsMoving 
        { 
            get; set; 
        }
        public float MountTotalWeight
        { 
            get; set; 
        }

        public MountUpInteraction MountUpInteraction 
        { 
            get; private set; 
        }
        public ShowStashMountInteraction ShowStashInteraction 
        { 
            get; private set; 
        }
        public InteractionActivator InteractionActivator 
        { 
            get; private set; 
        }
        public InteractionTriggerBase InteractionTriggerBase 
        { 
            get; private set; 
        }

        public Hitbox HitBox
        {
            get;  set;
        }

        public StackBasedStateMachine<BasicMountController> MountFSM
        {
            get; private set;
        }


        public Vector3 BaseInput => new Vector3(ControlsInput.MoveHorizontal(CharacterOwner.OwnerPlayerSys.PlayerID), 0, ControlsInput.MoveVertical(CharacterOwner.OwnerPlayerSys.PlayerID));
        public Vector3 CameraRelativeInput => Camera.main.transform.TransformDirection(BaseInput);
        public Vector3 CameraRelativeInputNoY => new Vector3(CameraRelativeInput.x, 0, CameraRelativeInput.z);
        public float DistanceToOwner => CharacterOwner != null ? Vector3.Distance(transform.position, CharacterOwner.transform.position) : 0f;
        public SkinnedMeshRenderer SkinnedMeshRenderer => GetComponentInChildren<SkinnedMeshRenderer>();
        public Color CurrentTintColor => SkinnedMeshRenderer.material.GetColor("_TintColor");
        public Color CurrentEmissionColor => SkinnedMeshRenderer.material.GetColor("_EmissionColor");

        public Transform MountPoint => transform.FindInAllChildren("SL_MOUNTPOINT");

        #endregion

        public Action OnSpawnComplete;

        public Character.SpellCastType MountAnimation = Character.SpellCastType.Sit;
        public Character.SpellCastType DismountAnimation = Character.SpellCastType.ExitBed;

        #region Speed
        public bool IsSprinting
        {
            get; set;
        }
        public float MoveSpeed { get; private set; }
        public float MoveSpeedModifier => IsSprinting ? SprintModifier : 1f;
        public float RotateSpeed { get; private set; }
        public float SprintModifier = 2f;
        public float ActualMoveSpeed
        {
            get
            {
                if (EmoMountMod.EnableWeightLimit.Value)
                {
                    return WeightAsNormalizedPercent > CarryWeightEncumberenceLimit ? (MoveSpeed * EncumberenceSpeedModifier) * MoveSpeedModifier : MoveSpeed * MoveSpeedModifier;
                }
                return MoveSpeed * MoveSpeedModifier;
            }
        }
        #endregion

        public bool IsTransform = false;
        public float LeashDistance => EmoMountMod.LeashDistance.Value;
        //A Point is randomly chosen in LeashPointRadius around player to leash to.
        public float LeashPointRadius => EmoMountMod.LeashRadius.Value;
        public float TargetStopDistance = 1.4f;
        public float MoveToRayCastDistance = 20f;
        public LayerMask MoveToLayerMask => LayerMask.GetMask("LargeTerrainEnvironment", "WorldItems");

        //weight
        public float CurrentCarryWeight = 0;
        //no idea on a reasonable number for any of this
        public float MaximumCarryWeight = 120f;
        public float CarryWeightEncumberenceLimit = 0.75f;
        public float EncumberenceSpeedModifier = 0.5f;
        public float WeightAsNormalizedPercent => CurrentCarryWeight / MaximumCarryWeight;

        public Vector3 MountedCameraOffset;
        private Vector3 OriginalPlayerCameraOffset;
        private bool IsTeleporting = false;

        public void Awake()
        {
            Initalized = false;
            Animator = GetComponent<Animator>();
            Controller = GetComponent<CharacterController>();
            NavMesh = gameObject.AddComponent<NavMeshAgent>();
            MountFood = gameObject.AddComponent<MountFood>();
            EventComp = gameObject.AddComponent<MountEventComp>();
            SetupInteractionComponents();

            gameObject.layer = LayerMask.NameToLayer("Characters");

            //needs to be done this way to avoid the jillion racetime errors
            MountFood.Init();

            NavMesh.height = 1f;
            NavMesh.stoppingDistance = 1f;
            NavMesh.enabled = false;
            MountUID = Guid.NewGuid().ToString();
            MountTotalWeight = 0;
            IsSprinting = true;
            SetupFSM();
            OnSpawnComplete?.Invoke();
            Initalized = true;
        }
        private void SetupFSM()
        {
            MountFSM = new StackBasedStateMachine<BasicMountController>(this);
            MountFSM.AddState("Base", new MountState_Unmounted());
            MountFSM.PushState("Base");
        }
        private void SetupInteractionComponents()
        {
            EmoMountMod.Log.LogMessage($"Creating Interaction Components...");
            MountUpInteraction = gameObject.AddComponent<MountUpInteraction>();
            ShowStashInteraction = gameObject.AddComponent<ShowStashMountInteraction>();
            //dismissMountInteraction = gameObject.AddComponent<StoreMountInteraction>();
            InteractionActivator = gameObject.AddComponent<InteractionActivator>();
            InteractionTriggerBase = gameObject.AddComponent<InteractionTriggerBase>();

            InteractionActivator.BasicInteraction = MountUpInteraction;
            //interactionActivator.AddBasicInteractionOverride(petMountInteraction);
            InteractionActivator.m_defaultHoldInteraction = ShowStashInteraction;
            InteractionTriggerBase.DetectionColliderRadius = 1f;
        }


        #region Setters

        public void SetOwner(Character mountTarget)
        {
            CharacterOwner = mountTarget;
        }

        public void SetMountUI(MountUI mountUI)
        {
            MountUI = mountUI;
        }


        /// <summary>
        /// Sets Movespeed, RotationSpeed, NavMeshMoveSpeed, NavMeshAcceleration and Camera Offset from the provided species definition.
        /// </summary>
        /// <param name="mountSpecies"></param>
        public void SetSpecies(MountSpecies mountSpecies)
        {
            SpeciesName = mountSpecies.SpeciesName;
            PrefabName = mountSpecies.PrefabName;
            SLPackName = mountSpecies.SLPackName;
            AssetBundleName = mountSpecies.AssetBundleName;
            SetMoveSpeed(mountSpecies.MoveSpeed);
            SetRotationSpeed(mountSpecies.RotateSpeed);
            SetNavMeshMoveSpeed(mountSpecies.MoveSpeed);
            SetNavMeshAcceleration(mountSpecies.Acceleration);
            SetCameraOffset(mountSpecies.CameraOffset);
        }

        #endregion

        #region Food
        public void SetFoodTags(List<string> foodTags)
        {
            this.MountFood.FoodTags = new List<Tag>();

            foreach (var item in foodTags)
            {
                Tag TagDef = OutwardHelpers.GetTagDefinition(item);

                if (TagDef != default(Tag))
                {
                    this.MountFood.FoodTags.Add(TagDef);
                }  
            }
        }
        public void SetFavouriteFoods(List<MountFoodData> newFavourites)
        {
            MountFood.FavouriteFoods.Clear();

            foreach (var item in newFavourites)
            {
                MountFood.FavouriteFoods.Add(item.ItemID, item.FoodValue);
            }

        }
        public void SetHatedFoods(List<MountFoodData> newHated)
        {
            MountFood.HatedFoods.Clear();

            foreach (var item in newHated)
            {
                MountFood.HatedFoods.Add(item.ItemID, item.FoodValue);
            }
        }
        #endregion

        #region Movement

        public void SetNavMeshMoveSpeed(float newSpeed)
        {
            if (NavMesh != null)
            {
                NavMesh.speed = newSpeed;
                //EmoMountMod.Log.LogMessage($"{MountName} setting MoveSpeed to {newSpeed}");
            }
        }
        public void SetNavMeshAcceleration(float acceleration)
        {
            if (NavMesh != null)
            {
                NavMesh.acceleration = acceleration;
            }
        }
        public void SetMoveSpeed(float newSpeed)
        {
            MoveSpeed = newSpeed;
            NavMesh.speed = newSpeed;
        }
        public void SetRotationSpeed(float newSpeed)
        {
            RotateSpeed = newSpeed;
            NavMesh.angularSpeed = newSpeed;
        }

        public void SetCameraOffset(Vector3 newOffset)
        {
            MountedCameraOffset = newOffset;
        }

        public void SetCharacterCameraOffset(Character _affectedCharacter, Vector3 NewOffset)
        {
            _affectedCharacter.CharacterCamera.Offset = NewOffset;
        }
        #endregion

        #region Bag & Weight
        private void UpdateCurrentWeight(float newWeight)
        {
            CurrentCarryWeight = newWeight;
        }
        /// <summary>
        /// Can the mount carry weightToCarry as well as it's own current weight
        /// </summary>
        /// <param name="weightToCarry"></param>
        /// <returns></returns>
        public bool CanCarryWeight(float weightToCarry)
        {
            return EmoMountMod.EnableWeightLimit.Value ? this.MountTotalWeight + weightToCarry < MaximumCarryWeight : true;
        }
        #endregion

        #region Render
        public void SetTintColor(Color TintColor)
        {
            if (SkinnedMeshRenderer)
            {
                SkinnedMeshRenderer.material.SetColor("_TintColor", TintColor);
            }
        }
        public void SetEmissionColor(Color newColor, float intensity = 1f)
        {
            if (SkinnedMeshRenderer != null)
            {
                SkinnedMeshRenderer.material.SetColor("_EmissionColor", newColor * intensity);
            }
        }
        public void DisableEmission()
        {
            if (SkinnedMeshRenderer != null)
            {
                SkinnedMeshRenderer.material.SetColor("_EmissionColor", Color.clear);
            }
        }
        #endregion

        #region Unity
        public void Update()
        {
            //todo move to state machine, probably stack based
            if (MountFSM != null)
            {
                MountFSM.Update();
            }
        }


        public void FixedUpdate()
        {
            if (MountFSM != null)
            {
                MountFSM.FixedUpdate();
            }
        }
        #endregion

        #region Public Methods

        public bool CanMount(Character character)
        {
            if (MountFood.FoodAsNormalizedPercent < 0.3f && EmoMountMod.EnableFoodNeed.Value)
            {
                DisplayNotification($"{MountName} is too hungry!");
                return false;
            }

            if (!CanCarryWeight(character.Inventory.TotalWeight) && EmoMountMod.EnableWeightLimit.Value)
            {
                DisplayNotification($"You are carrying too much weight to mount {MountName}");
                return false;
            }

            return true;
        }
        public void MountCharacter(Character _affectedCharacter)
        {
            CurrentlyMountedCharacter = _affectedCharacter;
            //HitBox.OwnerChar = CurrentlyMountedCharacter;
            _affectedCharacter.CharacterController.enabled = false;
            _affectedCharacter.CharacterControl.enabled = false;
            //cancel movement in animator

            if (IsTransform)
            {
                //character.SpellCastAnim(TransformAnimation, Character.SpellCastModifier.Mobile, 0);
            }
            else
            {
                _affectedCharacter.SetAnimMove(0, 0);
                _affectedCharacter.SpellCastAnim(MountAnimation, Character.SpellCastModifier.Immobilized, 1);
            }

            TryToParent(_affectedCharacter, gameObject);
            OriginalPlayerCameraOffset = _affectedCharacter.CharacterCamera.Offset;

            SetCharacterCameraOffset(_affectedCharacter, OriginalPlayerCameraOffset + MountedCameraOffset);
            DisableNavMeshAgent();
            UpdateCurrentWeight(_affectedCharacter.Inventory.TotalWeight);

            MountFSM.PushDynamicState(new MountState_Mounted(CurrentlyMountedCharacter));

            EventComp.OnMounted?.Invoke(this, CurrentlyMountedCharacter);

            if (CurrentlyMountedCharacter != null)
            {
                CurrentlyMountedCharacter.GetComponent<CharacterMount>().SetIsMounted(true);
            }


            
        }
        public void DismountCharacter(Character _affectedCharacter)
        {
            //HitBox.OwnerChar = null;
            _affectedCharacter.CharacterController.enabled = true;
            _affectedCharacter.CharacterControl.enabled = true;
            _affectedCharacter.Animator.Update(Time.deltaTime);

            UpdateCurrentWeight(0);

            _affectedCharacter.transform.parent = null;
            _affectedCharacter.transform.position = transform.position;
            _affectedCharacter.transform.eulerAngles = transform.eulerAngles;


            if (IsTransform)
            {
                if (BaseInput.z != 0 || BaseInput.x != 0)
                {
                    _affectedCharacter.SpellCastAnim(Character.SpellCastType.EvasionShoot, Character.SpellCastModifier.Immobilized, 1);
                }
                else
                {
                    _affectedCharacter.SpellCastAnim(Character.SpellCastType.IdleAlternate, Character.SpellCastModifier.Immobilized, 1);
                }
                
            }
            else
            {
                _affectedCharacter.SetAnimMove(0, 1);
                _affectedCharacter.SpellCastAnim(DismountAnimation, Character.SpellCastModifier.Mobile, 1);
            }

            SetCharacterCameraOffset(_affectedCharacter, OriginalPlayerCameraOffset);
            EventComp.OnUnMounted?.Invoke(this, CurrentlyMountedCharacter);
            CurrentlyMountedCharacter.GetComponent<CharacterMount>().SetIsMounted(false);
            CurrentlyMountedCharacter = null;
            MountFSM.PopState();
        }

        public void SetIsMounted(bool isMounted)
        {
            IsMounted = isMounted;
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

        public void EnableNavMeshAgent()
        {
            //dont turn the nav mesh on at all if this is a transform, it messes with positions too much when summoning/unsummoning
            if (!IsTransform)
            {
                NavMesh.enabled = true;
                NavMesh.isStopped = false;
            }
        }
        public void DisableNavMeshAgent()
        {
            NavMesh.enabled = false;
            NavMesh.isStopped = true;
        }
        public void Teleport(Vector3 Position, Vector3 Rotation, Action OnTeleported = null)
        {
            if (!IsTeleporting)
            {
                StartCoroutine(DelayTeleport(Position, Rotation, OnTeleported));
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(DelayTeleport(Position, Rotation, OnTeleported));
            }
           
        }

        public void TeleportToOwner()
        {
            if (CharacterOwner != null)
            {
                EmoMountMod.Log.LogMessage($"Teleporting {MountName} to Owner");
                Teleport(CharacterOwner.transform.position, CharacterOwner.transform.eulerAngles);
            }
        }

        private IEnumerator DelayTeleport(Vector3 Position, Vector3 Rotation, Action OnTeleported = null)
        {     
            IsTeleporting = true;
            EmoMountMod.Log.LogMessage($"Teleporting {MountName} to {Position} {Rotation}");

            if (NavMesh.enabled)
            {
                NavMesh.Warp(Position);
            }
            else
            {
                transform.position = Position;
                transform.rotation = Quaternion.Euler(Rotation);
            }
                    
            OnTeleported?.Invoke();
            IsTeleporting = false;
            yield break;
        }

        #endregion


        public MountInstanceData CreateInstanceData()
        {
            MountInstanceData mountInstanceData = new MountInstanceData();
            mountInstanceData.MountName = MountName;
            mountInstanceData.MountUID = MountUID;
            mountInstanceData.MountSpecies = SpeciesName;
            mountInstanceData.CurrentFood = MountFood.CurrentFood;
            mountInstanceData.MaximumFood = MountFood.MaximumFood;


            AgeComp AgeComp = GetComponent<AgeComp>();
            if (AgeComp)
            {
                mountInstanceData.AgeInSeconds = AgeComp.AgeInHours;
            }

            mountInstanceData.Position = transform.position;
            mountInstanceData.Rotation = transform.eulerAngles;
            mountInstanceData.TintColor = CurrentTintColor;
            mountInstanceData.EmissionColor = CurrentEmissionColor;
            return mountInstanceData;
        }

        public void EnableCharacterRenderers(Character Character)
        {
            foreach (var item in Character.Visuals.ActiveVisualsHead.Renderers)
            {
                item.enabled = true;
            }

            foreach (var item in Character.Visuals.ActiveVisualsBody.Renderers)
            {
                item.enabled = true;
            }

            foreach (var item in Character.Visuals.ActiveVisualsFoot.Renderers)
            {
                item.enabled = true;
            }
        }

        public void DisableCharacterRenderers(Character Character)
        {
            foreach (var item in Character.Visuals.ActiveVisualsHead.Renderers)
            {
                item.enabled = false;
            }

            foreach (var item in Character.Visuals.ActiveVisualsBody.Renderers)
            {
                item.enabled = false;
            }

            foreach (var item in Character.Visuals.ActiveVisualsFoot.Renderers)
            {
                item.enabled = false;
            }
        }

        private void TryToParent(Character _affectedCharacter, GameObject MountInstance)
        {
            //probably insanely inefficient, or uses some bizzare form of windings to find the transform, who knows with extension methods :shrug:
            //Transform mountPointTransform = transform.FindInAllChildren("SL_MOUNTPOINT");

            if (MountPoint != null)
            {
                _affectedCharacter.transform.parent = MountPoint;
                _affectedCharacter.transform.localPosition = Vector3.zero;
                _affectedCharacter.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                _affectedCharacter.transform.parent = MountInstance.transform;
                _affectedCharacter.transform.localPosition = Vector3.zero;
                _affectedCharacter.transform.localEulerAngles = Vector3.zero;

                EmoMountMod.Log.LogMessage("Could not find SL_MOUNTPOINT Transform");
            }
        }


        public bool DoDetectionType<T>(Collider[] colliders, Func<T, bool> Condition, out T Nearest, Action<T, float> OnDetected = null) where T : Component
        {
            if (ColliderHasComponent<T>(colliders))
            {
                List<T> foundType = FindComponentsInColliders<T>(colliders, Condition);

                //sort the list by distance
                foundType.Sort((x, y) => { return (Controller.transform.position - x.transform.position).sqrMagnitude.CompareTo((Controller.transform.position - y.transform.position).sqrMagnitude); });

                if (foundType.Count > 0)
                {
                    float distance = Vector3.Distance(Controller.transform.position, foundType[0].transform.position);
                    OnDetected?.Invoke(foundType[0], distance);
                    Nearest = foundType[0];
                    return true;
                }
                else
                {
                    DisableEmission();
                    Nearest = null;
                    return false;
                }
            }

            Nearest = null;
            return false;
        }
        public bool DoDetectionType<T>(float DetectionRadius, Func<T, bool> Condition, out T Nearest, Action<T, float> OnDetected = null) where T : Component
        {        
            return DoDetectionType<T>(GetCollidersInRadius(DetectionRadius),  Condition, out Nearest, OnDetected);
        }
        //Returns true if the angle difference between the two Controllers forward and the targets direction is less than MexAngleDiff
        public bool IsFacing(Transform targetTransform, float MaxAngleDiff = 10f)
        {
            return Vector3.Angle(Controller.transform.forward, targetTransform.position - Controller.transform.position) <= MaxAngleDiff;
        }
        public Collider[] GetCollidersInRadius(float DetectionRadius)
        {
            return Physics.OverlapSphere(Controller.transform.position, DetectionRadius, LayerMask.GetMask("Characters", "WorldItem"));
        }
        public List<T> FindComponentsInColliders<T>(Collider[] colliders, Func<T, bool> Condition = null) where T : Component
        {
            List<T> foundList = new List<T>();

            foreach (var col in colliders)
            {

                if (col.gameObject == Controller.gameObject || col.transform.root.name == Controller.transform.root.name)
                {
                    continue;
                }

                if (col.transform.root.name == CharacterOwner.transform.root.name)
                {
                    continue;
                }

                T foundThing = col.GetComponentInChildren<T>();

                if (foundThing == null)
                {
                    foundThing = col.GetComponentInParent<T>();
                }

                if (foundThing != null)
                {
                    if (Condition == null)
                    {
                        foundList.Add(foundThing);
                    }
                    else
                    {
                        if (Condition.Invoke(foundThing))
                        {
                            foundList.Add(foundThing);
                        }
                    }

                }
            }

            return foundList;
        }
        public bool ColliderHasComponent<T>(Collider[] colliders)
        {
            foreach (var col in colliders)
            {
                //Skip anything parented
                if (col.transform.root.name == Controller.transform.root.name)
                {
                    continue;
                }

                //skip the gameObject itself
                if (col.gameObject == Controller.gameObject)
                {
                    continue;
                }

                if (col.GetComponentInChildren<T>() != null)
                {
                    return true;
                }
                else if (col.GetComponentInParent<T>() != null)
                {
                    return true;
                }
            }

            return false;
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
