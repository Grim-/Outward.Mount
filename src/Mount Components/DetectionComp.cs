using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount.Mount_Components
{
    public class DetectionComp : MountComp
    {
        public float DetectionInterval = 1f;

        public float IntervalTimer = 0;

        public Color NothingDetectedColor = Color.white;
        public Color LootDetectedColor = Color.yellow;
        public float BaseIntensity = 1f;
        public float MinIntensity = 0f;
        public float MaxIntensity = 4f;
        public float DetectionRadius = 5f;
        public bool ScaleIntensityWithDistance = true;



        public Action<Item, float> OnItemDetected;
        public Action<Character, float> OnEnemyDetected;
        public Action<Gatherable, float> OnGatherableDetected;

        public Func<TreasureChest, bool> LootPredicate =>
        (container) =>
        {
            return container.HasAnyDrops && !container.IsEmpty;
        };
        public Func<Gatherable, bool> GatherablePredicate =>
        (gatherable) =>
        {
            return gatherable.CanGather;
        };
        public Func<Item, bool> ItemPredicate =>
        (item) =>
        {
            if (item is SelfFilledItemContainer)
            {
                SelfFilledItemContainer itemContainer = item as SelfFilledItemContainer;

                return !itemContainer.IsEmpty && itemContainer.IsPickable;
            }
            else
            {
                return item.CanBePutInInventory && item.IsPickable;
            }
        };



        public override void Update()
        {
            base.Update();

            IntervalTimer += Time.deltaTime;

            if (IntervalTimer >= DetectionInterval)
            {
                DoDetection();
                IntervalTimer = 0;
            }

        }
        public virtual void DoDetection()
        {
            if (Controller.DoDetectionType<Item>(DetectionRadius, ItemPredicate, out Item FoundItem))
            {
                float distance = Vector3.Distance(Controller.transform.position, FoundItem.transform.position);
                float CurrentProximityItensity = (Mathf.Clamp(1.0f - distance / DetectionRadius, MinIntensity, MaxIntensity) * BaseIntensity);
                if (ScaleIntensityWithDistance)
                {

                    if (Controller.IsFacing(FoundItem.transform))
                    {
                        Controller.SetEmissionColor(LootDetectedColor, (CurrentProximityItensity + 1.2f));
                    }
                    else
                    {
                        Controller.SetEmissionColor(LootDetectedColor, CurrentProximityItensity);
                    }

                }
                else
                {
                    Controller.SetEmissionColor(LootDetectedColor, BaseIntensity);
                }
            }
            else if (Controller.DoDetectionType<TreasureChest>(DetectionRadius, LootPredicate, out TreasureChest FoundContainer))
            {
                float distance = Vector3.Distance(Controller.transform.position, FoundItem.transform.position);
                float CurrentProximityItensity = (Mathf.Clamp(1.0f - distance / DetectionRadius, MinIntensity, MaxIntensity) * BaseIntensity);
                if (ScaleIntensityWithDistance)
                {

                    if (Controller.IsFacing(FoundItem.transform))
                    {
                        Controller.SetEmissionColor(LootDetectedColor, (CurrentProximityItensity + 1.2f));
                    }
                    else
                    {
                        Controller.SetEmissionColor(LootDetectedColor, CurrentProximityItensity);
                    }

                }
                else
                {
                    Controller.SetEmissionColor(LootDetectedColor, BaseIntensity);
                }
            }
            else
            {
                Controller.DisableEmission();
            }

        }
    }

    [XmlType("DetectionCompProp")]
    public class DetectionCompProp : MountCompProp
    {
        [XmlElement("DetectionRadius")]
        public float DetectionRadius;

        [XmlElement("BaseIntensity")]
        public float BaseIntensity;

        [XmlElement("MinIntensity")]
        public float MinIntensity;

        [XmlElement("MaxIntensity")]
        public float MaxIntensity;

        [XmlElement("DetectionInterval")]
        public float DetectionInterval;
    }
}
