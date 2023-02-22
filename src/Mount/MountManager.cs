using EmoMount.Mount_Components;
using SideLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace EmoMount
{
    /// <summary>
    /// Manages Mount Instances so they can be destroyed when needed along with their ui.
    /// </summary>
    public class MountManager
    {
        #region Properties
        public Dictionary<string, MountSpecies> SpeciesData
        {
            get; private set;
        }

        public Dictionary<Character, BasicMountController> MountControllers
        {
            get; private set;
        }

        public List<int> MountWhistleIDs
        {
            get; private set;
        }
        public List<int> WorldDropMountWhistleIDs
        {
            get; private set;
        }

        #endregion

        private string RootFolder;
        private string SpeciesFolder => RootFolder + "MountSpecies/";

        public MountManager(string rootFolder)
        {
            RootFolder = rootFolder;
            EmoMountMod.Log.LogMessage($"Initalising MountManager at : {RootFolder}");
            MountWhistleIDs = new List<int>();
            WorldDropMountWhistleIDs = new List<int>();
            MountControllers = new Dictionary<Character, BasicMountController>();
            MountComponentFactory.Initialize();
            LoadAllSpeciesDataFiles();
        }

        #region Species Definition

        private void LoadAllSpeciesDataFiles()
        {
            SpeciesData = new Dictionary<string, MountSpecies>();
            EmoMountMod.Log.LogMessage($"MountManager Loading Species Definitions..");

            if (!HasFolder(SpeciesFolder))
            {
                EmoMountMod.Log.LogMessage($"MountManager MountSpecies Folder does not exist, creating..");
                Directory.CreateDirectory(SpeciesFolder);
            }

            string[] filePaths = Directory.GetFiles(SpeciesFolder, "*.xml", SearchOption.AllDirectories);
            EmoMountMod.Log.LogMessage($"MountManager MountSpecies [{filePaths.Length}] Definitions found.");

            foreach (var item in filePaths)
            {
                string file = item;
                MountSpecies mountSpecies = null;
                try
                {
                    mountSpecies = DeserializeFromXML<MountSpecies>(file);
                }
                catch (Exception e)
                {
                    EmoMountMod.Log.LogMessage($"Failed To deserialize file {file} exception -> \r\n {e.Message} ");
                }

                if (mountSpecies != null)
                {
                    EmoMountMod.Log.LogMessage($"Created MountSpecies definition : {mountSpecies.SpeciesName}.");

                    if (!HasSpeciesDefinition(mountSpecies.SpeciesName))
                    {
                        if (mountSpecies.WhistleItemID != -1 || mountSpecies.WhistleItemID != 0)
                        {
                            CreateWhistle(mountSpecies);
                        }

                        SpeciesData.Add(mountSpecies.SpeciesName, mountSpecies);
                    }
                }
            }
        }
        public bool HasSpeciesDefinition(string SpeciesName)
        {
            if (SpeciesData.ContainsKey(SpeciesName))
            {
                return true;
            }
            return false;
        }
        public MountSpecies GetSpeciesDefinitionByName(string SpeciesName)
        {
            if (HasSpeciesDefinition(SpeciesName))
            {
                return SpeciesData[SpeciesName];
            }

            return null;
        }
        public MountSpecies GetSpeciesDefinitionByItemID(int ItemID)
        {
            foreach (var item in SpeciesData)
            {
                if (item.Value.WhistleItemID == ItemID)
                {
                    return item.Value;
                }
            }
            return null;
        }
        #endregion

        #region File + Xml
        public T DeserializeFromXML<T>(string path)
        {
            var assembly = Assembly.Load("EmoMount");
            var componentTypes = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(MountCompProp)));

            XmlSerializer serializer = new XmlSerializer(typeof(T), componentTypes.ToArray());
            StreamReader reader = new StreamReader(path);
            T deserialized = (T)serializer.Deserialize(reader.BaseStream);
            reader.Close();
            return deserialized;
        }
        private bool HasFolder(string FolderLocation)
        {
            return Directory.Exists(FolderLocation);
        }
        #endregion

        #region Controller
        /// <summary>
        /// Create a new Mount GameObject from the provided SpeciesName, add it to the DontDestroyOnLoad scene th
        /// </summary>
        /// <param name="_affectedCharacter"></param>
        /// <param name="MountSpecies"></param>
        /// <param name="Position"></param>
        /// <param name="Rotation"></param>
        /// <returns></returns>
        public BasicMountController CreateMountFromSpecies(string mountSpecies, Vector3 Position, Vector3 Rotation, Color TintColor = default(Color), Color EmissionColor = default(Color),  Action<BasicMountController> OnMountSpawnComplete = null)
        {
            if (string.IsNullOrEmpty(mountSpecies))
            {
                EmoMountMod.Log.LogMessage($"CreateMountFromSpecies MountSpecies was null.");
                return null;
            }

            MountSpecies MountSpecies =  GetSpeciesDefinitionByName(mountSpecies);

            if (MountSpecies == null)
            {
                EmoMountMod.Log.LogMessage($"CreateMountFromSpecies MountSpecies was null.");
                return null;
            }

            GameObject Prefab = OutwardHelpers.GetFromAssetBundle<GameObject>(MountSpecies.SLPackName, MountSpecies.AssetBundleName, MountSpecies.PrefabName);
            GameObject MountInstance = null;

            if (Prefab == null)
            {
                EmoMountMod.Log.LogMessage($"CreateMountFromSpecies PrefabName : {MountSpecies.PrefabName} from AssetBundle was null.");
                return null;
            }
            EmoMountMod.Log.LogMessage($"MountManager:CreateMountFromSpecies :: Creating Mount Instance from species {mountSpecies} at {Position}");
            MountInstance = GameObject.Instantiate(Prefab, Position, Quaternion.Euler(Rotation));
            GameObject.DontDestroyOnLoad(MountInstance);


           
            MountInstance.transform.position = Position;
            MountInstance.transform.eulerAngles = Rotation;

            BasicMountController basicMountController = MountInstance.AddComponent<BasicMountController>();
            basicMountController.MountName = MountSpecies.GetRandomName();

            basicMountController.SetSpecies(MountSpecies);

            basicMountController.MountFood.RequiresFood = MountSpecies.RequiresFood;
            basicMountController.MountFood.SetMaximumFood(MountSpecies.MaximumFood);
            basicMountController.MountFood.PassiveFoodLoss = MountSpecies.PassiveFoodLoss;
            basicMountController.MountFood.PassiveFoodLossTickTime = MountSpecies.PassiveFoodLossTickTime;

            basicMountController.MountFood.FoodLostPerTravelDistance = MountSpecies.FoodLostPerTravelDistance;
            basicMountController.MountFood.TravelDistanceThreshold = MountSpecies.TravelDistanceThreshold;

            basicMountController.SetFoodTags(MountSpecies.FoodTags);
            basicMountController.SetFavouriteFoods(MountSpecies.FavouriteFoods);
            basicMountController.SetHatedFoods(MountSpecies.HatedFoods);

            basicMountController.MaximumCarryWeight = EmoMountMod.WeightLimitOverride.Value > 0 ? EmoMountMod.WeightLimitOverride.Value : MountSpecies.MaximumCarryWeight;
            basicMountController.CarryWeightEncumberenceLimit = MountSpecies.CarryWeightEncumberenceLimit;
            basicMountController.EncumberenceSpeedModifier = EmoMountMod.EncumberenceSpeedModifier.Value > 0 ? MountSpecies.EncumberenceSpeedModifier : 0.5f;

            if (TintColor != default(Color))
            {
                basicMountController.SetTintColor(TintColor);
            }

            if (EmissionColor != default(Color))
            {
                basicMountController.SetEmissionColor(EmissionColor);
            }

            if (MountSpecies.MountComponents != null)
            {
                EmoMountMod.Log.LogMessage($"Attempting to parse MountComps [{MountSpecies.MountComponents.Count}]");

                foreach (var item in MountSpecies.MountComponents)
                {
                    if (item == null || string.IsNullOrEmpty(item.CompName))
                    {
                        continue;
                    }
                    MountComp comp = null;

                    try
                    {
                        comp = MountComponentFactory.CreateComponent(basicMountController, item.CompName);
                    }
                    catch (Exception e)
                    {
                        EmoMountMod.Log.LogMessage($"Failed to create component {item.CompName}. Exception -> \r\n {e.Message}");
                        continue;
                    }
               
                    if (comp)
                    {
                        EmoMountMod.Log.LogMessage($"Added {item.CompName} to {basicMountController.MountName}.");
                        MountComponentFactory.ApplyMountCompProps(comp, item);
                        comp.OnApply(basicMountController);
                    }       
                }
            }


            OnMountSpawnComplete?.Invoke(basicMountController);
            return basicMountController;
        }

        public BasicMountController CreateMountForCharacter(Character _affectedCharacter, string mountSpecies, Vector3 Position, Vector3 Rotation, Color TintColor = default(Color), Color EmissionColor = default(Color), bool SetAsCurrentActiveMount = true, Action<BasicMountController> OnMountSpawnComplete = null)
        {
            BasicMountController basicMountController = CreateMountFromSpecies(mountSpecies, Position, Rotation, TintColor, EmissionColor, OnMountSpawnComplete);
            basicMountController.SetOwner(_affectedCharacter);

            CharacterMount characterMount = _affectedCharacter.gameObject.GetComponent<CharacterMount>();

            if (characterMount && SetAsCurrentActiveMount)
            {
                characterMount.SetActiveMount(basicMountController);
                AddMountControllerForCharacter(_affectedCharacter, basicMountController);
            }


            basicMountController.OnSpawnComplete += () =>
            {
                basicMountController.TeleportToOwner();
            };
           

            return basicMountController;
        }
        /// <summary>
        /// Creates a new Mount in the scene next to the Owner Player from SaveData. If SetAsActive is true this also calls MountCanvasManager.RegisterMount and sets the mount as the CurrentActiveMount for the Character.
        /// </summary>
        /// <param name="CharacterMount"></param>
        /// <param name="mountInstanceData"></param>
        /// <param name="SetAsActive"></param>
        /// <returns></returns>
        public BasicMountController CreateMountFromInstanceData(CharacterMount CharacterMount, MountInstanceData mountInstanceData, Vector3 Position, Vector3 Rotation, bool SetAsActive = true, Action<BasicMountController> OnMountSpawnComplete = null)
        {
            if (mountInstanceData == null)
            {
                EmoMountMod.Log.LogMessage($"CreateMountFromInstanceData Mount Instance data is null");
                return null;
            }

            BasicMountController basicMountController = CreateMountForCharacter(CharacterMount.Character, mountInstanceData.MountSpecies, Position, Rotation, mountInstanceData.TintColor, mountInstanceData.EmissionColor, SetAsActive);


            if (basicMountController == null)
            {
                EmoMountMod.Log.LogMessage($"CreateMountFromInstanceData Failed to create Mount, controller is null.");
                return null;
            }

            basicMountController.MountName = mountInstanceData.MountName;
            basicMountController.MountUID = mountInstanceData.MountUID;

            basicMountController.MountFood.CurrentFood = mountInstanceData.CurrentFood;
            basicMountController.MountFood.MaximumFood = mountInstanceData.MaximumFood;

            AgeComp AgeComp = basicMountController.GetComponent<AgeComp>();

            if (AgeComp)
            {
                AgeComp.SetAge((int)mountInstanceData.AgeInSeconds);
            }

            basicMountController.SetTintColor(mountInstanceData.TintColor);
            basicMountController.SetEmissionColor(mountInstanceData.EmissionColor);


            if (SetAsActive)
            {
                EmoMountMod.MainCanvasManager.RegisterMount(basicMountController);
                CharacterMount.SetActiveMount(basicMountController);
            }

            basicMountController.OnSpawnComplete += () =>
            {
                OnMountSpawnComplete?.Invoke(basicMountController);
            };

            return basicMountController;
        }
        public void AddMountControllerForCharacter(Character Character, BasicMountController Mount)
        {
            if (MountControllers.ContainsKey(Character))
            {

                EmoMountMod.Log.LogMessage($"{Character.Name} already has a registered MountController updating.");

                MountControllers[Character] = Mount;
            }
            else
            {
                MountControllers.Add(Character, Mount);
            }
        }
        public void RemoveMountControllerForCharacter(Character Character)
        {
            if (MountControllers.ContainsKey(Character))
            {
                MountControllers.Remove(Character);
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
            if (EmoMountMod.MountManager.CharacterHasMount(character))
            {
                EmoMountMod.Log.LogMessage($"Destroying Active Mount for {character.Name}");
                DestroyMount(character, EmoMountMod.MountManager.GetActiveMountForCharacter(character));

                RemoveMountControllerForCharacter(character);
            }
        }
        public void DestroyMount(Character character, BasicMountController basicMountController)
        {
            EmoMountMod.Log.LogMessage($"Destroying Mount for {character.Name}");
            basicMountController.DisableNavMeshAgent();

            if (MountCanvasManager.Instance.HasRegisteredUI(basicMountController))
            {
                MountCanvasManager.Instance.UnRegisterMount(basicMountController);
            }

            GameObject.Destroy(basicMountController.gameObject);       
        }

        #endregion
        private SL_Item CreateWhistle(MountSpecies mountSpecies)
        {
            string NiceName = mountSpecies.SpeciesName.Replace("_", " ");
            SL_Item WhistleItem = new SL_Item()
            {
                Target_ItemID = mountSpecies.TargetItemID,
                New_ItemID = mountSpecies.WhistleItemID,
                Name = $"{NiceName} Whistle",
                Description = $"Can be used to call upon a tame {NiceName}.",
                EffectTransforms = new SL_EffectTransform[]
                {
                    new SL_EffectTransform()
                    {
                        TransformName = "Normal",
                        Effects = new SL_Effect[]
                        {
                            new SL_SpawnMount()
                            {
                                SpeciesName = mountSpecies.SpeciesName
                            }
                        }
                    }
                }
            };

            WhistleItem.ApplyTemplate();

            MountWhistleIDs.Add(WhistleItem.New_ItemID);

            if (mountSpecies.IsWhistleWorldDrop)
            {
                WorldDropMountWhistleIDs.Add(WhistleItem.New_ItemID);
            }

            return WhistleItem;
        }
        public int GetRandomWhistleID()
        {
            return WorldDropMountWhistleIDs[UnityEngine.Random.Range(0, WorldDropMountWhistleIDs.Count)];
        }
    }
}
