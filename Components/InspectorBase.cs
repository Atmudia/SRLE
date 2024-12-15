using System;
using System.Reflection;
using MelonLoader;
using UnityEngine;

namespace SRLE.Components;

[RegisterTypeInIl2Cpp]
public class InspectorBase : MonoBehaviour
{
    public delegate object Getter();
    public delegate void Setter(object value);

    public Getter getter;
    public Setter setter;

    public void BindTo(object parent, MemberInfo member, string variableName = null)
    {
        switch (member)
        {
            case FieldInfo field:
                variableName ??= field.Name;

                BindTo(() => field.GetValue(parent), (value) =>
                {
                    field.SetValue(parent, value);
                });
                break;
            case PropertyInfo property:
                variableName ??= property.Name;

                BindTo(() => property.GetValue(parent, null), (value) =>
                {
                    property.SetValue(parent, value, null);
                });
                break;
            default:
                throw new ArgumentException("Member can either be a field or a property");
        }
    }

    public void BindTo(Getter getter, Setter setter)
    {
        this.getter = getter;
        this.setter = setter;
    }
}