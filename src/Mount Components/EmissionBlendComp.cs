using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount.Mount_Components
{
    public class EmissionBlendComp : MountComp
    {
        public Color StartColor;
        public Color EndColor;
        public float BlendTime = 15f;

        private float Timer = 0;

        private SkinnedMeshRenderer CachedRenderer;

        public override void OnApply(BasicMountController BasicMountController)
        {
            base.OnApply(BasicMountController);

            CachedRenderer = SkinnedMesh;
        }

        public override void Update()
        {
            base.Update();

            if (CachedRenderer)
            {
                if (Timer >= BlendTime)
                {
                    Timer = 0;
                }

                CachedRenderer.material.SetColor("_EmissionColor", Color.Lerp(StartColor, EndColor, Timer / BlendTime));

                Timer += Time.deltaTime;
            }     
        }
    }

    [XmlType("EmissionBlendCompProp")]
    public class EmissionBlendCompProp : MountCompProp
    {
        [XmlElement("StartColor")]
        public Color StartColor { get; set; }

        [XmlElement("EndColor")]
        public Color EndColor { get; set; }

        [XmlElement("BlendTime")]
        public float BlendTime { get; set; }
    }
}
