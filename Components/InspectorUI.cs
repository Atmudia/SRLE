using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Dialogue.ResearchDrone;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.World.ResearchDrone;
using MelonLoader;
using SRLE.RuntimeGizmo.Objects.Commands;
using SRLE.RuntimeGizmo.UndoRedo;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace SRLE.Components
{
    [RegisterTypeInIl2Cpp]
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

            foreach (var child in m_InspectorScroll.content)
            {
                Destroy(child.Cast<Transform>().gameObject);
            }
            HierarchyUI.Instance.transform.Find("Hierarchy").gameObject.SetActive(!active);

            if (active)
            {
                var positionObj = Instantiate(AssetManager.InspectorVector3, m_InspectorScroll.content);
                var positionUI = positionObj.AddComponent<InspectorVector3UI>();
                positionUI.Name = "Position";
                positionUI.BindTo(SRLECamera.Instance.transformGizmo.targetRootsOrdered[0].transform, typeof(Transform).GetMember("position")[0]);
                positionUI.OnBeforeChange = () =>
                {
                    //UndoManager.RegisterState(new UndoHandlemove(Gizmo.Tool.Position), "Moveing objects");
                };

                var rotationObj = Instantiate(AssetManager.InspectorVector3, m_InspectorScroll.content);
                var rotationUI = rotationObj.AddComponent<InspectorVector3UI>();
                rotationUI.Name = "Rotation";
                rotationUI.BindTo(SRLECamera.Instance.transformGizmo.targetRootsOrdered[0].transform, typeof(Transform).GetMember("eulerAngles")[0]);
                rotationUI.OnBeforeChange = () =>
                {
                    // UndoManager.RegisterState(new UndoHandlemove(Gizmo.Tool.Rotate), "Moveing objects");
                };

                var scaleObj = Instantiate(AssetManager.InspectorVector3, m_InspectorScroll.content);
                var scaleUI = scaleObj.AddComponent<InspectorVector3UI>();
                scaleUI.Name = "Scale";
                scaleUI.BindTo(SRLECamera.Instance.transformGizmo.targetRootsOrdered[0].transform, typeof(Transform).GetMember("localScale")[0]);
                scaleUI.OnBeforeChange = () =>
                {
                    // UndoManager.RegisterState(new UndoHandlemove(Gizmo.Tool.Scale), "Moveing objects");
                };

                var obj = SRLECamera.Instance.transformGizmo.targetRootsOrdered[0];

                //TODO Finish this
                // var researchDroneController = obj.GetComponentInChildren<ResearchDroneController>();
                // if (researchDroneController)
                // {
                //     var tpsourceObj = Instantiate(AssetManager.InspectorInput, m_InspectorScroll.content);
                //     var tpsourceUI = tpsourceObj.AddComponent<InspectorInput>();
                //     tpsourceUI.Name = "Title of Research Drone";
                //     tpsourceUI.Placeholder = "Text";
                //     researchDroneController._researchDroneEntry = ScriptableObject.CreateInstance<ResearchDroneEntry>();
                //     researchDroneController._researchDroneEntry.title = LocalizationUtil.CreateByKey("SRLE", obj.GetComponent<BuildObject>().BuildID + ".title");
                //     tpsourceUI.BindTo(obj.GetComponentInChildren<ResearchDroneController>()._researchDroneEntry.title, typeof(ResearchDroneEntry).GetMember("title")[0]);
                //
                //     
                //     tpsourceUI = tpsourceObj.AddComponent<InspectorInput>();
                //     tpsourceUI.Name = "Page of Research Drone";
                //     tpsourceUI.Placeholder = "Text";
                //              researchDroneController._researchDroneEntry.pages = new LocalizedString[]
                //     {
                //         LocalizationUtil.CreateByKey("SRLE", obj.GetComponent<BuildObject>().BuildID + ".page")
                //     };
                //     
                //     //TODO Finish This
                // }
                return;

                if (obj.GetComponentInChildren<DirectedSlimeSpawner>())
                {
                    var  inspectorArray = Instantiate(AssetManager.InspectorArray, m_InspectorScroll.content);
                    var inspectorUI = inspectorArray.AddComponent<InspectorSpawner>(); 
                    inspectorUI.BindTo(obj.GetComponentInChildren<DirectedSlimeSpawner>(), typeof(DirectedActorSpawner).GetMember("Constraints")[0]);

                    // var  inspectorFeral = Instantiate(AssetManager.InspectorBool, m_InspectorScroll.content);
                    // var inspectorBool = inspectorArray.AddComponent<InspectorBool>(); 
                    // inspectorBool.BindTo(obj.GetComponentInChildren<DirectedSlimeSpawner>(), typeof(DirectedSlimeSpawner).GetMember("Constraints")[0]);

                    
                }
                
                // if (obj.GetComponentInChildren<TeleportDestination>() != null)
                // {
                //     var tpsourceObj = Instantiate(Globals.InspectorInput, m_InspectorScroll.content);
                //     var tpsourceUI = tpsourceObj.AddComponent<InspectorInput>();
                //     tpsourceUI.Name = "TP Destination";
                //     tpsourceUI.Placeholder = "Teleport Destination Name";
                //     tpsourceUI.BindTo(obj.GetComponentInChildren<TeleportDestination>(), typeof(TeleportDestination).GetMember("teleportDestinationName")[0]);
                //     tpsourceUI.OnChange = () =>
                //     {
                //         SRSingleton<SceneContext>.Instance.TeleportNetwork.Deregister(obj.GetComponentInChildren<TeleportDestination>());
                //         SRSingleton<SceneContext>.Instance.TeleportNetwork.Register(obj.GetComponentInChildren<TeleportDestination>());
                //     };
                // }
                // if (obj.GetComponentInChildren<JournalEntry>() != null)
                // {
                //     var tpsourceObj = Instantiate(Globals.InspectorInput, m_InspectorScroll.content);
                //     var tpsourceUI = tpsourceObj.AddComponent<InspectorInput>();
                //     tpsourceUI.Name = "Journal Text";
                //     tpsourceUI.Placeholder = "Enter your custom text";
                //     tpsourceUI.BindTo(obj.GetComponentInChildren<JournalEntry>(), typeof(JournalEntry).GetMember("entryKey")[0]);
                // }
            }
        }
    }
}
