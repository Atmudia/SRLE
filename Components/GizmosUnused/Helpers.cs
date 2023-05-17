using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SRLE.Components;

namespace SRLE.Components.GizmosUnused
{
    public static class Geometry
    {
        public static float LinePlaneDistance(Vector3 linePoint, Vector3 lineVec, Vector3 planePoint, Vector3 planeNormal)
        {
            //calculate the distance between the linePoint and the line-plane intersection point
            float dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
            float dotDenominator = Vector3.Dot(lineVec, planeNormal);

            //line and plane are not parallel
            if (dotDenominator != 0f)
            {
                return dotNumerator / dotDenominator;
            }

            return 0;
        }

        //Note that the line is infinite, this is not a line-segment plane intersect
        public static Vector3 LinePlaneIntersect(Vector3 linePoint, Vector3 lineVec, Vector3 planePoint, Vector3 planeNormal)
        {
            float distance = LinePlaneDistance(linePoint, lineVec, planePoint, planeNormal);

            //line and plane are not parallel
            if (distance != 0f)
            {
                return linePoint + (lineVec * distance);
            }

            return Vector3.zero;
        }

        //Returns 2 points since on line 1 there will be a closest point to line 2, and on line 2 there will be a closest point to line 1.
        public static IntersectPoints ClosestPointsOnTwoLines(Vector3 point1, Vector3 point1Direction, Vector3 point2, Vector3 point2Direction)
        {
            IntersectPoints intersections = new IntersectPoints();

            //I dont think we need to normalize
            //point1Direction.Normalize();
            //point2Direction.Normalize();

            float a = Vector3.Dot(point1Direction, point1Direction);
            float b = Vector3.Dot(point1Direction, point2Direction);
            float e = Vector3.Dot(point2Direction, point2Direction);

            float d = a * e - b * b;

            //This is a check if parallel, howeverm since we are not normalizing the directions, it seems even if they are parallel they will not == 0
            //so they will get past this point, but its seems to be alright since it seems to still give a correct point (although a point very fary away).
            //Also, if they are parallel and we dont normalize, the deciding point seems randomly choses on the lines, which while is still correct,
            //our ClosestPointsOnTwoLineSegments gets undesireable results when on corners. (for example when using it in our ClosestPointOnTriangleToLine).
            if (d != 0f)
            {
                Vector3 r = point1 - point2;
                float c = Vector3.Dot(point1Direction, r);
                float f = Vector3.Dot(point2Direction, r);

                float s = (b * f - c * e) / d;
                float t = (a * f - c * b) / d;

                intersections.first = point1 + point1Direction * s;
                intersections.second = point2 + point2Direction * t;
            }
            else
            {
                //Lines are parallel, select any points next to eachother
                intersections.first = point1;
                intersections.second = point2 + Vector3.Project(point1 - point2, point2Direction);
            }

            return intersections;
        }

        public static IntersectPoints ClosestPointsOnSegmentToLine(Vector3 segment0, Vector3 segment1, Vector3 linePoint, Vector3 lineDirection)
        {
            IntersectPoints closests = ClosestPointsOnTwoLines(segment0, segment1 - segment0, linePoint, lineDirection);
            closests.first = ClampToSegment(closests.first, segment0, segment1);

            return closests;
        }

        //Assumes the point is already on the line somewhere
        public static Vector3 ClampToSegment(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            Vector3 lineDirection = linePoint2 - linePoint1;

            if (!ExtVector3.IsInDirection(point - linePoint1, lineDirection))
            {
                point = linePoint1;
            }
            else if (ExtVector3.IsInDirection(point - linePoint2, lineDirection))
            {
                point = linePoint2;
            }

            return point;
        }
    }

    public static class ExtVector3
    {
        public static float MagnitudeInDirection(Vector3 vector, Vector3 direction, bool normalizeParameters = true)
        {
            if (normalizeParameters) direction.Normalize();
            return Vector3.Dot(vector, direction);
        }

        public static Vector3 Abs(this Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static bool IsParallel(Vector3 direction, Vector3 otherDirection, float precision = .0001f)
        {
            return Vector3.Cross(direction, otherDirection).sqrMagnitude < precision;
        }

        public static bool IsInDirection(Vector3 direction, Vector3 otherDirection)
        {
            return Vector3.Dot(direction, otherDirection) > 0f;
        }
    }

    public static class ExtMathf
    {
        public static float Squared(this float value)
        {
            return value * value;
        }

        public static float SafeDivide(float value, float divider)
        {
            if (divider == 0) return 0;
            return value / divider;
        }
    }

    public static class ExtTransformType
    {
        public static bool TransformTypeContains(this TransformType mainType, TransformType type, TransformSpace space)
        {
            if (type == mainType) return true;

            if (mainType == TransformType.All)
            {
                if (type == TransformType.Move) return true;
                else if (type == TransformType.Rotate) return true;
                //else if(type == TransformType.RectTool) return false;
                else if (type == TransformType.Scale && space == TransformSpace.Local) return true;
            }

            return false;
        }
    }

    public static class ExtTransform
    {
        //This acts as if you are using a parent transform as your new pivot and transforming that parent instead of the child.
        //So instead of creating a gameobject and parenting "target" to it and translating only the parent gameobject, we can use this method.
        public static void SetScaleFrom(this Transform target, Vector3 worldPivot, Vector3 newScale)
        {
            Vector3 localOffset = target.InverseTransformPoint(worldPivot);

            Vector3 localScale = target.localScale;
            Vector3 scaleRatio = new Vector3(ExtMathf.SafeDivide(newScale.x, localScale.x), ExtMathf.SafeDivide(newScale.y, localScale.y), ExtMathf.SafeDivide(newScale.z, localScale.z));
            Vector3 scaledLocalOffset = Vector3.Scale(localOffset, scaleRatio);

            Vector3 newPosition = target.TransformPoint(localOffset - scaledLocalOffset);

            target.localScale = newScale;
            target.position = newPosition;
        }

        //This acts as if you are scaling based on a point that is offset from the actual pivot.
        //It gives results similar to when you scale an object in the unity editor when in Center mode instead of Pivot mode.
        //The Center was an offset from the actual Pivot.
        public static void SetScaleFromOffset(this Transform target, Vector3 worldPivot, Vector3 newScale)
        {
            //Seemed to work, except when under a parent that has a non uniform scale and rotation it was a bit off.
            //This might be due to transform.lossyScale not being accurate under those conditions, or possibly something else is wrong...
            //Maybe things can work if we can find a way to convert the "newPosition = ..." line to use Matrix4x4 for possibly more scale accuracy.
            //However, I have tried and tried and have no idea how to do that kind of math =/
            //Seems like unity editor also has some inaccuracies with skewed scales, such as scaling little by little compared to scaling one large scale.
            //
            //Will mess up or give undesired results if the target.localScale or target.lossyScale has any set to 0.
            //Unity editor doesnt even allow you to scale an axis when it is set to 0.

            Vector3 localOffset = target.InverseTransformPoint(worldPivot);

            Vector3 localScale = target.localScale;
            Vector3 scaleRatio = new Vector3(ExtMathf.SafeDivide(newScale.x, localScale.x), ExtMathf.SafeDivide(newScale.y, localScale.y), ExtMathf.SafeDivide(newScale.z, localScale.z));
            Vector3 scaledLocalOffset = Vector3.Scale(localOffset, scaleRatio);

            Vector3 newPosition = target.rotation * Vector3.Scale(localOffset - scaledLocalOffset, target.lossyScale) + target.position;

            target.localScale = newScale;
            target.position = newPosition;
        }

        public static Vector3 GetCenter(this Transform transform, CenterType centerType)
        {
            if (centerType == CenterType.Solo)
            {
                Renderer renderer = transform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    return renderer.bounds.center;
                }
                else
                {
                    return transform.position;
                }
            }
            else if (centerType == CenterType.All)
            {
                Bounds totalBounds = new Bounds(transform.position, Vector3.zero);
                GetCenterAll(transform, ref totalBounds);
                return totalBounds.center;
            }

            return transform.position;
        }
        static void GetCenterAll(this Transform transform, ref Bounds currentTotalBounds)
        {
            Renderer renderer = transform.GetComponent<Renderer>();
            if (renderer != null)
            {
                currentTotalBounds.Encapsulate(renderer.bounds);
            }
            else
            {
                currentTotalBounds.Encapsulate(transform.position);
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetCenterAll(ref currentTotalBounds);
            }
        }
    }
}
