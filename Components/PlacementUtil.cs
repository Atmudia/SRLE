using SRLE.Components.Gizmos;
using UnityEngine;
using InputManager = UniverseLib.Input.InputManager;

namespace SRLE.Components
{
    public class CenterPositionGetter : MonoBehaviour
    {
        public Vector3 pos; // Reference to the object you want to center
        public static CenterPositionGetter Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            Vector3 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                pos = hit.point;
            }
        }
    }
    public class Selector : MonoBehaviour
    {
        public static Selector Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (InputManager.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(TransformController.Instance.MousePos);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    TransformController.Instance.selected = hit.collider.transform;
                }
            }
        }
    }
}
