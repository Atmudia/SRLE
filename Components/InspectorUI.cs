
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

            //SetActive(false);
        }

        public void SetActive(bool active)
        {
            m_Gameobject.SetActive(active);

            foreach (Transform child in m_InspectorScroll.content)
            {
                Destroy(child.gameObject);
            }
            HierarchyUI.Instance.transform.Find("Hierarchy").gameObject.SetActive(!active);
            
            if (active)
            {
                var obj = SRLECamera.Instance.transformGizmo.targetRootsOrdered[0];
                var positionObj = Instantiate(AssetManager.InspectorVector3, m_InspectorScroll.content);
                var positionUI = positionObj.AddComponent<InspectorVector3UI>();
                positionUI.Name = "Position";
                positionUI.BindTo(obj.transform, typeof(Transform).GetMember("position")[0]);
                positionUI.OnBeforeChange = () =>
                {
                    //UndoManager.RegisterState(new UndoHandlemove(Gizmo.Tool.Position), "Moveing objects");
                };
            
                var rotationObj = Instantiate(AssetManager.InspectorVector3, m_InspectorScroll.content);
                var rotationUI = rotationObj.AddComponent<InspectorVector3UI>();
                rotationUI.Name = "Rotation";
                rotationUI.BindTo(obj.transform, typeof(Transform).GetMember("localEulerAngles")[0]);
                rotationUI.OnBeforeChange = () =>
                {
                    // UndoManager.RegisterState(new UndoHandlemove(Gizmo.Tool.Rotate), "Moveing objects");
                };
            
                var scaleObj = Instantiate(AssetManager.InspectorVector3, m_InspectorScroll.content);
                var scaleUI = scaleObj.AddComponent<InspectorVector3UI>();
                scaleUI.Name = "Scale";
                scaleUI.BindTo(obj.transform, typeof(Transform).GetMember("localScale")[0]);
                scaleUI.OnBeforeChange = () =>
                {
                    // UndoManager.RegisterState(new UndoHandlemove(Gizmo.Tool.Scale), "Moveing objects");
                };
                if (obj.GetComponentInChildren<JournalEntry>() != null)
                {
                    var tpsourceObj = Instantiate(AssetManager.InspectorInput, m_InspectorScroll.content);
                    var tpsourceUI = tpsourceObj.AddComponent<InspectorInput>();
                    tpsourceUI.Name = "Journal Text";
                    tpsourceUI.Placeholder = "Enter your custom text";
                    tpsourceUI.BindTo(obj.GetComponentInChildren<JournalEntry>(), typeof(JournalEntry).GetMember("entryKey")[0]);
                }
                

                if (obj.GetComponentInChildren<TeleportSource>() != null)
                {
                    var tpsourceObj = Instantiate(AssetManager.InspectorInput, m_InspectorScroll.content);
                    var tpsourceUI = tpsourceObj.AddComponent<InspectorInput>();
                    tpsourceUI.Name = "TP Source";
                    tpsourceUI.Placeholder = "Teleport Destination Name";
                    tpsourceUI.BindTo(obj.GetComponentInChildren<TeleportSource>(), typeof(TeleportSource).GetMember("destinationSetName")[0]);
                }
                if (obj.GetComponentInChildren<TeleportDestination>() != null)
                {
                    var tpsourceObj = Instantiate(AssetManager.InspectorInput, m_InspectorScroll.content);
                    var tpsourceUI = tpsourceObj.AddComponent<InspectorInput>();
                    tpsourceUI.Name = "TP Destination";
                    tpsourceUI.Placeholder = "Teleport Destination Name";
                    tpsourceUI.BindTo(obj.GetComponentInChildren<TeleportDestination>(), typeof(TeleportDestination).GetMember("teleportDestinationName")[0]);
                    tpsourceUI.OnChange = () =>
                    {
                        SRSingleton<SceneContext>.Instance.TeleportNetwork.Deregister(obj.GetComponentInChildren<TeleportDestination>());
                        SRSingleton<SceneContext>.Instance.TeleportNetwork.Register(obj.GetComponentInChildren<TeleportDestination>());
                    };
                }

            }
        }
    }
}
