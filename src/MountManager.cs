using System.Collections.Generic;
using UnityEngine;

namespace EmoMount
{
    /// <summary>
    /// Manages Mount Instances so they can be destroyed when needed along with their ui.
    /// </summary>
    public class MountManager
    {
        public Dictionary<Character, BasicMountController> MountControllers
        {
            get; private set;
        }

        public MountManager()
        {
            MountControllers = new Dictionary<Character, BasicMountController>();
            EmoMountMod.Log.LogMessage($"Initalising MountManager");
        }

        private string[] Names = new string[]
        {
                "Buddy",
                "Maisie",
                "Taff",
                "Lola",
                "Mooch",
                "Ebony"
        };

        public BasicMountController CreateMountForCharacter(Character _affectedCharacter, string MountName, string SLPackName, string AssetBundleName, string PrefabName, string bagID, Vector3 Position, Vector3 Rotation, float MountSpeed, float RotateSpeed)
        {
            GameObject Prefab = OutwardHelpers.GetFromAssetBundle<GameObject>(SLPackName, AssetBundleName, PrefabName);
            GameObject MountInstance = null;

            if (Prefab == null)
            {
                EmoMountMod.Log.LogMessage($"CreateMountForCharacter PrefabName : {PrefabName} from AssetBundle was null.");
                return null;
            }

            if (MountInstance == null)
            {
                MountInstance = GameObject.Instantiate(Prefab, Position, Quaternion.Euler(Rotation));

                AddMountInteractionComponents(MountInstance);
                GameObject.DontDestroyOnLoad(MountInstance);

                BasicMountController basicMountController = MountInstance.AddComponent<BasicMountController>();

                if (string.IsNullOrEmpty(MountName))
                {
                    basicMountController.MountName = EmoMountMod.MountManager.GetRandomName();
                }
                else
                {
                    basicMountController.MountName = MountName;
                }
             
                basicMountController.SetOwner(_affectedCharacter);
                basicMountController.SetMoveSpeed(MountSpeed);
                basicMountController.SetRotationSpeed(RotateSpeed);
                basicMountController.SetPrefabDetails(SLPackName, AssetBundleName, PrefabName);


                CharacterMount characterMount = _affectedCharacter.gameObject.GetComponent<CharacterMount>();

                if (characterMount)
                {
                    characterMount.SetMount(basicMountController);
                }

                Item Bag = CreateMountBag(basicMountController, bagID);

                if (Bag)
                {
                    basicMountController.SetInventory(Bag);
                }

                MountControllers.Add(_affectedCharacter, basicMountController);

                basicMountController.SetMountUI(MountCanvasManager.Instance.RegisterMount(basicMountController));
                return basicMountController;
            }

            return null;
        }
        public bool CharacterHasMount(Character character)
        {
            if (MountControllers.ContainsKey(character))
            {
                return true;
            }

            return false;
        }
        public BasicMountController GetControllerForCharacter(Character _affectedCharacter)
        {
            if (MountControllers.ContainsKey(_affectedCharacter))
            {
                return MountControllers[_affectedCharacter];
            }

            return null;
        }

        public Item CreateMountBag(BasicMountController mountController, string bagItemID)
        {
            Item Bag = ResourcesPrefabManager.Instance.GenerateItem(bagItemID);

            if (Bag != null)
            {
                if (Bag is Bag)
                {
                    Rigidbody bagRigidbody = Bag.gameObject.GetComponent<Rigidbody>();

                    if (bagRigidbody)
                    {
                        bagRigidbody.isKinematic = true;
                        bagRigidbody.useGravity = false;
                    }

                    RigidbodySuspender rigidbodySuspender = Bag.gameObject.GetComponentInChildren<RigidbodySuspender>();

                    if (rigidbodySuspender)
                    {
                        rigidbodySuspender.enabled = false;
                        GameObject.Destroy(rigidbodySuspender);
                    }

                }
                else
                {
                    EmoMountMod.Log.LogMessage($"Create Bag For Mount : ItemID {bagItemID} is not a bag.");
                }
            }
            else
            {
                EmoMountMod.Log.LogMessage($"Create Bag For Mount : ItemID {bagItemID} prefab was not found.");
            }

            return Bag;
        }



        public void AddMountInteractionComponents(GameObject MountInstance)
        {
            MountUpInteraction mountInteraction = MountInstance.AddComponent<MountUpInteraction>();
            FeedMountInteraction feedMountInteraction = MountInstance.AddComponent<FeedMountInteraction>();
            InteractionActivator interactionActivator = MountInstance.AddComponent<InteractionActivator>();
            InteractionTriggerBase interactionTriggerBase = MountInstance.AddComponent<InteractionTriggerBase>();

            interactionActivator.BasicInteraction = mountInteraction;
            interactionActivator.m_defaultHoldInteraction = feedMountInteraction;
            interactionTriggerBase.DetectionColliderRadius = 1.2f;
        }

        public void DestroyAllMountInstances()
        {
            EmoMountMod.Log.LogMessage($"Destroying All Mount Instances...");

            if (MountControllers != null)
            {
                foreach (var mount in MountControllers)
                {
                    mount.Value.DisableNavMeshAgent();
                    EmoMountMod.Log.LogMessage($"Destroying and unregistring from UI for {mount.Value.MountName} of {mount.Key.Name}");
                    MountCanvasManager.Instance.UnRegisterMount(mount.Value);
                    GameObject.Destroy(mount.Value.gameObject);
                }

                MountControllers.Clear();

                EmoMountMod.Log.LogMessage($"All Mount Instances Destroyed Successfully.");
            }
        }

        public string GetRandomName()
        {
            return Names != null ? Names[Random.Range(0, Names.Length)] : "Scooby";
        }
    }
}
