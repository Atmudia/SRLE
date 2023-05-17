using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRLE.Components.GizmosUnused
{
    public enum TransformSpace { Global, Local }
    public enum TransformType { Move, Rotate, Scale /*, RectTool*/, All }
    public enum TransformPivot { Pivot, Center }
    public enum Axis { None, X, Y, Z, Any }

    //CenterType.All is the center of the current object mesh or pivot if not mesh and all its childrens mesh or pivot if no mesh.
    //	CenterType.All might give different results than unity I think because unity only counts empty gameobjects a little bit, as if they have less weight.
    //CenterType.Solo is the center of the current objects mesh or pivot if no mesh.
    //Unity seems to use colliders first to use to find how much weight the object has or something to decide how much it effects the center,
    //but for now we only look at the Renderer.bounds.center, so expect some differences between unity.
    public enum CenterType { All, Solo }

    //ScaleType.FromPoint acts as if you are using a parent transform as your new pivot and transforming that parent instead of the child.
    //ScaleType.FromPointOffset acts as if you are scaling based on a point that is offset from the actual pivot. Its similar to unity editor scaling in Center pivot mode (though a little inaccurate if object is skewed)
    public enum ScaleType { FromPoint, FromPointOffset }

    public struct IntersectPoints
    {
        public Vector3 first;
        public Vector3 second;

        public IntersectPoints(Vector3 first, Vector3 second)
        {
            this.first = first;
            this.second = second;
        }
    }

    public struct Square
    {
        public Vector3 bottomLeft;
        public Vector3 bottomRight;
        public Vector3 topLeft;
        public Vector3 topRight;

        public Vector3 this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.bottomLeft;
                    case 1:
                        return this.topLeft;
                    case 2:
                        return this.topRight;
                    case 3:
                        return this.bottomRight;
                    case 4:
                        return this.bottomLeft; //so we wrap around back to start
                    default:
                        return Vector3.zero;
                }
            }
        }
    }

    public class TargetInfo
    {
        public Vector3 centerPivotPoint;

        public Vector3 previousPosition;
    }

    public class AxisVectors
    {
        public List<Vector3> x = new List<Vector3>();
        public List<Vector3> y = new List<Vector3>();
        public List<Vector3> z = new List<Vector3>();
        public List<Vector3> all = new List<Vector3>();

        public void Add(AxisVectors axisVectors)
        {
            x.AddRange(axisVectors.x);
            y.AddRange(axisVectors.y);
            z.AddRange(axisVectors.z);
            all.AddRange(axisVectors.all);
        }

        public void Clear()
        {
            x.Clear();
            y.Clear();
            z.Clear();
            all.Clear();
        }
    }

    public struct AxisInfo
    {
        public Vector3 pivot;
        public Vector3 xDirection;
        public Vector3 yDirection;
        public Vector3 zDirection;

        public void Set(Transform target, Vector3 pivot, TransformSpace space)
        {
            if (space == TransformSpace.Global)
            {
                xDirection = Vector3.right;
                yDirection = Vector3.up;
                zDirection = Vector3.forward;
            }
            else if (space == TransformSpace.Local)
            {
                xDirection = target.right;
                yDirection = target.up;
                zDirection = target.forward;
            }

            this.pivot = pivot;
        }

        public Vector3 GetXAxisEnd(float size)
        {
            return pivot + (xDirection * size);
        }
        public Vector3 GetYAxisEnd(float size)
        {
            return pivot + (yDirection * size);
        }
        public Vector3 GetZAxisEnd(float size)
        {
            return pivot + (zDirection * size);
        }
        public Vector3 GetAxisEnd(Vector3 direction, float size)
        {
            return pivot + (direction * size);
        }
    }
}
