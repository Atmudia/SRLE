using System;
using System.Collections;
using System.Linq;
using Il2Cpp;
using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Input;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using JetBrains.Annotations;
using MelonLoader;
using SRLE.Patches;
// using RuntimeHandle;
// using SRLE.RuntimeGizmo;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;
using InputManager = SRLE.Utils.InputManager;

namespace SRLE.Components;

[RegisterTypeInIl2Cpp]
public class SRLECamera : MonoBehaviour
{
    public SRLECamera(IntPtr value) : base(value) { }
    public static SRLECamera Instance;
    public Camera camera;
    public SRCameraController cameraController;
    public SRCharacterController playerController;

    public GameObject playerCamera;
    public CursorLockHandler cursorLockHandler; 
    public object DestroyDelayedObject;
    public RuntimeGizmo.TransformGizmo transformGizmo;
    public InputDirector inputDirector;

    
    public void Awake()
    {
        
        Instance = this;
        camera = GetComponent<Camera>();
        cameraController = SRSingleton<SceneContext>.Instance.Camera.GetComponent<SRCameraController>();
        playerController = SRSingleton<SceneContext>.Instance.Player.GetComponent<SRCharacterController>();
        
        playerCamera = GameObject.Find("PlayerCameraKCC");
        cursorLockHandler = Resources.FindObjectsOfTypeAll<CursorLockHandler>().First();
        transformGizmo = GetComponent<RuntimeGizmo.TransformGizmo>();
        inputDirector = SRSingleton<GameContext>.Instance.InputDirector;
        
        SRSingleton<SceneContext>.Instance.Player.transform.Find("GadgetModeOverlay").GetComponent<CustomPassVolume>().enabled = false;
        
    }

    public void OnEnable()
    {
        // return;
        SRSingleton<HudUI>.Instance.gameObject.SetActive(false);

        transform.position = cameraController.Position;
        transform.rotation =  cameraController.Rotation;
        playerController.BypassGravity = true;
        playerController.ResetVelocity(false);;
        playerController.enabled = false;
        // SRSingleton<SceneContext>.Instance.PlayerState.InGadgetMode = true;
        // base.transform.position = playerCamera.transform.position;
        // this.transform.localPosition = playerCamera.transform.localPosition;
        playerCamera.SetActive(false);
        foreach (var o in playerController.transform)
        {
            o.Cast<Transform>().gameObject.SetActive(false);
        }
        camera.tag = "MainCamera";
        DestroyDelayedObject = MelonCoroutines.Start(DestroyDelayed());

    }

    public void OnDisable()
    {
        if (SRSingleton<HudUI>.Instance == null) return;
        SRSingleton<HudUI>.Instance.gameObject.SetActive(true);
        playerController.Position = transform.position;
        playerController.Rotation = transform.rotation;
        cursorLockHandler.SetEnableCursor(false);
        playerCamera.SetActive(true);
        foreach (var o in playerController.transform)
        {
            o.Cast<Transform>().gameObject.SetActive(true);
        }
        MelonCoroutines.Stop(DestroyDelayedObject);
        // inputDirector._mainGame._asset.Enable();
        // inputDirector._paused._asset.Disable();
        playerController.BypassGravity = false;
        playerController.enabled = true;

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

            foreach (var directedActorSpawner in Resources.FindObjectsOfTypeAll<DirectedSlimeSpawner>() )
            {
                var component = directedActorSpawner.GetComponent<Renderer>();
                if (component != null)
                    component.enabled = true;
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

        if (InputManager.GetMouseButtonDown(0))
        {
            // if (HierarchyUI.Instance.SearchInput.isFocused && EventSystem.current.currentSelectedGameObject != HierarchyUI.Instance.SearchInput.gameObject)
            // {
            //     HierarchyUI.Instance.SearchInput.DeactivateInputField();
            // }
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
    

    private float pitch;
    private float yaw;

    private float speed = 6;
    private Vector3 lastRotation;

    public static string spawnObject;
    public static string teleportNode;


}