using System;
using MelonLoader;
using UnityEngine.UI;


namespace SRLE.Components
{
    [RegisterTypeInIl2Cpp]
    public class InspectorInput : InspectorBase
    {
        private Text m_Label;
        public InputField Input;

        public Action OnChange;

        public string Name { set => m_Label.text = value; }
        public string Placeholder { set => Input.placeholder.GetComponent<Text>().text = value; }

        public Type Cast;

        private void Awake()
        {
            m_Label = transform.Find("Label").GetComponent<Text>();
            Input = transform.Find("Input").GetComponent<InputField>();

            Input.onEndEdit.AddListener(new System.Action<string>(OnEndEdit));
        }

        private void Start()
        {
            Input.text = Convert.ToString(getter()); ;
        }

        private void OnEndEdit(string arg0)
        {
            if (Cast != null && Cast == typeof(float))
            {
                if (float.TryParse(arg0, out var result))
                {
                    setter(result);
                }
                else
                {
                    setter(0);
                }

            }
            else
            {
                setter(arg0);
            }
            OnChange?.Invoke();
        }
    }
}