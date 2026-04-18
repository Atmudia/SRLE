using System;
using System.Collections;
using System.Collections.Generic;
using SRLE.RuntimeGizmo;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SRLE.Components
{
    public class SRLECamera : MonoBehaviour
    {
        public static SRLECamera Instance;
        public Camera camera;
        public Camera playerCamera;
        public Transform vacuumTransform;
        public vp_FPController playerController;
        public PlayerState playerState;
        public vp_FPInput vp_FPInput;
        public Coroutine DestroyDelayedObject;
        public TransformGizmo transformGizmo;
        public AmbianceDirector ambianceDirector;
        public vp_FPPlayerEventHandler playerEvents;
        
        private Dictionary<DirectedActorSpawner, bool> m_SpawnerStates = new Dictionary<DirectedActorSpawner, bool>();
        private static Material s_SpawnerMaterial;

        public void Awake()
        {
            Instance = this;
            camera = GetComponent<Camera>();
            vp_FPInput = Object.FindObjectOfType<vp_FPInput>();
            var player = SRSingleton<SceneContext>.Instance.Player;
            playerController = player.GetComponent<vp_FPController>();
            playerCamera = player.GetComponentInChildren<Camera>();
            playerEvents = player.GetComponentInChildren<vp_FPPlayerEventHandler>();
            camera.CopyFrom(playerCamera);
            vacuumTransform = player.GetComponentInChildren<WeaponVacuum>().transform;
            transformGizmo = this.GetComponent<TransformGizmo>();
            ambianceDirector = SRSingleton<SceneContext>.Instance.AmbianceDirector;
            playerState = SRSingleton<SceneContext>.Instance.PlayerState;
        }

        public void OnEnable()
        {
            GameContext.Instance.InputDirector.input.SetInputMode(SRInput.InputMode.PAUSE, gameObject.GetInstanceID());
            SRSingleton<HudUI>.Instance.gameObject.SetActive(false);
            
            playerCamera.enabled = false;
            transform.position = playerController.transform.position;
            transform.rotation = playerController.transform.rotation;
            playerController.enabled = false;
            playerController.MotorFreeFly = true;
            foreach (Transform o in playerController.transform.GetChild(0))
            {
                o.gameObject.SetActive(false);
            }

            camera.tag = "MainCamera";
            DestroyDelayedObject = this.StartCoroutine(DestroyDelayed());
            SRSingleton<SceneContext>.Instance.TimeDirector.EnableCursor(vp_FPInput);

            m_SpawnerStates.Clear();
            foreach (var spawner in Object.FindObjectsOfType<DirectedActorSpawner>())
            {
                m_SpawnerStates[spawner] = spawner.enabled;
                spawner.enabled = false;
            }

            playerController.GetComponent<UIDetector>().enabled = false;
            ambianceDirector.ExitAllLiquid();
            playerEvents.Underwater.TryStop();
            ambianceDirector.caveTypeCounts.Clear();
            foreach (var gordoEat in ObjectManager.World.GetComponentsInChildren<GordoEat>())
            {
                gordoEat.gordoModel.gordoEatenCount = 0;
                gordoEat.OnResetEatenCount();
            }

            playerState.InGadgetMode = true;
        }

        public void OnDisable()
        {
            if (!SRSingleton<HudUI>.Instance) return;

            GameContext.Instance.InputDirector.input.SetInputMode(SRInput.InputMode.PAUSE, gameObject.GetInstanceID());
            SRSingleton<HudUI>.Instance.gameObject.SetActive(true);
            
            playerCamera.enabled = true;
            playerController.SetPosition(transform.position);
            playerController.transform.rotation = transform.rotation;
            SRSingleton<SceneContext>.Instance.TimeDirector.DisableCursor(vp_FPInput);
            foreach (Transform o in playerController.transform.GetChild(0))
            {
                o.gameObject.SetActive(true);
            }
            this.StopCoroutine(DestroyDelayedObject);
            playerController.enabled = true;
            playerController.MotorFreeFly = false;
            vacuumTransform.localScale = Vector3.one;

            foreach (var kvp in m_SpawnerStates)
            {
                if (kvp.Key) kvp.Key.enabled = kvp.Value;
            }
            m_SpawnerStates.Clear();
            
            foreach (var slimeSpawner in Resources.FindObjectsOfTypeAll<DirectedSlimeSpawner>())
            {
                var component = slimeSpawner.GetComponent<Renderer>();
                if (component)
                    component.enabled = false;
            }
            playerController.GetComponent<UIDetector>().enabled = true;
            ambianceDirector.ExitAllLiquid();
            ambianceDirector.caveTypeCounts.Clear();
            playerEvents.Underwater.TryStop();
            playerState.InGadgetMode = false;
        }

        public void SetActive(bool setActive)
        {
            gameObject.SetActive(setActive);
        }

        private IEnumerator DestroyDelayed()
        {
            while (gameObject.activeSelf)
            {
                foreach (var identifiable in Resources.FindObjectsOfTypeAll<Identifiable>())
                {
                    if (identifiable.id == Identifiable.Id.PLAYER) continue;
                    try
                    {
                        Destroyer.DestroyActor(identifiable.gameObject, "CameraActivated");
                    }
                    catch (Exception)
                    {
                        // Some identifiables cannot be destroyed mid-frame; ignore
                    }
                }

                foreach (var directedActorSpawner in Resources.FindObjectsOfTypeAll<DirectedSlimeSpawner>())
                {
                    var component = directedActorSpawner.GetComponent<Renderer>();
                    if (component)
                    {
                        component.enabled = true;
                    }
                    else
                    {
                        if (!s_SpawnerMaterial)
                            s_SpawnerMaterial = new Material(Shader.Find("Standard"));
                        var mr = directedActorSpawner.gameObject.AddComponent<MeshRenderer>();
                        mr.material = s_SpawnerMaterial;
                    }
                }

                yield return new WaitForSeconds(1);
            }
        }

        public void Update()
        {
            // ChunkManager.UpdateActiveChunks(transform.position);

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                speed = Mathf.Clamp(this.speed + 1f, 0.1f, 50f);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                speed = Mathf.Clamp(this.speed - 1f, 0.1f, 50f);
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position += -transform.right * (speed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.position += transform.right * (speed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                transform.position += transform.forward * (speed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                transform.position += -transform.forward * (speed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.C))
                    CopyPasteManager.Copy();
                else if (Input.GetKeyDown(KeyCode.V))
                    CopyPasteManager.Paste();
            }

            if (Input.GetMouseButtonDown(1))
            {
                this.lastRotation = base.transform.eulerAngles.y;
            }

            if (!Input.GetMouseButton(1))
            {
                vp_FPInput.MouseCursorForced = true;
            }
            else
            {
                vp_FPInput.MouseCursorForced = false;
                this.lastRotation += 2f * Input.GetAxis("Mouse X");
                this.rotation -= 2f * Input.GetAxis("Mouse Y");
                this.rotation = Mathf.Clamp(this.rotation, -90f, 90f);
                base.transform.eulerAngles = new Vector3(this.rotation, this.lastRotation, 0f);
            }
        }

        private float speed = 6;
        private float lastRotation = 0;
        private float rotation = 0f;

        public static string spawnObject;
        public static string teleportNode;
    }
}
