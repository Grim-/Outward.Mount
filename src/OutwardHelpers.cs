using SideLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EmoMount
{
    public static class OutwardHelpers
    {
        public static string EMISSION_VALUE = "_EmissionColor";

        public static void UpdateWeaponDamage(Weapon WeaponInstance, DamageList newDamageList)
        {
            WeaponInstance.Damage.Clear();
            //just fucken nuke everything 
            WeaponInstance.Stats.BaseDamage = newDamageList;
            WeaponInstance.m_baseDamage = WeaponInstance.Stats.BaseDamage.Clone();
            WeaponInstance.m_activeBaseDamage = WeaponInstance.Stats.BaseDamage.Clone();
            WeaponInstance.baseDamage = WeaponInstance.Stats.BaseDamage.Clone();
            WeaponInstance.Stats.Attacks = SL_WeaponStats.GetScaledAttackData(WeaponInstance);
            //ta-da updated weapon damage
        }

        public static Renderer TryGetWeaponRenderer(Weapon weaponGameObject)
        {
            return weaponGameObject.LoadedVisual.GetComponentInChildren<BoxCollider>().GetComponent<Renderer>();
        }

        public static T GetFromAssetBundle<T>(string SLPackName, string AssetBundle, string key) where T : UnityEngine.Object
        {
            if (!SL.PacksLoaded)
            {
                return default(T);
            }

            return SL.GetSLPack(SLPackName).AssetBundles[AssetBundle].LoadAsset<T>(key);
        }
        public static List<WeightedItem<Color>> ConvertToWeightedItemList(List<MountColorChance> mountColorChances)
        {
            List<WeightedItem<Color>> weightedItems = new List<WeightedItem<Color>>();
            foreach (var mountColorChance in mountColorChances)
            {
               weightedItems.Add(new WeightedItem<Color>(mountColorChance.Chance, mountColorChance.Color));        
            }
            return weightedItems;
        }

        public static Vector3 GetRandomPositionAroundCharacter(Character _affectedCharacter, float Radius = 5f)
        {
            Vector3 Position = _affectedCharacter.transform.position + Random.onUnitSphere * Radius;

            Position.y = _affectedCharacter.transform.position.y;
            return Position;
        }

        public static Vector3 GetPositionAroundCharacter(Character _affectedCharacter, Vector3 PositionOffset = default(Vector3))
        {
            return _affectedCharacter.transform.position + PositionOffset;
        }

        public static void DelayDo(Action OnAfterDelay, float DelayTime)
        {
            EmoMountMod.Instance.StartCoroutine(DoAfterDelay(OnAfterDelay, DelayTime));
        }

        public static IEnumerator DoAfterDelay(Action OnAfterDelay, float DelayTime)
        {
            yield return new WaitForSeconds(DelayTime);
            OnAfterDelay.Invoke();
            yield break;
        }

        public static List<T> GetTypeFromColliders<T>(Collider[] colliders) where T : Component
        {
            List<T> list = new List<T>();
            foreach (var col in colliders)
            {
                T type = col.GetComponentInChildren<T>();
                if (type != null)
                {
                    list.Add(type);
                }
            }
            return list;
        }

        public static void AddDamageType(DamageList Damage, DamageType.Types damageType, float value)
        {

            if (!HasDamageType(Damage, damageType))
            {
                Damage.Add(damageType);
                Damage[damageType].Damage = value;
            }
        }
        public static void RemoveDamageType(DamageList Damage, DamageType.Types damageType)
        {
            if (HasDamageType(Damage, damageType))
            {
                Damage.Remove(damageType);
            }
        }
        public static DamageType GetDamageTypeDamage(DamageList Damage, DamageType.Types damageType)
        {
            return Damage[damageType];
        }

        public static void AddDamageToDamageType(DamageList Damage, DamageType.Types damageType, float value, bool addIfNotExist = true)
        {
            if (HasDamageType(Damage, damageType))
            {
                GetDamageTypeDamage(Damage, damageType).Damage += value;
            }
            else
            {
                if (addIfNotExist) AddDamageType(Damage, damageType, value);
            }
        }
        public static void RemoveDamageFromDamageType(DamageList Damage, DamageType.Types damageType, float value)
        {
            if (HasDamageType(Damage, damageType))
            {
                GetDamageTypeDamage(Damage, damageType).Damage -= value;

                if (Damage[damageType].Damage <= 0)
                {
                    RemoveDamageType(Damage, damageType);
                }
            }
        }

        public static bool HasDamageType(DamageList Damage, DamageType.Types damageType)
        {
            return Damage[damageType] != null;
        }

        public static Tag GetTagDefinition(string TagName)
        {
            foreach (var item in TagSourceManager.Instance.m_tags)
            {

                if (item.TagName == TagName)
                {
                    return item;
                }
            }

            return default(Tag);
        }


        public static void GrantItemRewardToAllPlayers(int ItemID, int Quantity)
        {
            Character[] playerCharacters = CharacterManager.Instance.Characters.ValuesArray.Where(x => x.IsLocalPlayer || !x.IsAI).ToArray();

            foreach (var player in playerCharacters)
            {
                player.Inventory.ReceiveItemReward(ItemID, Quantity, false);
            }
        }

        public static void SpawnTransformVFX(SkinnedMeshRenderer Target, float DelayDestroy = 3f, string VFXName = "TransformVFX_Smoke", ParticleSystemSimulationSpace ParticleSystemSimulationSpace  = ParticleSystemSimulationSpace.World)
        {
            GameObject prefab = OutwardHelpers.GetFromAssetBundle<GameObject>("mount", "emomountbundle", VFXName);

            if (prefab)
            {
                GameObject instance = GameObject.Instantiate(prefab, Target.transform);
                instance.transform.localPosition = Vector3.zero;

                ParticleSystem ps = instance.GetComponent<ParticleSystem>();
                ParticleSystem.MainModule main = ps.main;
                if (ps)
                {
                    main.simulationSpace = ParticleSystemSimulationSpace;

                    ParticleSystem.ShapeModule shape = ps.shape;
                   
                    if (Target)
                    {
                        shape.skinnedMeshRenderer = Target;
                    }                           
                }

                GameObject.Destroy(instance, DelayDestroy);
            }

        }



        public static float GetTotalFoodValue(Food foodItem)
        {
            float foodValue = 0;

            foreach (var item in foodItem.m_affectFoodEffects)
            {
                foodValue += item.m_affectQuantity * item.m_totalPotency;
            }

            return foodValue;
        }
    }
}
