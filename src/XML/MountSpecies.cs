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
        public int WhistleItemID;
        public int TargetItemID = 4300130;


        public string SpeciesName;
        public string SLPackName;
        public string AssetBundleName;
        public string PrefabName;


        //movement
        public float MoveSpeed = 12f;
        //used by nav mesh agent
        public float Acceleration = 7f;
        public float RotateSpeed = 100f;

        public Vector3 CameraOffset;

        public bool IsWhistleWorldDrop = true;
        public bool GenerateRandomTint = false;
        public bool GenerateRandomEmission = false;

        [XmlArray("MountColors"), XmlArrayItem(typeof(MountColorChance), ElementName = "ColorChance")]
        public List<MountColorChance> MountColors { get; set; }

        [XmlArray("MountEmissionColors"), XmlArrayItem(typeof(MountColorChance), ElementName = "EmissionColorChance")]
        public List<MountColorChance> MountEmissionColors { get; set; }

        //Food
        public List<string> FoodTags = new List<string>();
        public List<MountFoodData> FavouriteFoods = new List<MountFoodData>();
        public List<MountFoodData> HatedFoods = new List<MountFoodData>();

        public List<string> Names = new List<string>();

        public float MaximumFood;
        public float FoodTakenPerTick;
        public float HungerTickTime;

        //weight
        public float MaximumCarryWeight;
        public float CarryWeightEncumberenceLimit;
        public float EncumberenceSpeedModifier;


        [XmlArray("MountComponents"), XmlArrayItem(typeof(MountCompProp), ElementName = "MountCompProp")]
        public List<MountCompProp> MountComponents { get; set; }

        public Color GetRandomColor()
        {
            return WeightedItem<MountColorChance>.GetWeightedRandomValueFromList(OutwardHelpers.ConvertToWeightedItemList(MountColors));
        }


        public string GetRandomName()
        {
            return Names[UnityEngine.Random.Range(0, Names.Count)];
        }
    }

    public class MountColorChance
    {
        [XmlAttribute("Chance")]
        public int Chance { get; set; }
        [XmlAttribute("Color")]
        public string Color { get; set; }
    }
}

