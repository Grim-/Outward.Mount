using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount
{
    /// <summary>
    /// Manages Mount Instances so they can be destroyed when needed along with their ui.
    /// </summary>
    public class MountManager
    {
        private List<MountSpecies> SpeciesData = new List<MountSpecies>();

        public Dictionary<Character, BasicMountController> MountControllers
        {
            get; private set;
        }

        private string RootFolder;
        private string SpeciesFolder => RootFolder + "MountSpecies/";

        public MountManager(string rootFolder)
        {
            RootFolder = rootFolder;
            
            EmoMountMod.Log.LogMessage($"Initalising MountManager at : {RootFolder}");
            MountControllers = new Dictionary<Character, BasicMountController>();
            LoadAllSpeciesDataFiles();
        }

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
            EmoMountMod.Log.LogMessage($"MountManager MountSpecies Definitions [{filePaths.Length}] Found.");


            foreach (var item in filePaths)
            {
                EmoMountMod.Log.LogMessage($"MountManager MountSpecies Reading {item} data.");
                MountSpecies mountSpecies = DeserializeFromXML<MountSpecies>(item);

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

        public static T DeserializeFromXML<T>(string path)
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
        /// <summary>
        /// Creates a new Mount in the scene next to the Owner Player from the XML Definition.
        /// </summary>
        /// <param name="_affectedCharacter"></param>
        /// <param name="MountSpecies"></param>
        /// <param name="Position"></param>
        /// <param name="Rotation"></param>
        /// <returns></returns>
        public BasicMountController CreateMountFromSpecies(Character _affectedCharacter, MountSpecies MountSpecies, Vector3 Position, Vector3 Rotation, string bagUID = "")
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
                GameObject.DontDestroyOnLoad(MountInstance);

                BasicMountController basicMountController = MountInstance.AddComponent<BasicMountController>();
                basicMountController.MountName = MountSpecies.GetRandomName();

                basicMountController.SetOwner(_affectedCharacter);
                basicMountController.SetSpecies(MountSpecies);
                basicMountController.SetFoodTags(MountSpecies.FoodTags);
                basicMountController.SetFavouriteFoods(MountSpecies.FavouriteFoods);
                basicMountController.SetHatedFoods(MountSpecies.HatedFoods);
                basicMountController.MountFood.SetMaximumFood(MountSpecies.MaximumFood);

                basicMountController.MaxCarryWeight = MountSpecies.MaximumCarryWeight;
                basicMountController.EncumberenceSpeedModifier = MountSpecies.EncumberenceSpeedModifier;
                basicMountController.MountFood.FoodTakenPerTick = MountSpecies.FoodTakenPerTick;
                basicMountController.MountFood.HungerTickTime = MountSpecies.HungerTickTime;

                CharacterMount characterMount = _affectedCharacter.gameObject.GetComponent<CharacterMount>();

                if (characterMount)
                {
                    characterMount.SetActiveMount(basicMountController);
                }


                Item Bag = ResourcesPrefabManager.Instance.GenerateItem("5300000");

                if (Bag)
                {
                    if (!string.IsNullOrEmpty(bagUID))
                    {
                        EmoMountMod.Log.LogMessage($"Updateing Bag UID to {bagUID}");
                        Bag.UID = bagUID;
                    }

                    basicMountController.SetInventory(Bag);
                }


                MountControllers.Add(_affectedCharacter, basicMountController);

                basicMountController.Teleport(Position, Rotation);
                return basicMountController;
            }

            return null;
        }
        /// <summary>
        /// Creates a new Mount in the scene next to the Owner Player from SaveData. If SetAsActive is true this also calls MountCanvasManager.RegisterMount and sets the mount as the CurrentActiveMount for the Character.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="mountInstanceData"></param>
        /// <param name="SetAsActive"></param>
        /// <returns></returns>
        public BasicMountController CreateMountFromInstanceData(Character character, MountInstanceData mountInstanceData, bool SetAsActive = true)
        {
            EmoMountMod.Log.LogMessage($"CreateMountFromInstanceData Creating from MountInstanceData for ");

            if (mountInstanceData == null)
            {
                EmoMountMod.Log.LogMessage($"CreateMountFromInstanceData Mount Instance data is null");
                return null;
            }

            BasicMountController basicMountController = CreateMountFromSpecies(character, mountInstanceData.MountSpecies, mountInstanceData.Position, mountInstanceData.Rotation, mountInstanceData.BagUID);


            if (basicMountController == null)
            {
                EmoMountMod.Log.LogMessage($"CreateMountFromInstanceData Failed to Create Mount");
                return null;
            }

            basicMountController.MountName = mountInstanceData.MountName;
            basicMountController.MountUID = mountInstanceData.MountUID;
            basicMountController.MountFood.CurrentFood = mountInstanceData.CurrentFood;
            basicMountController.MountFood.MaximumFood = mountInstanceData.MaximumFood;

            if (SetAsActive)
            {
                EmoMountMod.MainCanvasManager.RegisterMount(basicMountController);
                character.GetComponent<CharacterMount>().SetActiveMount(basicMountController);
            }

            return basicMountController;
        }

        /// <summary>
        /// Returns the SaveData for the BasicMountController
        /// </summary>
        /// <param name="characterMount"></param>
        /// <returns></returns>
        public MountInstanceData CreateInstanceDataFromMount(BasicMountController characterMount)
        {
            EmoMountMod.Log.LogMessage($"Creating Instance Data For {characterMount.MountName}");
            MountInstanceData mountInstanceData = new MountInstanceData();
            mountInstanceData.MountName = characterMount.MountName;
            mountInstanceData.MountUID = characterMount.MountUID;
            mountInstanceData.MountSpecies = characterMount.MountSpecies;
            mountInstanceData.BagUID = characterMount.BagContainer.UID;
            mountInstanceData.CurrentFood = characterMount.MountFood.CurrentFood;
            mountInstanceData.MaximumFood = characterMount.MountFood.MaximumFood;
            mountInstanceData.Position = characterMount.transform.position;
            mountInstanceData.Rotation = characterMount.transform.eulerAngles;
            return mountInstanceData;
        }

        public void SerializeMountBagContents(MountInstanceData MountInstanceData, BasicMountController basicMountController)
        {
            EmoMountMod.Log.LogMessage($"Saving Mount Bag Contents For {basicMountController.MountName}");

            Bag itemAsBag = (Bag)basicMountController.BagContainer;
        
            MountInstanceData.ItemSaveData = new List<BasicSaveData>();
            if (itemAsBag.m_container == null)
            {
                EmoMountMod.Log.LogMessage($"{basicMountController.MountName} has no bag container.");
                //no bag
                return;
            }

            MountInstanceData.BagUID = itemAsBag.UID;

            if (itemAsBag.m_container.ItemCount > 0)
            {
                foreach (var item in itemAsBag.m_container.GetContainedItems())
                {
                    EmoMountMod.Log.LogMessage($"Saving {item.UID} {item.Name}");
                    MountInstanceData.ItemSaveData.Add(new BasicSaveData(item));
                }
            }
        }

        public void DeSerializeMountBagContents(MountInstanceData MountInstanceData, BasicMountController basicMountController)
        {
            EmoMountMod.Log.LogMessage($"Loading Mount Bag Contents For {MountInstanceData.MountName}");

            if (basicMountController.BagContainer != null)
            {
                EmoMountMod.Log.LogMessage($"Loading  {MountInstanceData.ItemSaveData.Count} items");
                ItemManager.Instance.LoadItems(MountInstanceData.ItemSaveData);
            }
        }
        public bool CharacterHasMount(Character character)
        {
            if (MountControllers.ContainsKey(character))
            {
                return true;
            }

            return false;
        }
        public BasicMountController GetActiveMountForCharacter(Character _affectedCharacter)
        {
            if (MountControllers.ContainsKey(_affectedCharacter))
            {
                return MountControllers[_affectedCharacter];
            }

            return null;
        }
        public void DestroyAllMountInstances()
        {
            EmoMountMod.Log.LogMessage($"Destroying All Mount Instances...");

            if (MountControllers != null)
            {
                foreach (var mount in MountControllers.ToList())
                {
                   
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
            EmoMountMod.Log.LogMessage($"Destroying Mount instance for {character.Name}");
            basicMountController.DisableNavMeshAgent();
            MountCanvasManager.Instance.UnRegisterMount(basicMountController);
            basicMountController.DestroyBagContainer();
            GameObject.Destroy(basicMountController.gameObject);
            MountControllers.Remove(character);
        }

        #endregion
    }
}
