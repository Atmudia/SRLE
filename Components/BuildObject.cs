using System;
using System.Collections.Generic;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using MelonLoader;
using SRLE.Models;
using SRLE.RuntimeGizmo.Objects;
using SRLE.Utils;
using UnityEngine;

namespace SRLE.Components
{
    [RegisterTypeInIl2Cpp]
    public class BuildObject : MonoBehaviour
    {
        public static Dictionary<uint, BuildObject> AllObjects = new Dictionary<uint, BuildObject>();

        private float m_OcclusionTime;
        private Renderer[] m_Renderers;
        private bool m_DoesRender = true;

        public IdClass ID;

        public SceneGroup SceneGroup;
        private uint m_BuildID;
        public uint BuildID
        {
            get
            {
                return m_BuildID;
            }
            set
            {
                AllObjects[value] = this;
                m_BuildID = value;
            }
        }

        private void Start()
        {
            m_Renderers = GetComponentsInChildren<Renderer>();

            // if (gameObject.GetComponentInChildren<StaticTeleporterNode>() != null)
            // {
            //     gameObject.GetComponentInChildren<StaticTeleporterNode>(). = Region;
            // }
        }

        private void Update()
        {
            m_OcclusionTime -= Time.deltaTime;
            if (m_OcclusionTime <= 0)
            {
                m_OcclusionTime = 1;
                if (m_DoesRender && Vector3.Distance(SRLECamera.Instance.transform.position, transform.position) > SaveManager.Settings.RenderDistance)
                {
                    m_DoesRender = !m_DoesRender;
                    foreach (var rend in m_Renderers)
                    {
                        rend.enabled = m_DoesRender;
                    }
                }
                else if (!m_DoesRender && Vector3.Distance(SRLECamera.Instance.transform.position, transform.position) < SaveManager.Settings.RenderDistance)
                {
                    m_DoesRender = !m_DoesRender;
                    foreach (var rend in m_Renderers)
                    {
                        rend.enabled = m_DoesRender;
                    }
                }
            }
        }

        public static BuildObject GetBuildObject(uint id)
        {
            if (AllObjects.TryGetValue(id, out BuildObject obj))
                return obj;
            return null;
        }

        // internal Dictionary<string, StringV01> GetData()
        // {
        //     Dictionary<string, StringV01> data = new Dictionary<string, StringV01>();
        //
        //     if (gameObject.GetComponentInChildren<TeleportDestination>() != null)
        //     {
        //         data["tpdestination"] = new StringV01() { value = gameObject.GetComponentInChildren<TeleportDestination>().teleportDestinationName };
        //     }
        //     if (gameObject.GetComponentInChildren<TeleportSource>() != null)
        //     {
        //         data["tpsource"] = new StringV01() { value = gameObject.GetComponentInChildren<TeleportSource>().destinationSetName };
        //     }
        //     if (gameObject.GetComponentInChildren<JournalEntry>() != null)
        //     {
        //         data["journaltext"] = new StringV01() { value = gameObject.GetComponentInChildren<JournalEntry>().entryKey };
        //     }
        //
        //     return data;
        // }
        //
        // internal void SetData(Dictionary<string, StringV01> data)
        // {
        //     if (gameObject.GetComponentInChildren<TeleportDestination>() != null && data.ContainsKey("tpdestination"))
        //     {
        //         gameObject.GetComponentInChildren<TeleportDestination>().teleportDestinationName = data["tpdestination"].value;
        //     }
        //     if (gameObject.GetComponentInChildren<TeleportSource>() != null && data.ContainsKey("tpsource"))
        //     {
        //         gameObject.GetComponentInChildren<TeleportSource>().destinationSetName = data["tpsource"].value;
        //     }
        //     if (gameObject.GetComponentInChildren<JournalEntry>() != null && data.ContainsKey("journaltext"))
        //     {
        //         gameObject.GetComponentInChildren<JournalEntry>().entryKey = data["journaltext"].value;
        //     }
        // }
        
    }
}
