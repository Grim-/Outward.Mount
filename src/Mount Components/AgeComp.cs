using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount.Mount_Components
{
    public class AgeComp : MountComp
    {
        public int AgeInHours
        {
            get; private set;
        }

        public float AgeModifier = 1f;

        public int AgeInDays
        {
            get
            {
                return Mathf.RoundToInt((float)AgeInHours / 24f);
            }
        }

        private int MaxGrowthAge = 48;

        private float GrowthAsPercent => (float)AgeInHours / (float)MaxGrowthAge;


        public float BabyScale = 0.4f;
        public float JuvenileScale = 0.6f;
        public float AdultScale = 1f;

        private float CurrentTargetScale => Mathf.Lerp(BabyScale, AdultScale, GrowthAsPercent);


        public override void OnApply(BasicMountController BasicMountController)
        {
            base.OnApply(BasicMountController);
            EmoMountMod.Instance.OnGameHourPassed += OnGameHourPassed;
            //TOD_Sky.Instance.TODTime.OnHour += TODTime_OnHour;
            UpdateScale();
        }

        private void TODTime_OnHour()
        {
            AgeInHours++;
            UpdateScale();
        }

        private void OnGameHourPassed(float currentHour)
        {
            AgeInHours++;
            UpdateScale();
        }

        public override void OnRemove(BasicMountController Controller)
        {
            base.OnRemove(Controller);
            //TOD_Sky.Instance.TODTime.OnHour -= TODTime_OnHour;
            EmoMountMod.Instance.OnGameHourPassed -= OnGameHourPassed;
        }

        private void UpdateScale()
        {
            Controller.transform.localScale = new Vector3(CurrentTargetScale, CurrentTargetScale, CurrentTargetScale);
        }
    }

    [XmlType("AgeCompProp")]
    public class AgeCompProp : MountCompProp
    {
        [XmlElement("AgeModifier")]
        public float AgeModifier;
        [XmlAttribute("BabyScale")]
        public float BabyScale = 0.4f;
        [XmlAttribute("JuvenileScale")]
        public float JuvenileScale = 0.6f;
        [XmlAttribute("AdultScale")]
        public float AdultScale = 1f;
    }
}
