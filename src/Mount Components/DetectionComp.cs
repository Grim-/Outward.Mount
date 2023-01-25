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

        private bool DoDetectionType<T>(Collider[] colliders, Color DetectionColor, bool ScaleIntensityWithDistance, Func<T, bool> Condition) where T : Component
        {
            if (ColliderHasComponent<T>(colliders))
            {
                List<T> foundType = FindComponentsInColliders<T>(colliders, Condition);

                //sort the list by distance
                foundType.Sort((x, y) => { return (Controller.transform.position - x.transform.position).sqrMagnitude.CompareTo((Controller.transform.position - y.transform.position).sqrMagnitude); });

                if (foundType.Count > 0)
                {
                    float distance = Vector3.Distance(Controller.transform.position, foundType[0].transform.position);
                    float CurrentProximityItensity = (Mathf.Clamp(1.0f - distance / DetectionRadius, MinIntensity, MaxIntensity) * BaseIntensity);
                    if (ScaleIntensityWithDistance)
                    {

                        if (IsPlayerFacing(foundType[0].transform))
                        {
                            SetEmissionColor(DetectionColor, (CurrentProximityItensity + 1.2f));
                        }
                        else
                        {
                            SetEmissionColor(DetectionColor, CurrentProximityItensity);
                        }

                    }
                    else
                    {
                        SetEmissionColor(DetectionColor, BaseIntensity);
                    }


                    return true;
                }
                else
                {
                    DisableEmission();
                    return false;
                }
            }

            return false;
        }

        public virtual void DoDetection()
        {
            Collider[] colliders = Physics.OverlapSphere(Controller.transform.position, DetectionRadius, LayerMask.GetMask("Characters", "WorldItem"));

            if (DoDetectionType<Item>(colliders, LootDetectedColor, true, ItemPredicate))
            {

            }
            else if (DoDetectionType<TreasureChest>(colliders, LootDetectedColor, true, LootPredicate))
            {

            }
            else
            {
                DisableEmission();
            }

        }

        private List<T> FindComponentsInColliders<T>(Collider[] colliders, Func<T, bool> Condition = null) where T : Component
        {
            List<T> foundList = new List<T>();

            foreach (var col in colliders)
            {

                if (col.gameObject == Controller.gameObject  || col.transform.root.name == Controller.transform.root.name)
                {
                    continue;
                }

                if (col.transform.root.name == Controller.CharacterOwner.transform.root.name)
                {
                    continue;
                }

                T foundThing = col.GetComponentInChildren<T>();

                if (foundThing == null)
                {
                    foundThing = col.GetComponentInParent<T>();
                }

                if (foundThing != null)
                {
                    if (Condition == null)
                    {
                        foundList.Add(foundThing);
                    }
                    else
                    {
                        if (Condition.Invoke(foundThing))
                        {
                            foundList.Add(foundThing);
                        }
                    }

                }
            }

            return foundList;
        }

        private bool ColliderHasComponent<T>(Collider[] colliders)
        {
            foreach (var col in colliders)
            {
                //Skip anything parented
                if (col.transform.root.name == Controller.transform.root.name)
                {
                    continue;
                }

                //skip the gameObject itself
                if (col.gameObject == Controller.gameObject)
                {
                    continue;
                }

                if (col.GetComponentInChildren<T>() != null)
                {
                    return true;
                }
                else if (col.GetComponentInParent<T>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsPlayerFacing(Transform targetTransform)
        {
            return Vector3.Angle(Controller.transform.forward, targetTransform.position - Controller.transform.position) < 10f;
        }

        /// <summary>
        /// Use RGB 0-1 and pass an intensity value to make brigher
        /// </summary>
        /// <param name="newColor"></param>
        /// <param name="intensity"></param>
        public void SetEmissionColor(Color newColor, float intensity = 1f)
        {
            if (SkinnedMesh != null)
            {
                SkinnedMesh.material.SetColor("_EmissionColor", newColor * intensity);
            }
        }

        /// <summary>
        /// Sets the Emission color to a clear color effectively disabling it.
        /// </summary>
        public void DisableEmission()
        {
            if (SkinnedMesh != null)
            {
                SkinnedMesh.material.SetColor("_EmissionColor", NothingDetectedColor);
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
