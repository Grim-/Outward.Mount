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
        public Color StartColor;

        [XmlElement("EndColor")]
        public Color EndColor;

        [XmlElement("BlendTime")]
        public float BlendTime;
    }
}


//[Serializable]
//public struct SerializableColor
//{
//    public float R;
//    public float G;
//    public float B;
//    public float A;

//    public SerializableColor(float r, float g, float b, float a)
//    {
//        R = r;
//        G = g;
//        B = b;
//        A = a;
//    }

//    public static implicit operator Color(SerializableColor sc)
//    {
//        return new Color(sc.R, sc.G, sc.B, sc.A);
//    }

//    public static implicit operator SerializableColor(Color c)
//    {
//        return new SerializableColor(c.r, c.g, c.b, c.a);
//    }
//}