using SRLE.RuntimeGizmo.Objects.Commands;
using SRLE.RuntimeGizmo.UndoRedo;
using UnityEngine.UI;

namespace SRLE.Components
{
    public class InspectorBool : InspectorBase
    {
        public string Name { set => _label.text = value; }
        private Text _label;
        private Toggle _toggle;

        public void Awake()
        {
            _label = transform.Find("Label").GetComponent<Text>();
            _toggle = transform.Find("Toggle").GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        public void Start()
        {
            if (getter != null)
                _toggle.isOn = (bool)getter();
        }

        private void OnValueChanged(bool isOn)
        {
            if (getter == null || setter == null) return;
            var oldValue = (bool)getter();
            if (oldValue ==  isOn) return;
            UndoRedoManager.Execute(new InspectorChangeCommand(setter, oldValue, isOn));
        }
    }
}