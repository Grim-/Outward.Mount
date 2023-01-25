using System.Collections.Generic;
using UnityEngine;

namespace EmoMount
{
    [System.Serializable]
    public class MountInstanceData
    {
        public string MountName;
        public string MountUID;
        public string MountSpecies;
        public string BagUID;
        public float CurrentFood;
        public float MaximumFood;
        public Vector3 Position;
        public Vector3 Rotation;

        public List<BasicSaveData> ItemSaveData;
    }
}
