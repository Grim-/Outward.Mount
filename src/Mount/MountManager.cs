﻿using EmoMount.Mount_Components;
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
        /// Creates a new Mount in the scene next to the Owner Player from the XML Definition.
        /// </summary>
        /// <param name="_affectedCharacter"></param>
        /// <param name="MountSpecies"></param>
        /// <param name="Position"></param>
        /// <param name="Rotation"></param>
        /// <returns></returns>
        public BasicMountController CreateMountFromSpecies(Character _affectedCharacter, string mountSpecies, Vector3 Position, Vector3 Rotation, Color TintColor = default(Color), Color EmissionColor = default(Color), Action<BasicMountController> OnMountSpawnComplete = null)
        {
            if (string.IsNullOrEmpty(mountSpecies))
            {
                EmoMountMod.Log.LogMessage($"CreateMountFromSpecies MountSpecies was null.");
                return null;
            }

            //EmoMountMod.Log.LogMessage($"Creating {mountSpecies} for {_affectedCharacter.Name}");
            MountSpecies MountSpecies =  GetSpeciesDefinitionByName(mountSpecies);

            if (MountSpecies == null)
            {
                EmoMountMod.Log.LogMessage($"CreateMountFromSpecies MountSpecies was null.");
                return null;
            }


            GameObject Prefab = OutwardHelpers.GetFromAssetBundle<GameObject>(MountSpecies.SLPackName, MountSpecies.AssetBundleName, MountSpecies.PrefabName);
            GameObject MountInstance = null;


            //EmoMountMod.Log.LogMessage($"CreateMountFromSpecies PrefabName : {MountSpecies.SpeciesName} has {MountSpecies.MoveSpeed}");

            if (Prefab == null)
            {
                EmoMountMod.Log.LogMessage($"CreateMountForCharacter PrefabName : {MountSpecies.PrefabName} from AssetBundle was null.");
                return null;
            }

 
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

            basicMountController.MaxCarryWeight = EmoMountMod.WeightLimitOverride.Value > 0 ? EmoMountMod.WeightLimitOverride.Value : MountSpecies.MaximumCarryWeight;
            basicMountController.EncumberenceSpeedModifier = EmoMountMod.EncumberenceSpeedModifier.Value > 0 ? MountSpecies.EncumberenceSpeedModifier : 0.5f;
            basicMountController.MountFood.FoodTakenPerTick = MountSpecies.FoodTakenPerTick;
            basicMountController.MountFood.HungerTickTime = MountSpecies.HungerTickTime;


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


            CharacterMount characterMount = _affectedCharacter.gameObject.GetComponent<CharacterMount>();

            if (characterMount)
            {
                characterMount.SetActiveMount(basicMountController);
            }


            //Item Bag = ResourcesPrefabManager.Instance.GenerateItem("5300000");

            //if (Bag)
            //{
            //    if (!string.IsNullOrEmpty(bagUID))
            //    {
            //        EmoMountMod.Log.LogMessage($"Updateing Bag UID to {bagUID}");
            //        Bag.UID = bagUID;
            //    }

            //    basicMountController.SetInventory(Bag);
            //}


            MountControllers.Add(_affectedCharacter, basicMountController);

            return basicMountController;
        }

        /// <summary>
        /// Creates a new Mount in the scene next to the Owner Player from SaveData. If SetAsActive is true this also calls MountCanvasManager.RegisterMount and sets the mount as the CurrentActiveMount for the Character.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="mountInstanceData"></param>
        /// <param name="SetAsActive"></param>
        /// <returns></returns>
        public BasicMountController CreateMountFromInstanceData(Character character, MountInstanceData mountInstanceData, bool SetAsActive = true, Action<BasicMountController> OnMountSpawnComplete = null)
        {
            if (mountInstanceData == null)
            {
                EmoMountMod.Log.LogMessage($"CreateMountFromInstanceData Mount Instance data is null");
                return null;
            }

            BasicMountController basicMountController = CreateMountFromSpecies(character, mountInstanceData.MountSpecies, mountInstanceData.Position, mountInstanceData.Rotation);


            if (basicMountController == null)
            {
                EmoMountMod.Log.LogMessage($"CreateMountFromInstanceData Failed to Create Mount, controller is null.");
                return null;
            }

            basicMountController.MountName = mountInstanceData.MountName;
            basicMountController.MountUID = mountInstanceData.MountUID;
            basicMountController.MountFood.CurrentFood = mountInstanceData.CurrentFood;
            basicMountController.MountFood.MaximumFood = mountInstanceData.MaximumFood;

            basicMountController.SetTintColor(mountInstanceData.TintColor);
            basicMountController.SetEmissionColor(mountInstanceData.EmissionColor);


            if (SetAsActive)
            {
                EmoMountMod.MainCanvasManager.RegisterMount(basicMountController);
                character.GetComponent<CharacterMount>().SetActiveMount(basicMountController);
            }



            basicMountController.OnSpawnComplete += () =>
            {
                basicMountController.Teleport(character.transform.position, character.transform.eulerAngles);
                OnMountSpawnComplete?.Invoke(basicMountController);
            };

            return basicMountController;
        }
        //public void SerializeMountBagContents(MountInstanceData MountInstanceData, BasicMountController basicMountController)
        //{
        //    EmoMountMod.Log.LogMessage($"Saving Mount Bag Contents For {basicMountController.MountName}");

        //    Bag itemAsBag = (Bag)basicMountController.BagContainer;
        
        //    MountInstanceData.ItemSaveData = new List<BasicSaveData>();
        //    if (itemAsBag.Container == null)
        //    {
        //        EmoMountMod.Log.LogMessage($"{basicMountController.MountName} has no bag container.");
        //        //no bag
        //        return;
        //    }

        //    MountInstanceData.BagUID = itemAsBag.UID;

        //    if (itemAsBag.Container.ItemCount > 0)
        //    {
        //        foreach (var item in itemAsBag.Container.GetContainedItems())
        //        {
        //            EmoMountMod.Log.LogMessage($"Saving {item.UID} {item.Name}");
        //            MountInstanceData.ItemSaveData.Add(new BasicSaveData(item));
        //        }
        //    }
        //}
        //public void DeSerializeMountBagContents(MountInstanceData MountInstanceData, BasicMountController basicMountController)
        //{
        //    EmoMountMod.Log.LogMessage($"Loading Mount Bag Contents For {MountInstanceData.MountName}");

        //    if (basicMountController.BagContainer != null)
        //    {
        //        EmoMountMod.Log.LogMessage($"Loading  {MountInstanceData.ItemSaveData.Count} items");
        //        ItemManager.Instance.LoadItems(MountInstanceData.ItemSaveData);
        //    }
        //}
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

            if (MountCanvasManager.Instance.MountUIInstances.ContainsKey(basicMountController))
            {
                MountCanvasManager.Instance.UnRegisterMount(basicMountController);
            }

          
            //basicMountController.DestroyBagContainer();
            GameObject.Destroy(basicMountController.gameObject);
            MountControllers.Remove(character);
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
