using System;
using UnityEngine;

namespace Utilities
{

    [Serializable]
    public class RangeList
    {
        public float MinValue = 0;
        public float MaxValue = 1;
        public RangeList(float minValue, float maxValue)
        {
            if (MinValue > MaxValue)
                Debug.LogError("Min value must be lower than max value.");
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public float GetRandomNumber()
        {
            return UnityEngine.Random.Range(MinValue, MaxValue);
        }
    }

}