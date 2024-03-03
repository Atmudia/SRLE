using System;
using MelonLoader;
using UnityEngine.UI;

namespace SRLE.Components;

[RegisterTypeInIl2Cpp]
public class InspectorBool : InspectorBase
{
    public string Name { set { m_Label.text = value; } }
    private Text m_Label;
    private Toggle m_Toggle;

    public void Awake()
    {
        m_Label = transform.Find("Label").GetComponent<Text>();
        m_Toggle = transform.Find("Toggle").GetComponent<Toggle>();

        m_Toggle.onValueChanged.AddListener(new System.Action<bool>(Action));
    }

    public void Start()
    {
        m_Toggle.isOn = (bool)getter();
    }

    private void Action(bool isOn)
    {
        setter(isOn);
    }
}