using EmoMount.Mounted_States;
using SideLoader;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EmoMount
{
    public class MountState_Unmounted : BaseMountState
    {
        private Vector3 MoveToTarget = Vector3.zero;
        private bool StayStill = false;

        private SimpleTimer BoredomTimer;
        private SimpleTimer InteractTimer;

        private float LastVocalizeTime = 0;
        private float VocalizeCD = 30f;


        #region Detection Predicates
        public Func<TreasureChest, bool> LootPredicate =>
        (container) =>
        {
            return container.HasAnyDrops && !container.IsEmpty;
        };
        public Func<Gatherable, bool> GatherablePredicate =>
        (gatherable) =>
        {
            return gatherable.CanGather;
        };
        public Func<Item, bool> ItemPredicate =>
        (item) =>
        {
            if (item is SelfFilledItemContainer)
            {
                SelfFilledItemContainer itemContainer = item as SelfFilledItemContainer;

                return !itemContainer.IsEmpty && itemContainer.IsPickable;
            }
            else
            {
                return item.CanBePutInInventory && item.IsPickable;
            }
        };
        #endregion

        public override void OnEnter(BasicMountController MountController)
        {
            MountController.EnableNavMeshAgent();
            MountController.SetIsMounted(false);
            LastVocalizeTime = Time.time;

            BoredomTimer = new SimpleTimer(Random.Range(15f, 40f), true, OnBored);
            InteractTimer = new SimpleTimer(Random.Range(15f, 20f), true, OnInteract);
        }

        private void OnInteract()
        {
            if (!Parent.Controller.IsMoving && !StayStill)
            {
                int Rand = Random.Range(0, 5);
                switch (Rand)
                {
                    case 0:
                        if (Parent.Controller.DoDetectionType<Item>(12f, ItemPredicate, out Item Nearest))
                        {                     
                            ShowMessage($"Kupo! \r\n ({Parent.Controller.MountName} has noticed a {Nearest.DisplayName} nearby!)");
                            Parent.Controller.transform.LookAt(Nearest.transform.position);
                            Parent.Controller.PlayMountAnimation(MountAnimations.MOUNT_HAPPY);
                        }
                    break;

                    case 1:
                            Parent.Controller.PlayMountAnimation(MountAnimations.MOUNT_HAPPY);
                    break;
                }


            }
        }

        public bool CanVocalize()
        {
            return LastVocalizeTime - Time.time >= VocalizeCD;
        }

        public void ShowMessage(string message)
        {
            if (Parent.Controller.CharacterOwner)
            {
                if (CanVocalize())
                {
                    Parent.Controller.CharacterOwner.CharacterUI.NotificationPanel.ShowNotification(message);
                    LastVocalizeTime = Time.time;
                }
            }
        }

        private void OnBored()
        {
            if (!Parent.Controller.IsMoving)
            {
                Parent.Controller.PlayMountAnimation(MountAnimations.MOUNT_HAPPY);
            }
        }

        public override void OnExit(BasicMountController MountController)
        {
            MoveToTarget = Vector3.zero;
            StayStill = false;
        }

        public override void OnFixedUpdate(BasicMountController MountController)
        {

        }

        public override void OnUpdate(BasicMountController MountController)
        {
            base.OnUpdate(MountController);

            BoredomTimer.UpdateTimer(Time.deltaTime);
            InteractTimer.UpdateTimer(Time.deltaTime);

            if (CustomKeybindings.GetKeyDown(EmoMountMod.MOUNT_FOLLOW_WAIT_TOGGLE))
            {
                StayStill = !StayStill;

                if (StayStill)
                {
                    MountController.DisplayNotification($"{MountController.MountName}, stay.");
                }
                else
                {
                    MountController.DisplayNotification($"{MountController.MountName}, to me.");
                }
            }





            if (!StayStill)
            {
                if (MoveToTarget != Vector3.zero)
                {
                    MoveToTargetPosition(MountController, MoveToTarget, MountController.TargetStopDistance);
                }
                else
                {
                    if (MountController.DistanceToOwner > MountController.LeashDistance)
                    {
                        MoveToOwner(MountController);
                    }                  
                }
              
            }
            else
            {
                MountController.NavMeshAgent.isStopped = true;
                MoveToTarget = Vector3.zero;
            }
    
            CheckMoveToInput(MountController);
        }


        private void MoveToOwner(BasicMountController MountController)
        {
            Vector3 LeashPosition = MountController.CharacterOwner.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * MountController.LeashPointRadius;

            if (LeashPosition != Vector3.zero)
            {
                MoveToTargetPosition(MountController, LeashPosition,MountController.LeashDistance);
            }
        }
        protected void MoveToTargetPosition(BasicMountController MountController, Vector3 MoveToTarget, float StopDistance, Action OnTargetReached = null)
        {
            if (!MountController.NavMeshAgent.isOnNavMesh)
            {
                return;
            }

            StayStill = false;

            if (MoveToTarget != Vector3.zero)
            {
                float distanceToTarget = Vector3.Distance(MountController.transform.position, MoveToTarget);

                if (distanceToTarget <= StopDistance)
                {
                    MountController.NavMeshAgent.isStopped = true;
                    MoveToTarget = Vector3.zero;
                    if (OnTargetReached != null) OnTargetReached?.Invoke();
                }
                else
                {

                    MountController.NavMeshAgent.SetDestination(MoveToTarget);
                    MountController.NavMeshAgent.isStopped = false;
                }
            }
        }
        private void CheckMoveToInput(BasicMountController MountController)
        {
            if (CustomKeybindings.GetKeyDown(EmoMountMod.MOUNT_MOVE_TO_KEY))
            {
                ///middle of screen into world space
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

                if (Physics.Raycast(ray, out RaycastHit hit, MountController.MoveToRayCastDistance, MountController.MoveToLayerMask))
                {
                    MoveToTarget = hit.point;
                    MountController.DisplayNotification($"{MountController.MountName}! Go!");
                }
            }
        }

        //public override void UpdateAnimator(BasicMountController MountController)
        //{
        //    float forwardVel = Vector3.Dot(MountController.NavMesh.velocity.normalized, MountController.transform.forward);
        //    float sideVel = Vector3.Dot(MountController.NavMesh.velocity.normalized, MountController.transform.right);

        //    MountController.IsMoving = MountController.NavMesh.velocity != Vector3.zero;
        //    //dont even use X its just there because 
        //    MountController.Animator.SetFloat("Move X", sideVel, 5f, 5f);
        //    MountController.Animator.SetFloat("Move Z", forwardVel, 5f, 5f);
        //}
    }
}
