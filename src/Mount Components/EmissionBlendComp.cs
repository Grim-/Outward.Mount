﻿using System;
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
        public SerializableColor StartColor;
        public SerializableColor EndColor;
        public float BlendTime = 15f;
        public bool ReverseOnComplete = true;

        private float Timer = 0;

        private SkinnedMeshRenderer CachedRenderer;

        private bool IsRunning = true;
        private bool ShouldReverse = false;

        public override void OnApply(BasicMountController BasicMountController)
        {
            base.OnApply(BasicMountController);

            CachedRenderer = SkinnedMesh;

            Controller.EventComp.OnDodgeDown += OnDodgeKey;
        }

        private void OnDodgeKey(BasicMountController MountController, Character obj)
        {
            IsRunning = !IsRunning;

            if (IsRunning == false)
            {
                Controller.DisableEmission();
            }
        }

        public override void Update()
        {
            base.Update();

            if (CachedRenderer && IsRunning)
            {
                if (Timer >= BlendTime)
                {
                    Timer = 0;

                    if (ReverseOnComplete) ShouldReverse = !ShouldReverse;                 
                }

                if (ShouldReverse)
                {
                    CachedRenderer.material.SetColor("_EmissionColor", Color.Lerp(EndColor, StartColor, Timer / BlendTime));
                }
                else
                {
                    CachedRenderer.material.SetColor("_EmissionColor", Color.Lerp(StartColor, EndColor, Timer / BlendTime));
                }


                Timer += Time.deltaTime;
            }     
        }

        public override bool CanRun(BasicMountController Controller)
        {
            return true;
        }
    }

    [XmlType("EmissionBlendCompProp")]
    public class EmissionBlendCompProp : MountCompProp
    {
        [XmlElement("StartColor")]
        public SerializableColor StartColor;

        [XmlElement("EndColor")]
        public SerializableColor EndColor;

        [XmlElement("BlendTime")]
        public float BlendTime;

        [XmlElement("ReverseOnComplete")]
        public bool ReverseOnComplete;
    }
}