using System;
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

        public override void Update()
        {
            if (Controller.IsMounted)
            {
                if (ControlsInput.Sprint(Controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    Controller.MoveSpeedModifier = Mathf.MoveTowards(Controller.MoveSpeedModifier, SprintModifier, 0.5f);
                }
                else
                {
                    Controller.MoveSpeedModifier = 1f;
                }

                if (ControlsInput.DodgeButtonDown(Controller.CharacterOwner.OwnerPlayerSys.PlayerID))
                {
                    Controller.transform.position += new UnityEngine.Vector3(0, 2.5f, 0);
                }
            }

            
        }
    }

    [XmlType("GlideCompProp")]
    public class GlideCompProp : MountCompProp
    {
        [XmlElement("SprintModifier")]
        public float SprintModifier;
    }
}
