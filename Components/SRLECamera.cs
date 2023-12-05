using System;
using System.Collections;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using MelonLoader;
using SRLE.RuntimeGizmo;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SRLE.Components;

public class SRLECamera : MonoBehaviour
{
    public SRLECamera(IntPtr value) : base(value) { }
    public static SRLECamera Instance;
    public Camera camera;
    public SRCharacterController playerController;
    public GameObject playerCamera;
    public CursorLockHandler cursorLockHandler;
    public object DestroyDelayedObject;
    public TransformGizmo transformGizmo;
    public void Awake()
    {
        Instance = this;
        camera = GetComponent<Camera>();
        playerController = SRSingleton<SceneContext>.Instance.Player.GetComponent<SRCharacterController>();
        playerCamera = GameObject.Find("PlayerCameraKCC");
        cursorLockHandler = Resources.FindObjectsOfTypeAll<CursorLockHandler>().First();
        transformGizmo = GetComponent<TransformGizmo>();
    }

    public void OnEnable()
    {
        SRSingleton<HudUI>.Instance.gameObject.SetActive(false); 
        SRSingleton<SceneContext>.Instance.PlayerState.InGadgetMode = true;
        base.transform.position = playerCamera.transform.position;
        this.transform.localPosition = playerCamera.transform.localPosition;
        playerCamera.SetActive(false);
        foreach (var o in playerController.transform)
        {
            o.Cast<Transform>().gameObject.SetActive(false);
        }

        
        camera.tag = "MainCamera";
        /*foreach (var rootGameObjects in EntryPoint.GetAllScenes().Where(x => !x.name.EndsWith("Core")).SelectMany(x => x.GetRootGameObjects()))
        {
            foreach (var directedActorSpawner in rootGameObjects.GetComponentsInChildren<DirectedActorSpawner>())
            {
                directedActorSpawner.enabled = !directedActorSpawner.enabled;
            }
        }
        */
        
        DestroyDelayedObject = MelonCoroutines.Start(DestroyDelayed());

    }

    public void OnDisable()
    {
        if (SRSingleton<HudUI>.Instance == null) return;
        SRSingleton<HudUI>.Instance.gameObject.SetActive(true); 
        SRSingleton<SceneContext>.Instance.PlayerState.InGadgetMode = false;
        playerController.Position = transform.position;
        playerController.Rotation = transform.rotation;
        cursorLockHandler.SetEnableCursor(false);
        playerCamera.SetActive(true);
        foreach (var o in playerController.transform)
        {
            o.Cast<Transform>().gameObject.SetActive(true);
        }
        MelonCoroutines.Stop(DestroyDelayedObject);

        /*foreach (var rootGameObjects in SRLEConverterUtils.GetAllScenes().Where(x => !x.name.EndsWith("Core")).SelectMany(x => x.GetRootGameObjects()))
        {
            foreach (var directedActorSpawner in rootGameObjects.GetComponentsInChildren<DirectedActorSpawner>())
            {
                directedActorSpawner.enabled = !directedActorSpawner.enabled;
            }
        }
        */
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
                if (identifiable.identType == SRSingleton<SceneContext>.Instance.Player.GetComponent<Identifiable>().identType) continue;
                try
                {
                    Destroyer.DestroyActor(identifiable.gameObject, "CameraActivated");
                }
                catch
                {
                    // 
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void Update()
    {
        
        

        if (InputManager.MouseScrollDelta.y > 0f) // forward
        {
            speed = Mathf.Clamp(this.speed + 1f, 0.1f, 50f);
        }
        else if (InputManager.MouseScrollDelta.y < 0f)
        {
            speed = Mathf.Clamp(this.speed - 1f, 0.1f, 50f);
        }

        if (InputManager.GetKey(Key.A) || InputManager.GetKey(Key.LeftArrow))
        {
            transform.position += -transform.right * (speed * Time.deltaTime);
        }

        if (InputManager.GetKey(Key.D) || InputManager.GetKey(Key.RightArrow))
        {
            transform.position += transform.right * (speed * Time.deltaTime);
        }

        if (InputManager.GetKey(Key.W) || InputManager.GetKey(Key.UpArrow))
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }

        if (InputManager.GetKey(Key.S) || InputManager.GetKey(Key.DownArrow))
        {
            transform.position += -transform.forward * (speed * Time.deltaTime);
        }

        if (InputManager.GetMouseButtonDown(1))
        {
            //this.lastRotation = base.transform.eulerAngles.y;
        }
        if (!InputManager.GetMouseButton(1))
        {
            cursorLockHandler.SetEnableCursor(true);
        }
        else
        {
         
            
            cursorLockHandler.SetEnableCursor(false);
            /*pitch = Mathf.Clamp(pitch - Mouse.current.delta.y.ReadValue()  * 30, -90f, 90f);
            yaw += Mouse.current.delta.x.ReadValue() * 30f;
            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
            */

            Vector2 mouseInput = Mouse.current.delta.ReadValue();
            pitch = Math.Clamp(pitch - mouseInput.y * 0.5f, -90f, 90f);
            yaw += mouseInput.x * 0.5f;
            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);

        }
        lastRotation = InputManager.MousePosition;


    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(170f, 90f, 150f, 25f), "Quit"))
        {
            SRSingleton<SceneContext>.Instance.PauseMenuDirector.Quit();
        }

        GUI.Label(new Rect(15f, 125f, 150f, 25f), "Spawn object with id: ");
        spawnObject = GUI.TextField(new Rect(125f, 110f + 35f * 1, 200f, 25f), spawnObject);
        if (GUI.Button(new Rect(170f, 125f, 150f, 25f), "Spawn object"))
        {
            SRLEObjectManager.SpawnObject(Convert.ToUInt32(spawnObject));
            //SRLEManager.SpawnObjectFromId();
        }

        if (transformGizmo == null || transformGizmo.mainTargetRoot == null) return;
        var buildObject = transformGizmo.mainTargetRoot.GetComponent<BuildObjectId>().buildObject;
        var staticTeleporterNode = transformGizmo.mainTargetRoot.GetComponentInChildren<TeleporterNode>();
        if (staticTeleporterNode != null)
        {
            GUI.Label(new Rect(15f, 215f, 100f, 25f), "Teleport:");
            teleportNode = GUI.TextField(new Rect(125f, 215f, 200f, 25f), teleportNode);

            if (GUI.Button(new Rect(170f, 275f, 150f, 25f), "Teleport"))
            {
                staticTeleporterNode._hasDestination = false;
                //staticTeleporterNode.DeregisterSelf();
                buildObject.Properties["TeleporterNode"] = teleportNode;
                var nodeId = staticTeleporterNode.NodeId;
                staticTeleporterNode.Awake();
                staticTeleporterNode.RegisterSelf();
                //staticTeleporterNode.nodeDefinition
            }
        }
        
       
    }


    private float pitch;
    private float yaw;

    private float speed = 6;
    private Vector3 lastRotation;

    public static string spawnObject;
    public static string teleportNode;


}