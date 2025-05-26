using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public static class RandomWeighted
    {
        /// <summary>
        /// Picks random number using a weight table
        /// </summary>
        /// <param name="min">Minimum number (included)</param>
        /// <param name="max">Maximum number (included)</param>
        /// <param name="weightTable">Example : new RandomWeighted.WeightTable(1, 0.5f) means number 1 has 50% chance of appearing.</param>
        public static int PickRandom(int min, int max, params WeightedNumber[] weightTable)
        {
            if (min > max)
            {
                Debug.LogError("Minimum value is greater than maximum value!");
                return -1;
            }

            int count = (max - min) + 1;

            #region Calculate used and remaining weight

            float usedWeight = 0;
            int weightedNCount = weightTable.Length;

            for (int i = 0; i < weightedNCount; i++)
            {
                usedWeight += weightTable[i].percentage;
            }

            float diff = 1 - usedWeight;
            if (diff < -0.001f)
            {
                Debug.LogError("The used weight table is greater than 1!");
                return -1;
            }

            float remainWeight = (1.0f - usedWeight) / (count - weightedNCount);

            #endregion

            #region Creating final weighted table
            //Create a new list and add all the number and their weight (including numbers that hasn't been weighted)

            List<WeightedNumber> finalWeightTable = new List<WeightedNumber>();
            for (int i = 0; i < count; i++)
            {
                int curNum = min + i;
                float curWeight;

                if (!ContainsNumber(weightTable, curNum))
                    curWeight = remainWeight;
                else
                    curWeight = GetWeight(weightTable, curNum);

                finalWeightTable.Add(new WeightedNumber(curNum, curWeight));
            }

            #endregion

            #region Extracting the number
            float randomValue = Random.value;

            float curWeightChecking = 0;
            for (int i = 0; i < finalWeightTable.Count; i++)
            {

                if (i != finalWeightTable.Count - 1)    //Add the weight if its not the last number, else just return the final number(there're no other options so far)
                    curWeightChecking += finalWeightTable[i].percentage;
                else
                    return finalWeightTable[i].number;

                if (randomValue <= curWeightChecking)
                {
                    return finalWeightTable[i].number;
                }
            }
            #endregion

            return Random.Range(min, max + 1);  //Just in case
        }

        private static bool ContainsNumber(WeightedNumber[] weightedNumbers, int num)
        {

            foreach (WeightedNumber wh in weightedNumbers)
            {
                if (wh.number == num)
                    return true;
            }
            return false;
        }

        private static float GetWeight(WeightedNumber[] weightedNumbers, int num)
        {
            foreach (WeightedNumber wh in weightedNumbers)
            {
                if (wh.number == num)
                    return wh.percentage;
            }
            return 0;
        }

    }
    public class WeightedNumber
    {
        public int number;
        public float percentage;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num">Number</param>
        /// <param name="percen">Percentage of the number appearance</param>
        public WeightedNumber(int num, float percen)
        {
            number = num;
            percentage = percen;
        }
    }
}