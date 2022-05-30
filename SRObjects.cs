using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SRLE
{
    public class SRObjects
    {
        public static T Get<T>() where T : Object
        {
            foreach (var o in Resources.FindObjectsOfTypeAll<T>())
            {
                return o;
            }

            return null;
        }
    }
}