using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SRLE.Components
{
    public class SRLECamera : SRSingleton<SRLECamera>
    {
        public Camera camera;
        public GameObject player;
        public List<GameObject> listOfUIs = new List<GameObject>();

        // Token: 0x040000DF RID: 223

        public override void Awake()
        {
            base.Awake();
            camera = GetComponent<Camera>();
            player = SRSingleton<SceneContext>.Instance.Player;
            _vpFpInput = Object.FindObjectOfType<vp_FPInput>();
            
        }

        public void OnDisable()
        {
            player.GetComponent<vp_FPController>().MotorFreeFly = false;
            foreach (Transform child in player.transform.GetChild(0))
            {
                child.gameObject.SetActive(true);
            }

            foreach (DirectedActorSpawner directedActorSpawner in
                Resources.FindObjectsOfTypeAll<DirectedActorSpawner>())
            {
                directedActorSpawner.enabled = true;
            }

            SRSingleton<HudUI>.Instance.gameObject.SetActive(true);
            SRSingleton<SceneContext>.Instance.PlayerState.InGadgetMode = false;
            foreach (var objects in listOfUIs)
            {
                objects.SetActive(false);
            }
            SRSingleton<SceneContext>.Instance.Player.transform.localPosition = this.transform.localPosition;




        }

        public void OnEnable()
        {
            
            base.transform.position = player.transform.position;
            this.transform.localPosition = player.transform.localPosition;
            player.GetComponent<vp_FPController>().MotorFreeFly = true;
            foreach (Transform child in player.transform.GetChild(0))
            {
                child.gameObject.SetActive(false);
            }

            foreach (DirectedActorSpawner directedActorSpawner in
                Resources.FindObjectsOfTypeAll<DirectedActorSpawner>())
            {
                directedActorSpawner.enabled = false;
            }

            SRSingleton<HudUI>.Instance.gameObject.SetActive(false);
            SRSingleton<SceneContext>.Instance.PlayerState.InGadgetMode = true;
            foreach (var objects in listOfUIs)
            {
                objects.SetActive(true);
            }

        }

        public void Update()
        {


            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                speed = Mathf.Clamp(this.speed + 0.1f, 0.1f, 10f);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f )
            {
                speed = Mathf.Clamp(this.speed - 0.1f, 0.1f, 10f);
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
                _vpFpInput.MouseCursorForced = true;

            }
            else
            {
                _vpFpInput.MouseCursorForced = false;
                this.lastRotation += 2f * Input.GetAxis("Mouse X");
                this.rotation -= 2f * Input.GetAxis("Mouse Y");
                this.rotation = Mathf.Clamp(this.rotation, -90f, 90f);
                base.transform.eulerAngles = new Vector3(this.rotation,
                    this.lastRotation, 0f);

            }

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo))
                {
                }

            }
            
        }


        // Token: 0x040000E0 RID: 224
        private float lastRotation = 0f;

        // Token: 0x040000E1 RID: 225
        private float rotation = 0f;
        private float speed = 4;
        private vp_FPInput _vpFpInput;
    }
    
}
           
