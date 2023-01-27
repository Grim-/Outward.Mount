using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoMount
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class WeightedItem<T>
    {
        [Range(0, 100)]
        public int Probability = 0;
        public T Value;

        public WeightedItem(int probability, T value)
        {
            Probability = probability;
            Value = value;
        }

        public static T GetWeightedRandomValueFromList<T>(List<WeightedItem<T>> weightedItems)
        {
            int sumOfAllWeights = GetSumOfAllWeights(weightedItems);

            int randWeight = Random.Range(1, sumOfAllWeights + 1);

            int something = 0;

            foreach (var item in weightedItems)
            {
                something += item.Probability;

                if (randWeight <= something)
                {
                    return item.Value;
                }
            }

            return default(T);
        }

        public static int GetSumOfAllWeights<T>(List<WeightedItem<T>> weightedItems)
        {
            int sum = 0;

            foreach (var item in weightedItems)
            {
                sum += item.Probability;
            }

            return sum;
        }
    }
}
