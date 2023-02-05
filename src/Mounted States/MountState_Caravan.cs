using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoMount.Mounted_States
{
    public class MountState_Caravan : MountState_Unmounted
    {
        public Vector3 TargetAreaChange = Vector3.zero;

        public MountState_Caravan(Vector3 targetAreaChange)
        {
            TargetAreaChange = targetAreaChange;
        }

        public override void OnDodge(BasicMountController controller)
        {
            Parent.PopState();
        }

        public override void OnEnter(BasicMountController MountController)
        {
            base.OnEnter(MountController);
        }

        public override void OnExit(BasicMountController MountController)
        {
            base.OnExit(MountController);
        }

        public override void OnFixedUpdate(BasicMountController MountController)
        {
           
        }

        public override void OnSprint(BasicMountController controller)
        {
            
        }

        public override void OnUpdate(BasicMountController MountController)
        {
            if (TargetAreaChange != Vector3.zero)
            {
                MoveToTargetPosition(MountController, TargetAreaChange, 1f, () =>
                {
                    Parent.PopState();
                });
            }
            else
            {
                Parent.PopState();
            }
        }
    }
}
