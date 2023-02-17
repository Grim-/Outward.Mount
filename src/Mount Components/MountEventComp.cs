using EmoMount.Mount_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoMount
{
    public class MountEventComp : MountComp
    {
        public Action<BasicMountController, Character> OnMounted;
        public Action<BasicMountController, Character> OnUnMounted;

        public float InRangeDistance = 4f;

        public Action<Character> OnOwnerInRange;
        public Action<Character> OnOwnerOutOfRange;

        public Action<Character> OnOwnerTakeDamage;
        public Action<Character> OnOwnerSleep;

        public Action<BasicMountController, Character> OnInteract;
        public Action<BasicMountController, Character> OnDodgeDown;
        public Action<BasicMountController, Character> OnSprintHeld;

        public Action<BasicMountController, Character> OnLeftClick;
        public Action<BasicMountController, Character> OnShiftLeftClick;
        public Action<BasicMountController, Character> OnRightClick;
        public Action<BasicMountController, Character> OnRightClickHeld;


        public Action<BasicMountController, Character> OnCrouch;


        private float RangeCheckUpdateTime = 5f;
        private float RangeCheckTimer = 0;



        public override void Update()
        {
            base.Update();

            if (RangeCheckTimer >= RangeCheckUpdateTime)
            {
                UpdateDistanceToOwner();
                RangeCheckTimer = 0;
            }
        }

        private void UpdateDistanceToOwner()
        {
            if (Controller.CharacterOwner)
            { 
                float DistanceFrom = Vector3.Distance(transform.position, Controller.CharacterOwner.transform.position);

                if (DistanceFrom <= InRangeDistance)
                {
                    OnOwnerInRange?.Invoke(Controller.CharacterOwner);
                }
                else
                {
                    OnOwnerOutOfRange?.Invoke(Controller.CharacterOwner);
                }
            }
        }
    }
}
