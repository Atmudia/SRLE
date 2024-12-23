using UnityEngine;

namespace SRLE.Utils;

public static class PrefabUtils
{
    public static Transform DisabledParent;
    static PrefabUtils()
    {
        DisabledParent = new GameObject("DeactivedObject").transform;
        DisabledParent.gameObject.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(DisabledParent.gameObject);
        DisabledParent.gameObject.hideFlags |= HideFlags.HideAndDontSave;
    }

    public static GameObject CopyPrefab(GameObject prefab)
    {
        var newG = UnityEngine.Object.Instantiate(prefab, DisabledParent);
        return newG;
    }
}