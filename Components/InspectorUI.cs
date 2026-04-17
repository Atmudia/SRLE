using UnityEngine;
using UnityEngine.UI;

namespace SRLE.Components
{
    public class InspectorUI : MonoBehaviour
    {
        private GameObject m_Gameobject;
        private ScrollRect m_InspectorScroll;
        public static InspectorUI Instance;

        private void Start()
        {
            Instance = this;
            m_Gameobject = transform.Find("Inspector").gameObject;
            m_InspectorScroll = transform.Find("Inspector/InspectorScroll").GetComponent<ScrollRect>();
        }

        public void SetActive(bool active)
        {
            m_Gameobject.SetActive(active);

            foreach (Transform child in m_InspectorScroll.content)
                Destroy(child.gameObject);

            HierarchyUI.Instance.transform.Find("Hierarchy").gameObject.SetActive(!active);

            if (!active) return;

            var obj = SRLECamera.Instance.transformGizmo.targetRootsOrdered[0];

            var positionObj = Instantiate(AssetManager.InspectorVector3, m_InspectorScroll.content);
            var positionUI = positionObj.AddComponent<InspectorVector3UI>();
            positionUI.Name = "Position";
            var positionTransform = obj.transform;
            positionUI.BindTo(
                () => positionTransform.position,
                value => {
                    var prev = positionTransform.position;
                    positionTransform.position = (Vector3)value;
                    ChunkManager.Reregister(positionTransform, prev);
                });

            var rotationObj = Instantiate(AssetManager.InspectorVector3, m_InspectorScroll.content);
            var rotationUI = rotationObj.AddComponent<InspectorVector3UI>();
            rotationUI.Name = "Rotation";
            rotationUI.BindTo(obj.transform, typeof(Transform).GetMember("localEulerAngles")[0]);

            var scaleObj = Instantiate(AssetManager.InspectorVector3, m_InspectorScroll.content);
            var scaleUI = scaleObj.AddComponent<InspectorVector3UI>();
            scaleUI.Name = "Scale";
            scaleUI.BindTo(obj.transform, typeof(Transform).GetMember("localScale")[0]);

            if (obj.GetComponentInChildren<JournalEntry>())
            {
                var journalObj = Instantiate(AssetManager.InspectorInput, m_InspectorScroll.content);
                var journalUI = journalObj.AddComponent<InspectorInput>();
                journalUI.Name = "Journal Text";
                journalUI.Placeholder = "Enter your custom text";
                journalUI.BindTo(obj.GetComponentInChildren<JournalEntry>(), typeof(JournalEntry).GetMember("entryKey")[0]);
            }

            if (obj.GetComponentInChildren<TeleportSource>())
            {
                var tpSourceObj = Instantiate(AssetManager.InspectorInput, m_InspectorScroll.content);
                var tpSourceUI = tpSourceObj.AddComponent<InspectorInput>();
                tpSourceUI.Name = "TP Source";
                tpSourceUI.Placeholder = "Teleport Destination Name";
                tpSourceUI.BindTo(obj.GetComponentInChildren<TeleportSource>(), typeof(TeleportSource).GetMember("destinationSetName")[0]);
            }

            if (obj.GetComponentInChildren<TeleportDestination>())
            {
                var tpDestObj = Instantiate(AssetManager.InspectorInput, m_InspectorScroll.content);
                var tpDestUI = tpDestObj.AddComponent<InspectorInput>();
                tpDestUI.Name = "TP Destination";
                tpDestUI.Placeholder = "Teleport Destination Name";
                tpDestUI.BindTo(obj.GetComponentInChildren<TeleportDestination>(), typeof(TeleportDestination).GetMember("teleportDestinationName")[0]);
                var dest = obj.GetComponentInChildren<TeleportDestination>();
                tpDestUI.OnChange = () =>
                {
                    SRSingleton<SceneContext>.Instance.TeleportNetwork.Deregister(dest);
                    SRSingleton<SceneContext>.Instance.TeleportNetwork.Register(dest);
                };
            }

            if (obj.GetComponentInChildren<GordoEat>())
            {
                ObjectManager.GetBuildObject(obj.gameObject, out var buildObject);
                var dropKeyObj = Instantiate(AssetManager.InspectorBool, m_InspectorScroll.content);
                var dropKeyUI = dropKeyObj.AddComponent<InspectorBool>();
                dropKeyUI.Name = "Drop Key?";
                dropKeyUI.BindTo(
                    () => buildObject.ExtraData.TryGetValue("dropkey", out var v) && bool.Parse(v),
                    value => buildObject.ExtraData["dropkey"] = ((bool)value).ToString());
            }
        }
    }
}
