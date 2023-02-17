using System;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount.Mount_Components
{
    public class LightableComp : MountComp
    {

        public Vector3 Position = Vector3.up;
        public SerializableColor Color;
        public float Intensity = 1;
        public float Range = 4f;


        private GameObject LightGO = null;
        private Light Light;

        public override void OnApply(BasicMountController BasicMountController)
        {
            base.OnApply(BasicMountController);

            if (LightGO == null)
            {
                LightGO = new GameObject("Light");
                LightGO.transform.parent = transform;
                LightGO.transform.localPosition = Position;

                Light = LightGO.AddComponent<Light>();
                Light.type = LightType.Point;

                Light.intensity = Intensity;
                Light.color = Color != null ? (Color)Color : UnityEngine.Color.white;
                Light.range = Range;
            }


            Controller.EventComp.OnDodgeDown += OnDodgeKey;
        }

        private void OnDodgeKey(BasicMountController MountController, Character obj)
        {
            if (Light)
            {
                Light.enabled = !Light.enabled;
            }
        }

        public override void OnRemove(BasicMountController Controller)
        {
            base.OnRemove(Controller);

            if (LightGO)
            {
                Destroy(LightGO);
                LightGO = null;
            }
        }

    }

    [XmlType("LightableCompProp")]
    public class LightableCompProp : MountCompProp
    {
        [XmlElement("Position")]
        public Vector3 Position = Vector3.up;
        [XmlElement("Color")]
        public SerializableColor Color;
        [XmlElement("Intensity")]
        public float Intensity = 1;
        [XmlElement("Range")]
        public float Range = 4f;
    }
}
