﻿using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Object = UnityEngine.Object;

namespace SRLE.Utils
{
    public static class RuntimePreviewGenerator
    {
        // Source: https://github.com/MattRix/UnityDecompiled/blob/master/UnityEngine/UnityEngine/Plane.cs
        private struct ProjectionPlane
        {
            private readonly Vector3 m_Normal;
            private readonly float m_Distance;

            public ProjectionPlane(Vector3 inNormal, Vector3 inPoint)
            {
                m_Normal = Vector3.Normalize(inNormal);
                m_Distance = -Vector3.Dot(inNormal, inPoint);
            }

            public Vector3 ClosestPointOnPlane(Vector3 point)
            {
                float d = Vector3.Dot(m_Normal, point) + m_Distance;
                return point - m_Normal * d;
            }

            public float GetDistanceToPoint(Vector3 point)
            {
                float signedDistance = Vector3.Dot(m_Normal, point) + m_Distance;
                if (signedDistance < 0f)
                    signedDistance = -signedDistance;

                return signedDistance;
            }
        }

        private class CameraSetup
        {
            private Vector3 position;
            private Quaternion rotation;

            private RenderTexture targetTexture;

            private Color backgroundColor;
            private bool orthographic;
            private float orthographicSize;
            private float nearClipPlane;
            private float farClipPlane;
            private float aspect;
            private CameraClearFlags clearFlags;

            public void GetSetup(Camera camera)
            {
                position = camera.transform.position;
                rotation = camera.transform.rotation;

                targetTexture = camera.targetTexture;

                backgroundColor = camera.backgroundColor;
                orthographic = camera.orthographic;
                orthographicSize = camera.orthographicSize;
                nearClipPlane = camera.nearClipPlane;
                farClipPlane = camera.farClipPlane;
                aspect = camera.aspect;
                clearFlags = camera.clearFlags;
            }

            public void ApplySetup(Camera camera)
            {
                camera.transform.position = position;
                camera.transform.rotation = rotation;

                camera.targetTexture = targetTexture;

                camera.backgroundColor = backgroundColor;
                camera.orthographic = orthographic;
                camera.orthographicSize = orthographicSize;
                camera.nearClipPlane = nearClipPlane;
                camera.farClipPlane = farClipPlane;
                camera.aspect = aspect;
                camera.clearFlags = clearFlags;

                targetTexture = null;
            }
        }

        private const int PREVIEW_LAYER = 22;
        private static Vector3 PREVIEW_POSITION = new Vector3(-9245f, 9899f, -9356f);

        private static Camera renderCamera;
        private static CameraSetup cameraSetup = new CameraSetup();

        private static List<Renderer> renderersList = new List<Renderer>(64);
        private static List<int> layersList = new List<int>(64);

        private static float aspect;
        private static float minX, maxX, minY, maxY;
        private static float maxDistance;

        private static Vector3 boundsCenter;
        private static ProjectionPlane projectionPlaneHorizontal, projectionPlaneVertical;

        private static Camera m_internalCamera = null;
        private static Camera InternalCamera
        {
            get
            {
                if (m_internalCamera != null) return m_internalCamera;
                m_internalCamera = new GameObject("ModelPreviewGeneratorCamera").AddComponent<Camera>();
                m_internalCamera.enabled = false;
                m_internalCamera.nearClipPlane = 0.01f;
                m_internalCamera.cullingMask = 1 << PREVIEW_LAYER;
                m_internalCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
                m_internalCamera.gameObject.AddComponent<HDAdditionalCameraData>();

                return m_internalCamera;
            }
        }

        private static Light m_renderLight = null;
        private static Light RenderLight
        {
            get
            {
                if (m_renderLight != null) return m_renderLight;

                m_renderLight = new GameObject("ModelPreviewGeneratorLight").AddComponent<Light>();
                m_renderLight.cullingMask = 1 << PREVIEW_LAYER;
                m_renderLight.type = LightType.Directional;
                m_renderLight.lightShadowCasterMode = LightShadowCasterMode.Everything;
                m_renderLight.shadows = LightShadows.Soft;

                m_renderLight.bounceIntensity = 16;
                m_renderLight.color = new Color(1, 1, 1, 1);
                m_renderLight.useColorTemperature = false;
                m_renderLight.colorTemperature = 4600;
                m_renderLight.intensity = 500;
                m_renderLight.shadowNormalBias = 0;
                m_renderLight.shadowBias = 0.15f;

                HDAdditionalLightData hddData = m_renderLight.gameObject.AddComponent<HDAdditionalLightData>();
                hddData.lightUnit = LightUnit.Lux;
                hddData.m_Version = HDAdditionalLightData.Version.MoveEmissionMesh;
                hddData.angularDiameter = 0;
                hddData.enableSpotReflector = false;
                hddData.flareFalloff = 0.75f;
                hddData.flareSize = 4;
                hddData.flareTint = new Color(1, 0.8353f, 0.4588f, 1);
                hddData.intensity = 300;
                hddData.interactsWithSky = false;
                hddData.normalBias = 1;
                hddData.shadowTint = new Color(0.3302f, 0.3302f, 0.3302f, 1);
                hddData.surfaceTint = new Color(1, 0.8196f, 0.5804f, 1);
                hddData.useColorTemperature = false;

                m_renderLight.transform.rotation = Quaternion.Euler(90, 0, 0);
                m_renderLight.enabled = false;

                return m_renderLight;
            }
        }

        private static Camera m_previewRenderCamera;
        public static Camera PreviewRenderCamera
        {
            get => m_previewRenderCamera;
            set => m_previewRenderCamera = value;
        }

        private static Vector3 m_previewDirection;
        public static Vector3 PreviewDirection
        {
            get => m_previewDirection;
            set => m_previewDirection = value.normalized;
        }

        private static float m_padding;
        public static float Padding
        {
            get => m_padding;
            set => m_padding = Mathf.Clamp(value, -0.25f, 0.25f);
        }

        private static Color m_backgroundColor;
        public static Color BackgroundColor
        {
            get => m_backgroundColor;
            set => m_backgroundColor = value;
        }

        private static bool m_orthographicMode;
        public static bool OrthographicMode
        {
            get => m_orthographicMode;
            set => m_orthographicMode = value;
        }

        private static bool m_transparentBackground;
        public static bool TransparentBackground
        {
            get => m_transparentBackground;
            set => m_transparentBackground = value;
        }

        static RuntimePreviewGenerator()
        {
            PreviewRenderCamera = null;
            PreviewDirection = new Vector3(-1f, -1f, -1f);
            Padding = 0f;
            BackgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            OrthographicMode = false;
            TransparentBackground = false;
        }

        public static Texture2D GenerateMaterialPreview(Material material, PrimitiveType previewObject, int width = 64, int height = 64)
        {
            return GenerateMaterialPreviewWithShader(material, previewObject, null, null, width, height);
        }

        public static Texture2D GenerateMaterialPreviewWithShader(Material material, PrimitiveType previewPrimitive, Shader shader, string replacementTag, int width = 64, int height = 64)
        {
            GameObject previewModel = GameObject.CreatePrimitive(previewPrimitive);
            previewModel.gameObject.hideFlags = HideFlags.HideAndDontSave;
            previewModel.GetComponent<Renderer>().sharedMaterial = material;

            try
            {
                return GenerateModelPreviewWithShader(previewModel.transform, shader, replacementTag, width, height, false);
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
                //Debug.LogException(e);
            }
            finally
            {
                Object.DestroyImmediate(previewModel);
            }

            return null;
        }

        public static Texture2D GenerateModelPreview(Transform model, int width = 64, int height = 64, bool shouldCloneModel = true)
        {
            return GenerateModelPreviewWithShader(model, null, null, width, height, shouldCloneModel);
        }

        public static Texture2D GenerateModelPreviewWithShader(Transform model, Shader shader, string replacementTag, int width = 64, int height = 64, bool shouldCloneModel = false)
        {
            if (model == null || model.Equals(null))
                return null;

            foreach (Light l in UnityEngine.Object.FindObjectsOfType<Light>())
            {
                if (l != RenderLight)
                    l.cullingMask &= ~(1 << PREVIEW_LAYER);
            }

            Texture2D result = null;

            if (!model.gameObject.scene.IsValid() || !model.gameObject.scene.isLoaded)
                shouldCloneModel = true;

            Transform previewObject;
            if (shouldCloneModel)
            {
                previewObject = Object.Instantiate(model, null, false).Cast<Transform>();
                previewObject.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
            else
            {
                previewObject = model;

                layersList.Clear();
                GetLayerRecursively(previewObject);
            }

            bool isStatic = IsStatic(model);
            bool wasActive = previewObject.gameObject.activeSelf;
            Vector3 prevPos = previewObject.position;
            Quaternion prevRot = previewObject.rotation;

            try
            {
                SetupCamera();
                SetLayerRecursively(previewObject);

                if (!isStatic)
                {
                    previewObject.position = PREVIEW_POSITION;
                    previewObject.rotation = Quaternion.identity;
                }

                if (!wasActive)
                    previewObject.gameObject.SetActive(true);

                Vector3 previewDir = previewObject.rotation * m_previewDirection;

                renderersList.Clear();
                foreach (var componentsInChild in previewObject.GetComponentsInChildren<Renderer>(true))
                {
                    renderersList.Add(componentsInChild);
                }

                Bounds previewBounds = new Bounds();
                bool init = false;
                for (int i = 0; i < renderersList.Count; i++)
                {
                    if (!renderersList[i].enabled)
                        continue;

                    if (!init)
                    {
                        previewBounds = renderersList[i].bounds;
                        init = true;
                    }
                    else
                        previewBounds.Encapsulate(renderersList[i].bounds);
                }

                if (!init)
                    return null;

                boundsCenter = previewBounds.center;
                Vector3 boundsExtents = previewBounds.extents;
                Vector3 boundsSize = 2f * boundsExtents;

                aspect = (float)width / height;
                renderCamera.aspect = aspect;
                renderCamera.transform.rotation = Quaternion.LookRotation(previewDir, previewObject.up);

                float distance;
                if (m_orthographicMode)
                {
                    renderCamera.transform.position = boundsCenter;

                    minX = minY = Mathf.Infinity;
                    maxX = maxY = Mathf.NegativeInfinity;

                    Vector3 point = boundsCenter + boundsExtents;
                    ProjectBoundingBoxMinMax(point);
                    point.x -= boundsSize.x;
                    ProjectBoundingBoxMinMax(point);
                    point.y -= boundsSize.y;
                    ProjectBoundingBoxMinMax(point);
                    point.x += boundsSize.x;
                    ProjectBoundingBoxMinMax(point);
                    point.z -= boundsSize.z;
                    ProjectBoundingBoxMinMax(point);
                    point.x -= boundsSize.x;
                    ProjectBoundingBoxMinMax(point);
                    point.y += boundsSize.y;
                    ProjectBoundingBoxMinMax(point);
                    point.x += boundsSize.x;
                    ProjectBoundingBoxMinMax(point);

                    distance = boundsExtents.magnitude + 1f;
                    renderCamera.orthographicSize = (1f + m_padding * 2f) * Mathf.Max(maxY - minY, (maxX - minX) / aspect) * 0.5f;
                }
                else
                {
                    projectionPlaneHorizontal = new ProjectionPlane(renderCamera.transform.up, boundsCenter);
                    projectionPlaneVertical = new ProjectionPlane(renderCamera.transform.right, boundsCenter);

                    maxDistance = Mathf.NegativeInfinity;

                    Vector3 point = boundsCenter + boundsExtents;
                    CalculateMaxDistance(point);
                    point.x -= boundsSize.x;
                    CalculateMaxDistance(point);
                    point.y -= boundsSize.y;
                    CalculateMaxDistance(point);
                    point.x += boundsSize.x;
                    CalculateMaxDistance(point);
                    point.z -= boundsSize.z;
                    CalculateMaxDistance(point);
                    point.x -= boundsSize.x;
                    CalculateMaxDistance(point);
                    point.y += boundsSize.y;
                    CalculateMaxDistance(point);
                    point.x += boundsSize.x;
                    CalculateMaxDistance(point);

                    distance = (1f + m_padding * 2f) * Mathf.Sqrt(maxDistance);
                }

                renderCamera.transform.position = boundsCenter - previewDir * distance;
                renderCamera.farClipPlane = distance * 4f;

                RenderLight.transform.rotation = renderCamera.transform.rotation;
                RenderLight.enabled = true;

                RenderTexture temp = RenderTexture.active;
                RenderTexture renderTex = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
                RenderTexture.active = renderTex;
                if (m_transparentBackground)
                    GL.Clear(false, true, Color.clear);

                renderCamera.targetTexture = renderTex;

                if (shader == null)
                    renderCamera.Render();
                else
                    renderCamera.RenderWithShader(shader, replacementTag ?? string.Empty);

                renderCamera.targetTexture = null;

                result = new Texture2D(width, height, TextureFormat.ARGB32, false);
                result.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
                result.Apply(false, false);

                RenderLight.enabled = false;

                RenderTexture.active = temp;
                RenderTexture.ReleaseTemporary(renderTex);
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
            finally
            {
                if (shouldCloneModel)
                    Object.DestroyImmediate(previewObject.gameObject);
                else
                {
                    if (!wasActive)
                        previewObject.gameObject.SetActive(false);

                    if (!isStatic)
                    {
                        previewObject.position = prevPos;
                        previewObject.rotation = prevRot;
                    }

                    int index = 0;
                    SetLayerRecursively(previewObject, ref index);
                }

                if (renderCamera == m_previewRenderCamera)
                    cameraSetup.ApplySetup(renderCamera);
            }

            return result;
        }

        private static void SetupCamera()
        {
            if (m_previewRenderCamera != null && !m_previewRenderCamera.Equals(null))
            {
                cameraSetup.GetSetup(m_previewRenderCamera);

                renderCamera = m_previewRenderCamera;
                renderCamera.nearClipPlane = 0.01f;
            }
            else
                renderCamera = InternalCamera;

            renderCamera.backgroundColor = m_backgroundColor;
            renderCamera.orthographic = m_orthographicMode;
            renderCamera.clearFlags = m_transparentBackground ? CameraClearFlags.Depth : CameraClearFlags.Color;
        }

        private static void ProjectBoundingBoxMinMax(Vector3 point)
        {
            Vector3 localPoint = renderCamera.transform.InverseTransformPoint(point);
            if (localPoint.x < minX)
                minX = localPoint.x;
            if (localPoint.x > maxX)
                maxX = localPoint.x;
            if (localPoint.y < minY)
                minY = localPoint.y;
            if (localPoint.y > maxY)
                maxY = localPoint.y;
        }

        private static void CalculateMaxDistance(Vector3 point)
        {
            Vector3 intersectionPoint = projectionPlaneHorizontal.ClosestPointOnPlane(point);

            float horizontalDistance = projectionPlaneHorizontal.GetDistanceToPoint(point);
            float verticalDistance = projectionPlaneVertical.GetDistanceToPoint(point);

            // Credit: https://docs.unity3d.com/Manual/FrustumSizeAtDistance.html
            float halfFrustumHeight = Mathf.Max(verticalDistance, horizontalDistance / aspect);
            float distance = halfFrustumHeight / Mathf.Tan(renderCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            float distanceToCenter = (intersectionPoint - m_previewDirection * distance - boundsCenter).sqrMagnitude;
            if (distanceToCenter > maxDistance)
                maxDistance = distanceToCenter;
        }

        private static bool IsStatic(Transform obj)
        {
            if (obj.gameObject.isStatic)
                return true;

            for (int i = 0; i < obj.childCount; i++)
            {
                if (IsStatic(obj.GetChild(i)))
                    return true;
            }

            return false;
        }

        private static void SetLayerRecursively(Transform obj)
        {
            obj.gameObject.layer = PREVIEW_LAYER;
            for (int i = 0; i < obj.childCount; i++)
            {
                try
                {
                    SetLayerRecursively(obj.GetChild(i));
                }
                catch
                {
                    // ignored
                }
            }
        }

        private static void GetLayerRecursively(Transform obj)
        {
            layersList.Add(obj.gameObject.layer);
            for (int i = 0; i < obj.childCount; i++)
                GetLayerRecursively(obj.GetChild(i));
        }

        private static void SetLayerRecursively(Transform obj, ref int index)
        {
            try
            {
                obj.gameObject.layer = layersList[index++];
                for (int i = 0; i < obj.childCount; i++)
                    SetLayerRecursively(obj.GetChild(i), ref index);
            }
            catch
            {
                // ignored
            }
        }
    }
}