using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
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

        private string RootFolder;
        private string SpeciesFolder => RootFolder + "MountSpecies/";

        public MountManager(string rootFolder)
        {
            RootFolder = rootFolder;
            
            EmoMountMod.Log.LogMessage($"Initalising MountManager {RootFolder}");
            MountControllers = new Dictionary<Character, BasicMountController>();
            LoadAllSpeciesDataFiles();
       }

        public List<MountSpecies> SpeciesData = new List<MountSpecies>();



        private void LoadAllSpeciesDataFiles()
        {
            EmoMountMod.Log.LogMessage($"MountManager Initalising Species Definitions..");
            SpeciesData.Clear();

            if (!HasFolder("MountSpecies"))
            {
                EmoMountMod.Log.LogMessage($"MountManager MountSpecies Folder does not exist, creating..");
                Directory.CreateDirectory(RootFolder + "MountSpecies/");
            }

            string[] filePaths = Directory.GetFiles(SpeciesFolder, "*.xml");
            EmoMountMod.Log.LogMessage($"MountManager MountSpecies Definitions {filePaths.Length} Found.");


            foreach (var item in filePaths)
            {
                EmoMountMod.Log.LogMessage($"MountManager MountSpecies Reading {item} data.");
                MountSpecies mountSpecies = Deserialize<MountSpecies>(item);

                if (!HasSpeciesDefinition(mountSpecies.SpeciesName))
                {
                    SpeciesData.Add(mountSpecies);
                    EmoMountMod.Log.LogMessage($"MountManager MountSpecies Added {mountSpecies.SpeciesName} data.");
                }           
            }
        }

        public bool HasSpeciesDefinition(string SpeciesName)
        {
            return SpeciesData.Find(x => x.SpeciesName == SpeciesName) != null ? true : false;
        }

        public MountSpecies GetSpeciesDefinitionByName(string SpeciesName)
        {
            if (SpeciesData != null)
            {
                return SpeciesData.Find(x => x.SpeciesName == SpeciesName);
            }

            return null;
        }

        public static void Serialize(MountSpecies item, string path)
        {
            XmlSerializer serializer = new XmlSerializer(item.GetType());
            StreamWriter writer = new StreamWriter(path);
            serializer.Serialize(writer.BaseStream, item);
            writer.Close();
        }

        public static T Deserialize<T>(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamReader reader = new StreamReader(path);
            T deserialized = (T)serializer.Deserialize(reader.BaseStream);
            reader.Close();
            return deserialized;
        }

        private bool HasFolder(string FolderLocation)
        {
            return Directory.Exists(FolderLocation);
        }

        #region Controller
         public BasicMountController CreateMountFor(Character _affectedCharacter, MountSpecies MountSpecies, Vector3 Position, Vector3 Rotation)
        {
            GameObject Prefab = OutwardHelpers.GetFromAssetBundle<GameObject>(MountSpecies.SLPackName, MountSpecies.AssetBundleName, MountSpecies.PrefabName);
            GameObject MountInstance = null;

            if (Prefab == null)
            {
                EmoMountMod.Log.LogMessage($"CreateMountForCharacter PrefabName : {MountSpecies.PrefabName} from AssetBundle was null.");
                return null;
            }

            if (MountInstance == null)
            {
                MountInstance = GameObject.Instantiate(Prefab, Position, Quaternion.Euler(Rotation));

                AddMountInteractionComponents(MountInstance);
                GameObject.DontDestroyOnLoad(MountInstance);

                BasicMountController basicMountController = MountInstance.AddComponent<BasicMountController>();
                basicMountController.MountName = MountSpecies.GetRandomName();

                basicMountController.SetOwner(_affectedCharacter);
                basicMountController.SetSpecies(MountSpecies);
                basicMountController.SetFavouriteFoods(MountSpecies.FavouriteFoods);
                basicMountController.SetHatedFoods(MountSpecies.HatedFoods);

                CharacterMount characterMount = _affectedCharacter.gameObject.GetComponent<CharacterMount>();

                if (characterMount)
                {
                    characterMount.SetActiveMount(basicMountController);
                }

                Item Bag = CreateMountBag(basicMountController, "5300000");

                if (Bag)
                {
                    basicMountController.SetInventory(Bag);
                }

                MountControllers.Add(_affectedCharacter, basicMountController);

                return basicMountController;
            }

            return null;
        }

        public MountInstanceData CreateInstanceData(BasicMountController characterMount)
        {
            MountInstanceData mountInstanceData = new MountInstanceData();
            mountInstanceData.MountName = characterMount.MountName;
            mountInstanceData.MountUID = characterMount.MountUID;
            mountInstanceData.MountSpecies = characterMount.MountSpecies;
            mountInstanceData.BagID = characterMount.BagContainer.ItemIDString;
            mountInstanceData.CurrentFood = characterMount.MountFood.CurrentFood;
            mountInstanceData.MaximumFood = characterMount.MountFood.MaximumFood;
            mountInstanceData.Position = characterMount.transform.position;
            mountInstanceData.Rotation = characterMount.transform.eulerAngles;
            return mountInstanceData;
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
            EmoMountMod.Log.LogMessage($"Creating Interaction Components...");

            MountUpInteraction mountInteraction = MountInstance.AddComponent<MountUpInteraction>();
            FeedMountInteraction feedMountInteraction = MountInstance.AddComponent<FeedMountInteraction>();
            PetMountInteraction petMountInteraction = MountInstance.AddComponent<PetMountInteraction>();
            DismissMountInteraction dismissMountInteraction = MountInstance.AddComponent<DismissMountInteraction>();
            InteractionActivator interactionActivator = MountInstance.AddComponent<InteractionActivator>();
            InteractionTriggerBase interactionTriggerBase = MountInstance.AddComponent<InteractionTriggerBase>();

            interactionActivator.BasicInteraction = mountInteraction;
            //interactionActivator.AddBasicInteractionOverride(petMountInteraction);
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
                    DestroyMount(mount.Key, mount.Value);
                }

                MountControllers.Clear();

                EmoMountMod.Log.LogMessage($"All Mount Instances Destroyed Successfully.");
            }
        }
        public void DestroyActiveMount(Character character)
        {
            EmoMountMod.Log.LogMessage($"Destroying Active Mount instance for {character.Name}");

            if (MountControllers != null)
            {
                if (MountControllers.ContainsKey(character))
                {
                    DestroyMount(character, MountControllers[character]);
                }
            }
        }
        public void DestroyMount(Character character, BasicMountController basicMountController)
        {
            MountCanvasManager.Instance.UnRegisterMount(basicMountController);
            GameObject.Destroy(basicMountController.gameObject);
            MountControllers.Remove(character);
        }

        #endregion
    }
}
