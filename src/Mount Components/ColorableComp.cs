using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount.Mount_Components
{
    public class ColorableComp : MountComp
    {
        public SerializableColor TintColor = Color.clear;
        public SerializableColor EmissionColor = Color.clear;

        public bool RemoveOnChange = true;

        public override void OnApply(BasicMountController BasicMountController)
        {
            base.OnApply(BasicMountController);

            if (TintColor != Color.clear)
            {
                BasicMountController.SetTintColor(TintColor);
            }

            if (EmissionColor != Color.clear)
            {
                BasicMountController.SetEmissionColor(EmissionColor);
            }


            if(RemoveOnChange) Destroy(this);
        }
    }

    [XmlType("ColorableCompProp")]
    public class ColorableCompProp : MountCompProp
    {
        [XmlElement("TintColor")]
        public SerializableColor TintColor;

        [XmlElement("EmissionTintColor")]
        public SerializableColor EmissionColor;
    }
}
