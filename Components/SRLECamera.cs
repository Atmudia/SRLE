using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.UI;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using UniverseLib.Config;
using UniverseLib.Input;
using InputManager = UniverseLib.Input.InputManager;

namespace SRLE.Components;

public class SRLECamera : MonoBehaviour
{
    public static SRLECamera Instance;
    public Camera camera;
    public GameObject playerController;
    public GameObject playerCamera;
    public void Awake()
    {
        Instance = this;
        camera = GetComponent<Camera>();
        playerController = SRSingleton<SceneContext>.Instance.Player;
        playerCamera = playerController.scene.GetRootGameObjects().FirstOrDefault(x => x.name.Equals("PlayerCameraKCC"));
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
        foreach (var rootGameObjects in SRLEConverterUtils.GetAllScenes().Where(x => !x.name.EndsWith("Core")).SelectMany(x => x.GetRootGameObjects()))
        {
            foreach (var directedActorSpawner in rootGameObjects.GetComponentsInChildren<DirectedActorSpawner>())
            {
                directedActorSpawner.enabled = !directedActorSpawner.enabled;
            }
        }
        MelonCoroutines.Start(DestroyDelayed());

    }

    public void OnDisable()
    {
        SRSingleton<HudUI>.Instance.gameObject.SetActive(true); 
        SRSingleton<SceneContext>.Instance.PlayerState.InGadgetMode = false;
        playerController.GetComponent<SRCharacterController>().Position = transform.position;
        playerController.GetComponent<SRCharacterController>().Rotation = transform.localRotation;

        //playerController.transform.localPosition = this.transform.localPosition;
        playerCamera.SetActive(true);
        foreach (var o in playerController.transform)
        {
            o.Cast<Transform>().gameObject.SetActive(true);
        }

        foreach (var rootGameObjects in SRLEConverterUtils.GetAllScenes().Where(x => !x.name.EndsWith("Core")).SelectMany(x => x.GetRootGameObjects()))
        {
            foreach (var directedActorSpawner in rootGameObjects.GetComponentsInChildren<DirectedActorSpawner>())
            {
                directedActorSpawner.enabled = !directedActorSpawner.enabled;
            }
        }
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

        if (InputManager.GetKey(KeyCode.A) || InputManager.GetKey(KeyCode.LeftArrow))
        {
            transform.position += -transform.right * (speed * Time.deltaTime);
        }

        if (InputManager.GetKey(KeyCode.D) || InputManager.GetKey(KeyCode.RightArrow))
        {
            transform.position += transform.right * (speed * Time.deltaTime);
        }

        if (InputManager.GetKey(KeyCode.W) || InputManager.GetKey(KeyCode.UpArrow))
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }

        if (InputManager.GetKey(KeyCode.S) || InputManager.GetKey(KeyCode.DownArrow))
        {
            transform.position += -transform.forward * (speed * Time.deltaTime);
        }

        if (InputManager.GetMouseButtonDown(1))
        {
            this.lastRotation = base.transform.eulerAngles.y;
        }
        if (!InputManager.GetMouseButton(1))
        {
            
            //_vpFpInput.MouseCursorForced = true;

        }
        else
        {
            float newRotationX = transform.localEulerAngles.y + Mouse.current.delta.x.ReadValue() * 0.5f;
            float newRotationY = transform.localEulerAngles.x - Mouse.current.delta.y.ReadValue() * 0.5f;
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
            /*
            this.lastRotation += 0.5f * Mouse.current.delta.x.ReadValue();
            this.rotation -= 2f * Mouse.current.delta.y.ReadValue();
            this.rotation = Mathf.Clamp(this.rotation, -90f, 90f);
            base.transform.eulerAngles = new Vector3(this.rotation, this.lastRotation, 0f);
            */
        }

    }

    
    private float rotation = 0f;

    private float speed = 6;
    private float lastRotation = 0f;

}