using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount.Mount_Components
{
    public class SprintComp : MountComp
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
            }
        }
    }

    [XmlType("SprintCompProp")]
    public class SprintCompProp : MountCompProp
    {
        [XmlElement("SprintModifier")]
        public float SprintModifier;
    }
}
