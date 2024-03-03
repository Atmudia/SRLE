using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SRLE.Components;

[RegisterTypeInIl2Cpp]
public class InspectorSpawner : InspectorBase
{
    private Text m_Label;
    private Dropdown m_Dropdown;
    private List<Transform> m_Transforms = new List<Transform>();
    private Type WhatReturns;
    

    public string Name
    {
        set { m_Label.text = value; }
    }

    private void Awake()
    {
        m_Label = transform.Find("Label").GetComponent<Text>();
        m_Dropdown = GetComponentInChildren<Dropdown>();
        // var optionData = option[^1];
        // transform.Find("Triangle").GetComponent<Button>().onClick.AddListener(new System.Action(() =>
        // {
        //     transform.Find("Dropdown").gameObject.SetActive(!transform.Find("Dropdown").gameObject.activeSelf);
        // }));
    }

    private void Start()
    {
        var getter1 = getter();
        if (getter1 is Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<DirectedActorSpawner.SpawnConstraint>)
        {
              DirectedActorSpawner.SpawnConstraint[] array = (Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<DirectedActorSpawner.SpawnConstraint>)getter();
                for (var index = 0; index < array.Length; index++)
                {
                    m_Dropdown.options.Add(new Dropdown.OptionData()
                    {
                        text = $"Constraint {m_Dropdown.options.Count}"
                    });
                }

                m_Dropdown.options.Add(new Dropdown.OptionData()
                {
                    text = "Add new constraint"
                });
                m_Dropdown.RefreshShownValue();
                m_Dropdown.onValueChanged.AddListener(new Action<int>(i =>
                {
                    if (i == m_Dropdown.options.Count - 1)
                    {
                        var il2CppArrayBase = m_Dropdown.options.ToArray();
                        il2CppArrayBase[i].text = $"Constraint {m_Dropdown.options.Count}";
                        m_Dropdown.value = i;
                        m_Dropdown.options.Add(new Dropdown.OptionData()
                        {
                            text = "Add new constraint"
                        });
                        m_Dropdown.Set(i);
                        setter((Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<DirectedActorSpawner.SpawnConstraint>)array.AddToArray(new DirectedActorSpawner.SpawnConstraint()
                        {
                            Slimeset = new SlimeSet()
                            {
                                Members = Array.Empty<SlimeSet.Member>(),
                            },
                            Window = new DirectedActorSpawner.TimeWindow()
                            {
                                EndHour = 0,
                                StartHour = 0,
                                TimeMode = DirectedActorSpawner.TimeMode.ANY
                            },
                            Feral = false,
                            Weight = 0,
                            MaxAgitation = false,
                        }));
                        
                        m_Dropdown.RefreshShownValue();
                    }
                    else
                    {
                        foreach (var mTransform in m_Transforms)
                        {
                            Object.Destroy(mTransform.gameObject);
                        }

                        foreach (var inspectorSpawnerArray in this.transform.parent.GetComponentsInChildren<InspectorSpawner>().SelectMany(x => x.m_Transforms))
                        {
                            Object.Destroy(inspectorSpawnerArray.gameObject);
                        }
                        
                        m_Transforms.Clear();
                        DirectedActorSpawner.SpawnConstraint[] array = (Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<DirectedActorSpawner.SpawnConstraint>)getter();
                        var spawnConstraint = array[i];
                        var  inspectorBool = Instantiate(AssetManager.InspectorBool, transform.parent).AddComponent<InspectorBool>();
                        inspectorBool.BindTo(spawnConstraint, typeof(DirectedActorSpawner.SpawnConstraint).GetMember("Feral")[0]);
                        inspectorBool.Name = "Is Feral?";
                        m_Transforms.Add(inspectorBool.transform);
                        inspectorBool = Instantiate(AssetManager.InspectorBool, transform.parent).AddComponent<InspectorBool>();
                        inspectorBool.BindTo(spawnConstraint, typeof(DirectedActorSpawner.SpawnConstraint).GetMember("MaxAgitation")[0]);
                        inspectorBool.Name = "Max Agitation?";
                        m_Transforms.Add(inspectorBool.transform);
                        
                        
                        var inspectorInput = Instantiate(AssetManager.InspectorInput, transform.parent).AddComponent<InspectorInput>();
                        inspectorInput.BindTo(spawnConstraint, typeof(DirectedActorSpawner.SpawnConstraint).GetMember("Weight")[0]);
                        inspectorInput.Name = "Weight";
                        inspectorInput.Cast = typeof(float);
                        inspectorInput.Input.characterValidation = InputField.CharacterValidation.Decimal;
                        m_Transforms.Add(inspectorInput.transform);
                        
                        inspectorInput = Instantiate(AssetManager.InspectorInput, transform.parent).AddComponent<InspectorInput>();
                        inspectorInput.BindTo(spawnConstraint.Window, typeof(DirectedActorSpawner.TimeWindow).GetMember("StartHour")[0]);
                        inspectorInput.Name = "Start Hour";
                        inspectorInput.Cast = typeof(float);
                        inspectorInput.Input.characterValidation = InputField.CharacterValidation.Decimal;
                        m_Transforms.Add(inspectorInput.transform);
                        inspectorInput = Instantiate(AssetManager.InspectorInput, transform.parent).AddComponent<InspectorInput>();
                        inspectorInput.BindTo(spawnConstraint.Window, typeof(DirectedActorSpawner.TimeWindow).GetMember("EndHour")[0]);
                        inspectorInput.Name = "End Hour";
                        inspectorInput.Cast = typeof(float);
                        inspectorInput.Input.characterValidation = InputField.CharacterValidation.Decimal;
                        m_Transforms.Add(inspectorInput.transform);
                        
                        
                        var inspectorSpawner = Instantiate(AssetManager.InspectorArray, transform.parent).AddComponent<InspectorSpawner>();
                        var memberInfo = typeof(DirectedActorSpawner.TimeWindow).GetProperty("TimeMode");
                        inspectorSpawner.BindTo(spawnConstraint.Window, memberInfo);
                        inspectorSpawner.Name = "Time Window";
                        inspectorSpawner.WhatReturns = memberInfo.PropertyType;
                        m_Transforms.Add(inspectorSpawner.transform);
                        
                        inspectorSpawner = Instantiate(AssetManager.InspectorArray, transform.parent).AddComponent<InspectorSpawner>();
                        inspectorSpawner.BindTo(spawnConstraint.Slimeset, typeof(SlimeSet).GetMember("Members")[0]);
                        inspectorSpawner.Name = "SlimeSets";
                        m_Transforms.Add(inspectorSpawner.transform);


                    }
            
            
                }));
                m_Dropdown.onValueChanged.Invoke(0);
        }
        if (getter1 is Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<SlimeSet.Member>)
        {
            SlimeSet.Member[] array = (Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<SlimeSet.Member>)getter();
            foreach (var tMember in array)
            {
                m_Dropdown.options.Add(new Dropdown.OptionData()
                {
                    text = $"Member {tMember.IdentType.ReferenceId}"
                });
            }
            m_Dropdown.options.Add(new Dropdown.OptionData()
            {
                text = "Add new slimeset member"
            });
            m_Dropdown.RefreshShownValue();
             m_Dropdown.onValueChanged.AddListener(new Action<int>(i =>
                {
                    if (i == m_Dropdown.options.Count - 1)
                    {
                        var il2CppArrayBase = m_Dropdown.options.ToArray();
                        il2CppArrayBase[i].text = $"Slimeset {m_Dropdown.options.Count}";
                        m_Dropdown.value = i;
                        m_Dropdown.options.Add(new Dropdown.OptionData()
                        {
                            text = "Add new slimeset member"
                        });
                        setter((Il2CppReferenceArray<SlimeSet.Member>)array.AddToArray(new SlimeSet.Member()
                        {
                            
                        }));
                        m_Dropdown.RefreshShownValue();
                    }
                    else
                    {
                        
                        foreach (var mTransform in m_Transforms)
                        {
                            MelonLogger.Msg(mTransform.gameObject.ToString());
                            Object.Destroy(mTransform.gameObject);
                        }
                        m_Transforms.Clear();
                        SlimeSet.Member[] array = (Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<SlimeSet.Member>)getter();
                        var member = array[i];

                        var inspectorSpawner = Instantiate(AssetManager.InspectorArray, transform.parent).AddComponent<InspectorSpawner>();
                        m_Transforms.Add(inspectorSpawner.transform);
                        //inspectorSpawner.BindTo(member, typeof(SlimeSet.Member).GetMember("_prefab")[0]);
                        PropertyInfo memberInfo = typeof(SlimeSet.Member).GetProperty("_prefab");
                        inspectorSpawner.BindTo(getter: () => memberInfo.GetValue(member), setter: value =>
                        {
                            if (value != null)
                            {
                                memberInfo.SetValue(member, value);
                                member.ConvertPrefabToId();
                            }
                        });
                        inspectorSpawner.Name = "Identifiable ";
                        inspectorSpawner.WhatReturns = typeof(GameObject);

                    }
                }));
             m_Dropdown.onValueChanged.Invoke(0);
        }
        if (WhatReturns == typeof(GameObject))
        {
            
            var findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll<SlimeDefinition>().Where(x => x.prefab != null).ToArray();
            foreach (var tMember in findObjectsOfTypeAll)
            {
                var txt = tMember.ReferenceId;
                m_Dropdown.options.Add(new Dropdown.OptionData()
                {
                    text = $"{txt.Replace("SlimeDefinition.", string.Empty)}"
                });
                m_Dropdown.RefreshShownValue();
            }
            if (getter() != null)
            {
                var identTypeReferenceId = ((GameObject)getter()).GetComponent<Identifiable>().identType.ReferenceId;
                for (var index = 0; index < findObjectsOfTypeAll.Length; index++)
                {
                    var VARIABLE = findObjectsOfTypeAll[index];
                    if (VARIABLE.ReferenceId == identTypeReferenceId)
                    {
                        m_Dropdown.value = index;
                        break;
                    }
                }
            }

            m_Dropdown.onValueChanged.AddListener(new Action<int>(i =>
            {
                setter(findObjectsOfTypeAll[i].prefab);
                
                
            }));
        }

        if (WhatReturns is { IsEnum: true })
        {
            var values = Enum.GetValues(WhatReturns).Cast<object>().ToList();
            foreach (var enumObj in values)
            {
                m_Dropdown.options.Add(new Dropdown.OptionData()
                {
                    text = enumObj.ToString()
                });
            }
            m_Dropdown.RefreshShownValue();
            m_Dropdown.onValueChanged.AddListener(new Action<int>(i =>
            {
                setter(values[i]);
            }));
        }
        

        
    }
}