using EmoMount.Mount_Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount
{
    public class MountSpecies
    {
        //Item
        public int WhistleItemID;
        public int TargetItemID = 4300130;
        public bool IsWhistleWorldDrop = true;

        //Species info
        public string SpeciesName;
        public string SLPackName;
        public string AssetBundleName;
        public string PrefabName;

        public List<string> Names = new List<string>();
        public Vector3 CameraOffset = new Vector3(0, 0, -2);

        //movement
        public float MoveSpeed = 12f;
        public float SprintModifier = 2f;
        //used by nav mesh agent
        public float Acceleration = 7f;
        public float RotateSpeed = 100f;

        //weight
        public float MaximumCarryWeight;
        public float CarryWeightEncumberenceLimit;
        public float EncumberenceSpeedModifier;

        //Food
        public bool RequiresFood = true;
        public float MaximumFood = 150f;
        //Passive Food Loss
        public float PassiveFoodLoss = 10f;
        public float PassiveFoodLossTickTime = 200f;
        //Travel distance food loss
        public float TravelDistanceThreshold = 100f;
        public float FoodLostPerTravelDistance = 15f;

        public List<string> FoodTags = new List<string>();
        public List<MountFoodData> FavouriteFoods = new List<MountFoodData>();
        public List<MountFoodData> HatedFoods = new List<MountFoodData>();

        //colors
        public bool GenerateRandomTint = false;
        public bool GenerateRandomEmission = false;

        [XmlArray("MountColors"), XmlArrayItem(typeof(MountColorChance), ElementName = "ColorChance")]
        public List<MountColorChance> MountColors { get; set; }

        [XmlArray("MountEmissionColors"), XmlArrayItem(typeof(MountColorChance), ElementName = "EmissionColorChance")]
        public List<MountColorChance> MountEmissionColors { get; set; }

        //components
        [XmlArray("MountComponents"), XmlArrayItem(typeof(MountCompProp), ElementName = "MountCompProp")]
        public List<MountCompProp> MountComponents { get; set; }

        #region Instance Methods
        public Color GetRandomColor()
        {
            return WeightedItem<MountColorChance>.GetWeightedRandomValueFromList(OutwardHelpers.ConvertToWeightedItemList(MountColors));
        }

        public string GetRandomName()
        {
            return Names[UnityEngine.Random.Range(0, Names.Count)];
        }
        #endregion
    }

    public class MountColorChance
    {
        [XmlAttribute("Chance")]
        public int Chance { get; set; }
        [XmlElement("Color")]
        public SerializableColor Color { get; set; }
    }
}

