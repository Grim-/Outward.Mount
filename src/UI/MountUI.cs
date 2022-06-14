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
            StatusIcon = transform.Find("StatusIcon").GetComponent<Image>();
        }

        public void SetTargetMount(BasicMountController basicMount)
        {
            MountController = basicMount;
            UpdateNameLabel(basicMount.MountName);
        }

        private void Redraw()
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
