using UnityEngine;
using Object = UnityEngine.Object;

namespace SRLE
{
    public static class SRObjects
    {
        public static string GetFullName(this GameObject obj)
        {
            string str = obj.name;
            for (Transform parent = obj.transform.parent;
                (UnityEngine.Object) parent != (UnityEngine.Object) null;
                parent = parent.parent)
                str = parent.gameObject.name + "/" + str;
            return str;
        }

        public static Vector3 CopyVector(this Vector3 vector3)
        {
            return new Vector3(vector3.x, vector3.y, vector3.z);
        }
        public static Quaternion CopyQuaternion(this Quaternion quaternion)
        {
            return new Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
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