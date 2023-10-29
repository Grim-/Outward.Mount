using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount.Mount_Components
{
    public class VFXComp : MountComp
    {
        public string SLPackName;
        public string AssetBundleName;
        public string PrefabName;
        public bool IsSkinnedMeshVFX = false;

        public Vector3 Position;
        public Vector3 Rotation;


        private GameObject VFXInstance;

        public override void OnApply(BasicMountController BasicMountController)
        {
            base.OnApply(BasicMountController);


            GameObject FoundPrefab = OutwardHelpers.GetFromAssetBundle<GameObject>(SLPackName, AssetBundleName, PrefabName);

            if (FoundPrefab == null)
            {
                EmoMountMod.LogMessage($"Could not find a prefab for SLPack : {SLPackName} AssetBundleName : {AssetBundleName} Prefab Name : {PrefabName}");
                return;
            }

            if (VFXInstance == null)
            {
                VFXInstance = GameObject.Instantiate(FoundPrefab);
            }


            VFXInstance.transform.parent = Controller.MountPoint;
            VFXInstance.transform.localPosition = Position;
            VFXInstance.transform.localEulerAngles = Rotation;

            ParticleSystem PS = VFXInstance.GetComponent<ParticleSystem>();

            if (PS && IsSkinnedMeshVFX)
            {
                ParticleSystem.MainModule main = PS.main;
                ParticleSystem.ShapeModule Shape = PS.shape;
                Shape.skinnedMeshRenderer = Controller.SkinnedMeshRenderer;
                Shape.position = Vector3.zero;
                Shape.rotation = Vector3.zero;
                Shape.scale = Vector3.one;
            }
        }

        public override void OnRemove(BasicMountController Controller)
        {
            base.OnRemove(Controller);
            if (VFXInstance)
            {
                Destroy(VFXInstance);
                VFXInstance = null;
            }
        }
    }

    [XmlType("VFXCompProp")]
    public class VFXCompProp : MountCompProp
    {
        [XmlElement("SLPackName")]
        public string SLPackName;
        [XmlElement("AssetBundleName")]
        public string AssetBundleName;
        [XmlElement("PrefabName")]
        public string PrefabName;
        [XmlElement("IsSkinnedMeshVFX")]
        public bool IsSkinnedMeshVFX = false;
        [XmlElement("Position")]
        public Vector3 Position;
        [XmlElement("Rotation")]
        public Vector3 Rotation;
    }

}




