using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoMount
{
    /// <summary>
    /// Maybe skip using an SL_Effect all together.
    /// </summary>
    public class SL_SpawnMount : SL_Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_SpawnMount);
        public Type GameModel => typeof(SLEx_SpawnMount);

        public string SLPackName;
        public string AssetBundleName;
        public string PrefabName;
        public float MountSpeed;
        public float RotateSpeed;

        public Vector3 PositionOffset;
        public Vector3 RotationOffset;
        public Vector3 MountScale;


        public override void ApplyToComponent<T>(T component)
        {
            SLEx_SpawnMount comp = component as SLEx_SpawnMount;
            comp.SLPackName = SLPackName;
            comp.AssetBundleName = AssetBundleName;
            comp.PrefabName = PrefabName;
            comp.MountSpeed = MountSpeed;
            comp.RotateSpeed = RotateSpeed;

            comp.PositionOffset = PositionOffset;
            comp.RotationOffset = RotationOffset;
            comp.MountScale = MountScale;
        }

        public override void SerializeEffect<T>(T effect)
        {

        }
    }

    public class SLEx_SpawnMount : Effect, ICustomModel
    {
        public Type SLTemplateModel => typeof(SL_SpawnMount);
        public Type GameModel => typeof(SLEx_SpawnMount);

        public string SLPackName;
        public string AssetBundleName;
        public string PrefabName;
        public float MountSpeed;
        public float RotateSpeed;

        public Vector3 PositionOffset;
        public Vector3 RotationOffset;
        public Vector3 MountScale;
        public GameObject Prefab;

        public override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            if (!EmoMountMod.MountManager.CharacterHasMount(_affectedCharacter))
            {
                    EmoMountMod.MountManager.CreateMountForCharacter(_affectedCharacter, SLPackName, AssetBundleName, PrefabName, "5300000", 
                    OutwardHelpers.GetPositionAroundCharacter(_affectedCharacter, PositionOffset), RotationOffset, MountSpeed, RotateSpeed);
            }      
        }
    }
}

