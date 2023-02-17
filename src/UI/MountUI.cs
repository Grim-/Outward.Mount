using System;
using UnityEngine;
using UnityEngine.UI;

namespace EmoMount
{
    /// <summary>
    /// Controls the UI for the mount instance
    /// </summary>
    public class MountUI : MonoBehaviour
    {
        public Image StatusIcon
        {
            get; private set;
        }

        public Image HungerBar
        {
            get; private set;
        }
        public Text HungerLabel
        {
            get; private set;
        }

        public Image WeightBar
        {
            get; private set;
        }
        public Text WeightLabel
        {
            get; private set;
        }


        public Text NameLabel
        {
            get; private set;
        }

        public BasicMountController MountController
        {
            get; private set;
        }

        public void Awake()
        {
            NameLabel = transform.Find("NameLabel").GetComponent<Text>();
            HungerLabel = transform.Find("HungerLabel").GetComponent<Text>();
            HungerBar = transform.Find("Hunger/FG").GetComponent<Image>();
            WeightLabel = transform.Find("WeightLabel").GetComponent<Text>();
            WeightBar = transform.Find("Weight/FG").GetComponent<Image>();

            StatusIcon = transform.Find("StatusIcon").GetComponent<Image>();
        }

        public void SetTargetMount(BasicMountController basicMount)
        {
            MountController = basicMount;
            UpdateNameLabel(basicMount.MountName);

            MountController.MountFood.OnChange += UpdateUI;

            MountController.EventComp.OnMounted += OnMounted;
            MountController.EventComp.OnUnMounted += OnUnMounted;
            UpdateUI();
        }

        private void OnUnMounted(BasicMountController MountController, Character obj)
        {
            UpdateWeightBar(MountController.CurrentCarryWeight / MountController.MaximumCarryWeight);
            UpdateWeightLabel($"{MountController.CurrentCarryWeight} / {MountController.MaximumCarryWeight}");
        }

        private void OnMounted(BasicMountController MountController, Character obj)
        {
            if (MountController.WeightAsNormalizedPercent > MountController.CarryWeightEncumberenceLimit)
            {
                UpdateWeightLabel($"<color=red>{MountController.CurrentCarryWeight}</color> / {MountController.MaximumCarryWeight}");
            }
            else
            {
                UpdateWeightLabel($"{MountController.CurrentCarryWeight} / {MountController.MaximumCarryWeight}");
            }

            UpdateWeightBar(MountController.WeightAsNormalizedPercent);
            
        }

        private void UpdateUI()
        {
            UpdateHungerLabel($"{MountController.MountFood.CurrentFood} / {MountController.MountFood.MaximumFood}");
            UpdateHungerBar(MountController.MountFood.FoodAsNormalizedPercent);
        }

        public void UpdateStatusIcon(Sprite sprite)
        {
            if (StatusIcon != null)
            {
                StatusIcon.sprite = sprite;
            }
        }

        private void UpdateWeightBar(float value)
        {
            if (WeightBar != null)
            {
                WeightBar.fillAmount = value;
            }
        }

        private void UpdateWeightLabel(string text)
        {
            if (WeightLabel != null)
            {
                WeightLabel.text = text;
            }
        }


        private void UpdateHungerBar(float value)
        {
            if (HungerBar != null)
            {
                HungerBar.fillAmount = value;
            }
        }

        private void UpdateHungerLabel(string text)
        {
            if (HungerLabel != null)
            {
                HungerLabel.text = text;
            }
        }

        private void UpdateNameLabel(string text)
        {
            if (NameLabel != null)
            {
                NameLabel.text = text;
            }
        }

        public void ShowStatusIcon()
        {
            if (StatusIcon != null)
            {
                StatusIcon.gameObject.SetActive(true);
            }
        }

        public void HideStatusIcon()
        {
            if (StatusIcon != null)
            {
                StatusIcon.gameObject.SetActive(false);
            }
        }
    }
}
