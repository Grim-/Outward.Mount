using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount
{
    [System.Serializable]
    public class MountSpecies
    {
        public string SpeciesName;
        public string SLPackName;
        public string AssetBundleName;
        public string PrefabName;

        //movement
        public float MoveSpeed;
        //used by nav mesh agent
        public float Acceleration;
        public float RotateSpeed;

        public Vector3 CameraOffset;

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

        public string GetRandomName()
        {
            return Names[UnityEngine.Random.Range(0, Names.Count)];
        }
    }


    [System.Serializable]
    public class MountFoodData
    {
        public int ItemID;
        public float FoodValue;
    }
}
