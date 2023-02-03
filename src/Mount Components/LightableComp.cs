using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount.Mount_Components
{
    public class LightableComp : MountComp
    {

        public Vector3 Position = Vector3.zero;
        public Color Color = Color.white;
        public float Intensity = 1;
        public float Range = 4f;


        private GameObject LightGO = null;

        public override void OnApply(BasicMountController BasicMountController)
        {
            base.OnApply(BasicMountController);

            if (LightGO == null)
            {
                LightGO = new GameObject("Light");
                LightGO.transform.parent = transform;
                LightGO.transform.localPosition = Position;

                Light Light = LightGO.AddComponent<Light>();
                Light.type = LightType.Point;

                Light.intensity = Intensity;
                Light.color = Color;
                Light.range = Range;
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
        public Vector3 Position = Vector3.zero;
        [XmlElement("Color")]
        public Color Color = Color.white;
        [XmlElement("Intensity")]
        public float Intensity = 1;
        [XmlElement("Range")]
        public float Range = 4f;
    }
}
