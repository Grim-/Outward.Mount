﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmoMount
{
    public class MountFood : MonoBehaviour
    {
        public float CurrentFood = 0;
        public float MaximumFood = 100;
        public float PassiveFoodLoss = 10f;
        public bool RequiresFood = true;

        public float TravelDistanceThreshold = 100f;
        public float FoodLostPerTravelDistance = 15f;

        public float PassiveFoodLossTickTime = 60f;
        public float CurrentHungerTimer = 0;

        public float FoodAsNormalizedPercent => CurrentFood / MaximumFood;

        //itemid, food mod value
        public List<Tag> FoodTags = new List<Tag>();
        public Dictionary<int, float> FavouriteFoods = new Dictionary<int, float>();
        public Dictionary<int, float> HatedFoods = new Dictionary<int, float>();

        /// <summary>
        /// called when starving (0 food since last tick)
        /// </summary>
        public Action OnStarving;
        /// <summary>
        /// called when full
        /// </summary>
        public Action OnFull;
        /// <summary>
        /// Called when given some food
        /// </summary>
        public Action OnFed;
        /// <summary>
        /// Called every hunger tick
        /// </summary>
        public Action OnHungerTick;
        /// <summary>
        /// Called everytime the food value changes
        /// </summary>
        public Action OnChange;

        public BasicMountController MountController
        {
            get; private set;
        }

        /// <summary>
        /// Called by MountController
        /// </summary>
        public void Init()
        {
            MountController = GetComponent<BasicMountController>();
        }


        public void Update()
        {
            if (RequiresFood)
            {
                CurrentHungerTimer += Time.deltaTime;

                if (CurrentHungerTimer >= PassiveFoodLossTickTime)
                {
                    OnHungerTickHandler();

                    CurrentHungerTimer = 0;
                }
            }
        }

        public bool CanEat(Item item)
        {
            if (item.HasTag(FoodTags, false))
            {
                return true;
            }
            return false;
        }

        public void Feed(Item item, float foodAmount)
        {
            if (!CanEat(item))
            {
                MountController.DisplayNotification($"{MountController.MountName} can't eat this type of food!");
                MountController.PlayMountAnimation(MountAnimations.MOUNT_ANGRY);
                return;
            }

            float finalFoodValue = foodAmount;

            if (IsFavouriteFood(item.ItemID))
            {
                finalFoodValue *= FavouriteFoods[item.ItemID];

                MountController.DisplayNotification($"{MountController.MountName} loves this food!");
                MountController.PlayMountAnimation(MountAnimations.MOUNT_HAPPY);
            }
            else if (IsHatedFood(item.ItemID))
            {
                finalFoodValue *= HatedFoods[item.ItemID];
                MountController.DisplayNotification($"{MountController.MountName} HATES this food!");
                MountController.PlayMountAnimation(MountAnimations.MOUNT_ANGRY);
            }


            CurrentFood += finalFoodValue;

            if (CurrentFood >= MaximumFood)
            {
                CurrentFood = MaximumFood;
                OnFullHandler();
            }


            item.ParentContainer.RemoveItem(item);


            OnFoodAddedHandler();
            OnChange?.Invoke();
        }
        public void Remove(float foodAmount)
        {
            if (!EmoMountMod.EnableFoodNeed.Value || !RequiresFood)
            {
                return;
            }
           

            CurrentFood -= foodAmount;

            if (CurrentFood <= 0)
            {
                CurrentFood = 0;
                //starving
                OnStarvingHandler();
            }

            OnFoodRemovedHandler();
            OnChange?.Invoke();
        }

        public void OnHungerTickHandler()
        {
            if (!EmoMountMod.EnableFoodNeed.Value || !RequiresFood)
            {
                return;
            }

            if (MountController.IsMounted)
            {
                float halfAgain = PassiveFoodLoss * 0.5f;
                Remove(PassiveFoodLoss + halfAgain);
            }
            else
            {
                Remove(PassiveFoodLoss);
            }

            OnHungerTick?.Invoke();
        }


        public void OnFullHandler()
        {
            OnFull?.Invoke();
            MountController.DisplayImportantNotification($"{MountController.MountName} is full!");
        }

        public void OnFoodAddedHandler()
        {
            OnFed?.Invoke();
        }


        public void OnStarvingHandler()
        {
            if (!RequiresFood)
            {
                return;
            }

            OnStarving?.Invoke();
            MountController.DisplayImportantNotification($"{MountController.MountName} is starving!");
        }

        public void OnFoodRemovedHandler()
        {
           
        }

        public bool IsFavouriteFood(int itemID)
        {
            if (FavouriteFoods.ContainsKey(itemID))
            {
                return true;
            }

            return false;
        }

        public bool IsHatedFood(int itemID)
        {
            if (HatedFoods.ContainsKey(itemID))
            {
                return true;
            }

            return false;
        }

        public void SetMaximumFood(float MaxFood)
        {
            MaximumFood = MaxFood;
            CurrentFood = MaxFood;
        }
    }

}

