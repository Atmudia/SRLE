using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UniverseLib.Config;
using UniverseLib.Input;
using InputManager = UniverseLib.Input.InputManager;

namespace SRLE.Components.Gizmos
{
    public enum TransformMode
    {
        MOVE,
        ROTATE,
        SCALE,
    }

    internal class TransformController : MonoBehaviour
    {
        public static TransformController Instance;
        public Transform selected;
        public Vector3 MousePos;
        public TransformMode currentMode = TransformMode.MOVE;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            MousePos = InputManager.MousePosition;

            if (InputManager.GetKey(KeyCode.Keypad1))
            {
                currentMode = TransformMode.MOVE;
            }
            if (InputManager.GetKey(KeyCode.Keypad2))
            {
                currentMode = TransformMode.ROTATE;
            }
            if (InputManager.GetKey(KeyCode.Keypad3))
            {
                currentMode = TransformMode.SCALE;
            }
            if (InputManager.GetKey(KeyCode.Keypad4))
            {
                NEditX();
            }
            if (InputManager.GetKey(KeyCode.Keypad7))
            {
                EditX();
            }
            if (InputManager.GetKey(KeyCode.Keypad5))
            {
                NEditY();
            }
            if (InputManager.GetKey(KeyCode.Keypad8))
            {
                EditY();
            }
            if (InputManager.GetKey(KeyCode.Keypad6))
            {
                NEditZ();
            }
            if (InputManager.GetKey(KeyCode.Keypad9))
            {
                EditZ();
            }
        }
        public void EditX()
        {
            if (currentMode == TransformMode.MOVE)
            {
                selected.position = new Vector3(selected.position.x + 0.025f, selected.position.y, selected.position.z);
            }
            if (currentMode == TransformMode.ROTATE)
            {
                selected.Rotate(1, 0, 0);
            }
            if (currentMode == TransformMode.SCALE)
            {
                selected.localScale = new Vector3(selected.localScale.x + 0.02f, selected.localScale.y, selected.localScale.z);
            }
        }
        public void NEditX()
        {
            if (currentMode == TransformMode.MOVE)
            {
                selected.position = new Vector3(selected.position.x - 0.025f, selected.position.y, selected.position.z);
            }
            if (currentMode == TransformMode.ROTATE)
            {
                selected.Rotate(-1, 0, 0);
            }
            if (currentMode == TransformMode.SCALE)
            {
                selected.localScale = new Vector3(selected.localScale.x - 0.02f, selected.localScale.y, selected.localScale.z);
            }
        }
        public void EditY()
        {
            if (currentMode == TransformMode.MOVE)
            {
                selected.position = new Vector3(selected.position.x, selected.position.y + 0.025f, selected.position.z);
            }
            if (currentMode == TransformMode.ROTATE)
            {
                selected.Rotate(0, 1, 0);
            }
            if (currentMode == TransformMode.SCALE)
            {
                selected.localScale = new Vector3(selected.localScale.x, selected.localScale.y + 0.02f, selected.localScale.z);
            }
        }
        public void NEditY()
        {
            if (currentMode == TransformMode.MOVE)
            {
                selected.position = new Vector3(selected.position.x, selected.position.y - 0.025f, selected.position.z);
            }
            if (currentMode == TransformMode.ROTATE)
            {
                selected.Rotate(0, -1, 0);
            }
            if (currentMode == TransformMode.SCALE)
            {
                selected.localScale = new Vector3(selected.localScale.x, selected.localScale.y - 0.02f, selected.localScale.z);
            }
        }
        public void EditZ()
        {
            if (currentMode == TransformMode.MOVE)
            {
                selected.position = new Vector3(selected.position.x, selected.position.y, selected.position.z + 0.025f);
            }
            if (currentMode == TransformMode.ROTATE)
            {
                selected.Rotate(0, 0, 1);
            }
            if (currentMode == TransformMode.SCALE)
            {
                selected.localScale = new Vector3(selected.localScale.x, selected.localScale.y + 0.02f, selected.localScale.z);
            }
        }
        public void NEditZ()
        {
            if (currentMode == TransformMode.MOVE)
            {
                selected.position = new Vector3(selected.position.x, selected.position.y, selected.position.z - 0.025f);
            }
            if (currentMode == TransformMode.ROTATE)
            {
                selected.Rotate(0, 0, -1);
            }
            if (currentMode == TransformMode.SCALE)
            {
                selected.localScale = new Vector3(selected.localScale.x, selected.localScale.y - 0.02f, selected.localScale.z);
            }
        }
        
    }
}
