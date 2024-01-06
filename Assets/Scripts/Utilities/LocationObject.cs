using System;
using UnityEditor;
using UnityEngine;
namespace Utilities
{
    [Serializable]

    public class LocationObject
    {
        public float X;
        public float Y;

        public Vector3 GetVector()
        {
            return new Vector3(X, Y, 0);
        }
    }

}
