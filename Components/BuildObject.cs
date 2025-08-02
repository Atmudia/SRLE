using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using SRLE.Models;
using SRML.Console.Commands;
using UnityEngine;

namespace SRLE.Components
{
    public class BuildObject : MonoBehaviour
    {
        public static Dictionary<uint, BuildObject> AllObjects = new Dictionary<uint, BuildObject>();

        private float m_OcclusionTime;
        private Renderer[] m_Renderers;
        private bool m_DoesRender = true;

        public IdClass ID;

        public RegionRegistry.RegionSetId Region;
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

        public static int GlobalIdHandler;
        public int IdHandler;

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
                if (m_DoesRender && Vector3.Distance(SRLECamera.Instance.transform.position, transform.position) > 
                    SaveManager.Settings.RenderDistance)
                {
                    m_DoesRender = !m_DoesRender;
                    foreach (var rend in m_Renderers)
                    {
                        rend.enabled = m_DoesRender;
                    }
                }
                else if (!m_DoesRender && Vector3.Distance(SRLECamera.Instance.transform.position, transform.position) < 
                         SaveManager.Settings.RenderDistance)
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

        internal Dictionary<string, string> GetData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
        
            if (gameObject.GetComponentInChildren<TeleportDestination>() != null)
            {
                data["tpdestination"] = gameObject.GetComponentInChildren<TeleportDestination>().teleportDestinationName;
            }
            if (gameObject.GetComponentInChildren<TeleportSource>() != null)
            {
                data["tpsource"] = gameObject.GetComponentInChildren<TeleportSource>().destinationSetName;
            }
            if (gameObject.GetComponentInChildren<JournalEntry>() != null)
            {
                data["journaltext"] = gameObject.GetComponentInChildren<JournalEntry>().entryKey;
            }
        
            return data;
        }
        
        internal void SetData(Dictionary<string, string> data)
        {
            if (data == null)
                return;
            if (gameObject.GetComponentInChildren<TeleportDestination>() != null && data.TryGetValue("tpdestination", out var value))
            {
                gameObject.GetComponentInChildren<TeleportDestination>().teleportDestinationName = value;
            }
            if (gameObject.GetComponentInChildren<TeleportSource>() != null && data.TryGetValue("tpsource", out value))
            {
                gameObject.GetComponentInChildren<TeleportSource>().destinationSetName = value;
            }
            if (gameObject.GetComponentInChildren<JournalEntry>() != null && data.TryGetValue("journaltext", out value))
            {
                gameObject.GetComponentInChildren<JournalEntry>().entryKey = value;
            }
        }
        
    }
}
