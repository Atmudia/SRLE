using System.Collections.Generic;
using SRLE.Components;
using UnityEngine;

namespace SRLE
{
    /// <summary>
    /// Divides the world into a 2D (XZ) grid of chunks and activates/deactivates
    /// BuildObject GameObjects based on proximity to the editor camera.
    /// This replaces the per-object renderer-distance check in BuildObject.Update.
    /// </summary>
    public static class ChunkManager
    {
        public const float ChunkSize = 64f;

        private static int ActiveRadius => Mathf.Max(1, Mathf.CeilToInt(SaveManager.Settings.RenderDistance / ChunkSize));

        private static readonly Dictionary<Vector3Int, List<BuildObject>> s_Chunks
            = new Dictionary<Vector3Int, List<BuildObject>>();

        // Sentinel value meaning "no camera chunk recorded yet — treat all chunks as active"
        private static Vector3Int s_LastCameraChunk = new Vector3Int(int.MinValue, 0, int.MinValue);
    
        private static Vector3Int WorldToChunk(Vector3 pos)
            => new Vector3Int(Mathf.FloorToInt(pos.x / ChunkSize), 0, Mathf.FloorToInt(pos.z / ChunkSize));

        /// <summary>Called from BuildObject.Start to place the object in its initial chunk.</summary>
        public static void Register(BuildObject obj)
        {
            var chunk = WorldToChunk(obj.transform.position);
            if (!s_Chunks.TryGetValue(chunk, out var list))
                s_Chunks[chunk] = list = new List<BuildObject>();
            list.Add(obj);
            bool active = IsChunkActive(chunk);
            EntryPoint.ConsoleInstance.Log($"[ChunkManager] Register '{obj.name}' at {obj.transform.position} → chunk {chunk}, active={active}, totalChunks={s_Chunks.Count}");
            obj.gameObject.SetActive(active);
        }

        /// <summary>Called from BuildObject.OnDestroy to remove the object from its chunk.</summary>
        public static void Unregister(BuildObject obj)
        {
            var chunk = WorldToChunk(obj.transform.position);
            bool found = s_Chunks.TryGetValue(chunk, out var list);
            if (found) list.Remove(obj);
            EntryPoint.ConsoleInstance.Log($"[ChunkManager] Unregister '{obj.name}' from chunk {chunk}, found={found}");
        }

        /// <summary>
        /// Called after a BuildObject's position changes so its chunk registration stays current.
        /// <paramref name="oldPosition"/> is the world position before the move.
        /// </summary>
        public static void Reregister(Transform t, Vector3 oldPosition)
        {
            var buildObject = t.GetComponent<BuildObject>()
                              ?? t.GetComponentInParent<BuildObject>()
                              ?? t.GetComponentInChildren<BuildObject>();
            if (buildObject == null)
            {
                EntryPoint.ConsoleInstance.Log($"[ChunkManager] Reregister: no BuildObject found on '{t.name}', skipping");
                return;
            }

            var oldChunk = WorldToChunk(oldPosition);
            var newChunk = WorldToChunk(t.position);
            if (oldChunk == newChunk) return;

            if (s_Chunks.TryGetValue(oldChunk, out var list))
                list.Remove(buildObject);

            if (!s_Chunks.TryGetValue(newChunk, out list))
                s_Chunks[newChunk] = list = new List<BuildObject>();
            list.Add(buildObject);
            bool active = IsChunkActive(newChunk);
            EntryPoint.ConsoleInstance.Log($"[ChunkManager] Reregister '{buildObject.name}': chunk {oldChunk} → {newChunk}, active={active}");
            buildObject.gameObject.SetActive(active);
        }

        private static bool IsChunkActive(Vector3Int chunk)
        {
            if (s_LastCameraChunk.x == int.MinValue) return true;
            return Mathf.Abs(chunk.x - s_LastCameraChunk.x) <= ActiveRadius
                && Mathf.Abs(chunk.z - s_LastCameraChunk.z) <= ActiveRadius;
        }

        /// <summary>
        /// Called every frame from SRLECamera.Update.  Does real work only when the camera
        /// crosses a chunk boundary; otherwise returns immediately.
        /// </summary>
        public static void UpdateActiveChunks(Vector3 cameraPosition)
        {
            var cameraChunk = WorldToChunk(cameraPosition);
            if (cameraChunk == s_LastCameraChunk) return;
            s_LastCameraChunk = cameraChunk;

            // Keep currently selected objects always visible regardless of distance
            HashSet<GameObject> selected = null;
            var gizmo = SRLECamera.Instance?.transformGizmo;
            if (gizmo?.targetRootsOrdered != null && gizmo.targetRootsOrdered.Count > 0)
            {
                selected = new HashSet<GameObject>();
                foreach (var t in gizmo.targetRootsOrdered)
                    if (t) selected.Add(t.gameObject);
            }

            int activated = 0, deactivated = 0, pinned = 0;
            foreach (var kvp in s_Chunks)
            {
                bool active = Mathf.Abs(kvp.Key.x - cameraChunk.x) <= ActiveRadius
                           && Mathf.Abs(kvp.Key.z - cameraChunk.z) <= ActiveRadius;

                foreach (var obj in kvp.Value)
                {
                    if (!obj) continue;
                    if (!active && selected != null && selected.Contains(obj.gameObject)) { pinned++; continue; }
                    obj.gameObject.SetActive(active);
                    if (active) activated++; else deactivated++;
                }
            }
            EntryPoint.ConsoleInstance.Log($"[ChunkManager] Camera chunk {cameraChunk}: activated={activated}, deactivated={deactivated}, pinned={pinned}, totalChunks={s_Chunks.Count}");
        }

        /// <summary>
        /// Re-activates all registered objects and resets the camera chunk.
        /// Called when exiting build mode so objects aren't left inactive.
        /// </summary>
        public static void ActivateAll()
        {
            int count = 0;
            foreach (var kvp in s_Chunks)
                foreach (var obj in kvp.Value)
                    if (obj) { obj.gameObject.SetActive(true); count++; }
            EntryPoint.ConsoleInstance.Log($"[ChunkManager] ActivateAll: re-enabled {count} objects");
            s_LastCameraChunk = new Vector3Int(int.MinValue, 0, int.MinValue);
        }

        /// <summary>
        /// Clears all chunk data.  Called when loading or creating a new level
        /// so stale registrations from the previous session don't accumulate.
        /// </summary>
        public static void Clear()
        {
            EntryPoint.ConsoleInstance.Log($"[ChunkManager] Clear: removing {s_Chunks.Count} chunks");
            s_Chunks.Clear();
            s_LastCameraChunk = new Vector3Int(int.MinValue, 0, int.MinValue);
        }
    }
}
