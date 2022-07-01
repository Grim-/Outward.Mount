using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EmoMount.UI
{

    /// <summary>
    /// A UI Element for the Mount Retrieval Sceen
    /// </summary>
    public class MountSelectionElement : MonoBehaviour
    {
        public Text NameLabel;
        public Text FoodLabel;
        public Button RetrieveButton;

        public void Awake()
        {
            NameLabel = transform.Find("NameLabel").GetComponent<Text>();
            FoodLabel = transform.Find("FoodLabel").GetComponent<Text>();
            RetrieveButton = transform.Find("Button").GetComponent<Button>();
        }


        public void SetNameLabel(string text)
        {
            if (NameLabel != null)
            {
                NameLabel.text = text;
            }
        }

        public void SetFoodLabel(string text)
        {
            if (FoodLabel != null)
            {
                FoodLabel.text = text;
            }
        }

    }
}
