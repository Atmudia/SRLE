using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using SRLE.Models;
using UnityEngine;

namespace SRLE.Components
{
    public class BuildObject : MonoBehaviour
    {
        public IdClass ID;

        public RegionRegistry.RegionSetId Region;

        // Stores custom properties that have no matching game component field
        public Dictionary<string, string> ExtraData = new Dictionary<string, string>();

        public static int GlobalIdHandler;
        public int IdHandler;

        private void Start()
        {
            ChunkManager.Register(this);
        }

        private void OnDestroy()
        {
            ChunkManager.Unregister(this);
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
            
            foreach (var kvp in ExtraData)
                data[kvp.Key] = kvp.Value;
            
            return data;
        }

        internal void SetData(Dictionary<string, string> data)
        {
            if (data == null)
                return;
            if (gameObject.GetComponentInChildren<TeleportDestination>() && data.TryGetValue("tpdestination", out var value))
            {
                gameObject.GetComponentInChildren<TeleportDestination>().teleportDestinationName = value;
            }
            if (gameObject.GetComponentInChildren<TeleportSource>() && data.TryGetValue("tpsource", out value))
            {
                gameObject.GetComponentInChildren<TeleportSource>().destinationSetName = value;
            }
            if (gameObject.GetComponentInChildren<JournalEntry>() && data.TryGetValue("journaltext", out value))
            {
                gameObject.GetComponentInChildren<JournalEntry>().entryKey = value;
            }

            var knownKeys = new System.Collections.Generic.HashSet<string> { "tpdestination", "tpsource", "journaltext" };
            foreach (var kvp in data)
                if (!knownKeys.Contains(kvp.Key))
                    ExtraData[kvp.Key] = kvp.Value;
        }
    }
}
