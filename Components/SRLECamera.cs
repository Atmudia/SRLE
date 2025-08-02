using System;
using System.Collections;
using System.Linq;
using SRLE.RuntimeGizmo;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        public vp_FPInput vp_FPInput; 
        public Coroutine DestroyDelayedObject;
        public TransformGizmo transformGizmo;
        public AmbianceDirector ambianceDirector;
        public vp_FPPlayerEventHandler playerEvents;

    
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

        }

        public void OnEnable()
        {
            // return;
            SRSingleton<HudUI>.Instance.gameObject.SetActive(false);
            playerCamera.enabled = false;
            transform.position = playerController.transform.position;
            transform.rotation =  playerController.transform.rotation;
            playerController.enabled = false;
            playerController.MotorFreeFly = true;
            foreach (Transform o in playerController.transform.GetChild(0))
            {
                o.gameObject.SetActive(false);
            }
            
            camera.tag = "MainCamera";
            DestroyDelayedObject = this.StartCoroutine(DestroyDelayed());
            SRSingleton<SceneContext>.Instance.TimeDirector.EnableCursor(vp_FPInput);
            foreach (var directedActorSpawner in Object.FindObjectsOfType<DirectedActorSpawner>())
            {
                directedActorSpawner.enabled = !directedActorSpawner.enabled;
            }

            playerController.GetComponent<UIDetector>().enabled = false;
            ambianceDirector.ExitAllLiquid();
            playerEvents.Underwater.TryStop();


        }

        public void OnDisable()
        {
            if (!SRSingleton<HudUI>.Instance) return;
            SRSingleton<HudUI>.Instance.gameObject.SetActive(true);
            playerCamera.enabled = true;
            playerController.SetPosition(transform.position);
            playerController.transform.rotation =  transform.rotation;
            SRSingleton<SceneContext>.Instance.TimeDirector.DisableCursor(vp_FPInput);
            foreach (Transform o in playerController.transform.GetChild(0))
            {
                o.gameObject.SetActive(true);
            }
            this.StopCoroutine(DestroyDelayedObject);
            playerController.enabled = true;
            playerController.MotorFreeFly = false;
            vacuumTransform.localScale = Vector3.one;


            foreach (var directedActorSpawner in Object.FindObjectsOfType<DirectedActorSpawner>())
            {
                directedActorSpawner.enabled = !directedActorSpawner.enabled;
            }

            playerController.GetComponent<UIDetector>().enabled = true;
            ambianceDirector.ExitAllLiquid();
            playerEvents.Underwater.TryStop();


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
                    catch
                    {
                    
                        // 
                    }
                }

                foreach (var directedActorSpawner in Resources.FindObjectsOfTypeAll<DirectedSlimeSpawner>() )
                {
                    var component = directedActorSpawner.GetComponent<Renderer>();
                    if (component)
                        component.enabled = true;
                }
            
                yield return new WaitForSeconds(1);
            }
        
        
        }

        public void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                speed = Mathf.Clamp(this.speed + 1f, 0.1f, 50f);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f )
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
        
            
    

        private float pitch;
        private float yaw;

        private float speed = 6;
        private float lastRotation = 0;
        private float rotation = 0f;

        public static string spawnObject;
        public static string teleportNode;


    }
}