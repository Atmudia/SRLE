using System.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace SRLE.Components
{
    /**
* Component responsible for rendering the selection highlight/wireframe/bounds.
*/
    [RegisterTypeInIl2Cpp]
    public class ObjectHighlight : MonoBehaviour
    {
        public enum HighlightType
        {
            Wireframe,
            Glow,
            Bounds
        }

        private HighlightType type;

        public Material material;
        Mesh mesh;

        public void Awake()
        {
            MeshFilter mf = GetComponent<MeshFilter>();

            type = SaveManager.Settings.HighlightMethod;
            if (mf == null || mf.sharedMesh == null)
                type = HighlightType.Bounds;

            switch (type)
            {
                
                case HighlightType.Wireframe:
                    mesh = GenerateWireframe(mf.sharedMesh);
                    material = AssetManager.WireframeMaterial;
                    break;

                case HighlightType.Bounds:

                    Bounds bounds = mf != null && mf.sharedMesh != null ? mf.sharedMesh.bounds : new Bounds(Vector3.zero, Vector3.one);
                    mesh = GenerateBounds(bounds);
                    material = AssetManager.UnlitVertexColorMaterial;
                    break;

                case HighlightType.Glow:
                    mesh = mf.sharedMesh;
                    material = AssetManager.HighlightMaterial;
                    break;
            }

            mesh.RecalculateBounds();
            material.SetVector("_Center", mesh.bounds.center);

            Graphics.DrawMesh(mesh, transform.localToWorldMatrix, material, 0);
        }

        void OnDestroy()
        {
            MelonLogger.Msg("Got destroyed?");
            if (type == HighlightType.Wireframe || type == HighlightType.Bounds)
                UnityEngine.Object.Destroy(mesh);
        }

        void Update()
        {
            Graphics.DrawMesh(mesh, transform.localToWorldMatrix, material, 0);
        }

        /**
         * Generate a mesh wireframe to render over the objeect
         */
        Mesh GenerateWireframe(Mesh mesh)
        {
            if (!mesh.isReadable) return new Mesh();

            Mesh m = new Mesh();
            m.vertices = mesh.vertices;
            m.normals = mesh.normals;
            int[] tris = new int[mesh.triangles.Length * 2];
            int[] mtris = mesh.triangles;

            int c = 0;
            for (int i = 0; i < mtris.Length; i += 3)
            {
                tris[c++] = mtris[i + 0];
                tris[c++] = mtris[i + 1];
                tris[c++] = mtris[i + 1];
                tris[c++] = mtris[i + 2];
                tris[c++] = mtris[i + 2];
                tris[c++] = mtris[i + 0];
            }

            m.subMeshCount = 1;
            m.SetIndices(tris, MeshTopology.Lines, 0);

            return m;
        }

        /**
         * Generate a line segment bounds representation.
         */
        Mesh GenerateBounds(Bounds bounds)
        {
            Vector3 cen = bounds.center;
            Vector3 ext = bounds.extents + bounds.extents.normalized * .1f;

            // Draw Wireframe
            List<Vector3> v = new List<Vector3>();

            v.AddRange(DrawBoundsEdge(cen, -ext.x, -ext.y, -ext.z, .2f));
            v.AddRange(DrawBoundsEdge(cen, -ext.x, -ext.y, ext.z, .2f));
            v.AddRange(DrawBoundsEdge(cen, ext.x, -ext.y, -ext.z, .2f));
            v.AddRange(DrawBoundsEdge(cen, ext.x, -ext.y, ext.z, .2f));

            v.AddRange(DrawBoundsEdge(cen, -ext.x, ext.y, -ext.z, .2f));
            v.AddRange(DrawBoundsEdge(cen, -ext.x, ext.y, ext.z, .2f));
            v.AddRange(DrawBoundsEdge(cen, ext.x, ext.y, -ext.z, .2f));
            v.AddRange(DrawBoundsEdge(cen, ext.x, ext.y, ext.z, .2f));

            Vector2[] u = new Vector2[48];
            int[] t = new int[48];
            Color[] c = new Color[48];

            for (int i = 0; i < 48; i++)
            {
                t[i] = i;
                u[i] = Vector2.zero;
                c[i] = Color.white;
                c[i].a = .5f;
            }

            Mesh m = new Mesh();
            m.vertices = v.ToArray();
            m.subMeshCount = 1;
            m.SetIndices(t, MeshTopology.Lines, 0);

            m.uv = u;
            m.normals = v.ToArray();
            m.colors = c;

            return m;
        }

        Vector3[] DrawBoundsEdge(Vector3 center, float x, float y, float z, float size)
        {
            Vector3 p = center;
            Vector3[] v = new Vector3[6];

            p.x += x;
            p.y += y;
            p.z += z;

            v[0] = p;
            v[1] = (p + (-(x / Mathf.Abs(x)) * Vector3.right * Mathf.Min(size, Mathf.Abs(x))));

            v[2] = p;
            v[3] = (p + (-(y / Mathf.Abs(y)) * Vector3.up * Mathf.Min(size, Mathf.Abs(y))));

            v[4] = p;
            v[5] = (p + (-(z / Mathf.Abs(z)) * Vector3.forward * Mathf.Min(size, Mathf.Abs(z))));

            return v;
        }
    }
}
