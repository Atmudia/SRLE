using System;
using System.Globalization;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace SRLE.Components
{
    [RegisterTypeInIl2Cpp]
    public class InspectorVector3UI : InspectorBase
    {
        private Text Label;
        private InputField XInput;
        private InputField YInput;
        private InputField ZInput;

        private Vector3 m_LastVector;

        public Action OnBeforeChange;
        public string Name { set => Label.text = value; }

        public void Awake()
        {
            Label = transform.Find("Label").GetComponent<Text>();
            XInput = transform.Find("XInput").GetComponent<InputField>();
            YInput = transform.Find("YInput").GetComponent<InputField>();
            ZInput = transform.Find("ZInput").GetComponent<InputField>();
            
            XInput.onEndEdit.AddListener(new Action<string>(OnValueChanged));
            YInput.onEndEdit.AddListener(new Action<string>(OnValueChanged));
            ZInput.onEndEdit.AddListener(new Action<string>(OnValueChanged));
        }

        private void Update()
        {
            Vector3 vector = (Vector3)getter();
            if (Vector3.Distance(m_LastVector, vector) > 0.01f)
            {
                m_LastVector = vector;
                XInput.text = vector.x.ToString(CultureInfo.InvariantCulture);
                YInput.text = vector.y.ToString(CultureInfo.InvariantCulture);
                ZInput.text = vector.z.ToString(CultureInfo.InvariantCulture);;
            }
        }

        private void OnValueChanged(string arg)
        {
            if (float.TryParse(XInput.text, out float x) && float.TryParse(YInput.text, out float y) && float.TryParse(ZInput.text, out float z))
            {
                OnBeforeChange?.Invoke();
                setter(new Vector3(x, y, z));
            }
        }
        
    }
}