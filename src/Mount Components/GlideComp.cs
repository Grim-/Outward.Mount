using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount.Mount_Components
{
    public class GlideComp : MountComp
    {
        public float SprintModifier = 2f;

        private bool IsGlidingUp = false;

        public override void Update()
        {
            if (Controller.IsMounted)
            {
                if (ControlsInput.DodgeButtonDown(Controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    if (!IsGlidingUp)
                    {
                        StartCoroutine(GentlePushUp(Controller.transform, 0.4f));
                    }

                }
            }          
        }

        private IEnumerator GentlePushUp(Transform Transform, float GlideUpTime = 0.2f)
        {
            IsGlidingUp = true;

            float Timer = 0;
            Vector3 TargetPos = Transform.position + new Vector3(0, 2f, 0.1f);


            while (Timer < GlideUpTime)
            {
                Transform.position = Vector3.MoveTowards(Transform.position, TargetPos, 0.4f);
                Timer += Time.deltaTime;
                yield return null;
            }


            IsGlidingUp = false;
            yield break;
        }
    }


    [XmlType("GlideCompProp")]
    public class GlideCompProp : MountCompProp
    {
        [XmlElement("SprintModifier")]
        public float SprintModifier;
    }
}
