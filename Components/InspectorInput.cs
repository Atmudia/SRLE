using System;
using System.Globalization;
using SRLE.RuntimeGizmo.Objects.Commands;
using SRLE.RuntimeGizmo.UndoRedo;
using UnityEngine.UI;

namespace SRLE.Components
{
    public class InspectorInput : InspectorBase
    {
        private Text m_Label;
        public InputField Input;

        /// <summary>Called after both execute and undo, for side effects beyond setting the value.</summary>
        public Action OnChange;

        public string Name { set => m_Label.text = value; }
        public string Placeholder { set => Input.placeholder.GetComponent<Text>().text = value; }

        public Type Cast;

        private void Awake()
        {
            m_Label = transform.Find("Label").GetComponent<Text>();
            Input = transform.Find("Input").GetComponent<InputField>();
            Input.onEndEdit.AddListener(OnEndEdit);
        }

        private void Start()
        {
            Input.text = Convert.ToString(getter(), CultureInfo.InvariantCulture);
        }

        private void OnEndEdit(string arg0)
        {
            object oldValue = getter();
            object newValue;

            if (Cast == typeof(float))
                newValue = float.TryParse(arg0, NumberStyles.Float, CultureInfo.InvariantCulture, out var f) ? f : (object)0f;
            else
                newValue = arg0;

            if (Equals(oldValue, newValue)) return;

            UndoRedoManager.Execute(new InspectorChangeCommand(setter, oldValue, newValue, OnChange));
        }
    }
}
