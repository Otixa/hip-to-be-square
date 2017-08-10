using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[RequireComponent(typeof(Text))]
[Serializable]
[ExecuteInEditMode()]
public class PropertyMonitor : MonoBehaviour {
	public GameObject target;
	public Component component;
	public string fieldName;
	public object initialValue;
    public string formatting = "{value}";
	Text t;

	void Start() {
		t = GetComponent<Text> ();
		initialValue = GetField (component,fieldName);
        t.text = initialValue.ToString();
    }

	void Update() {
        if (GetField (component, fieldName) != initialValue) {
			initialValue = GetField (component,fieldName);
			t.text = formatting.Replace("{value}", initialValue.ToString());
		}
	}

	public void SetTarget(GameObject target) {
		this.target = target;
		component = target.GetComponent (component.GetType());
	}

	public static T GetReference<T>(object inObj, string fieldName) where T : class {
		return GetField (inObj, fieldName) as T;
	}
	public static T GetValue<T>(object inObj, string fieldName) where T : struct {
		return (T)GetField (inObj, fieldName);
	}
	public static void SetField(object inObj, string fieldName, object newValue) {
		FieldInfo info = inObj.GetType ().GetField (fieldName);
		if (info != null)
			info.SetValue (inObj, newValue);
	}
	public static object GetField(object inObj, string fieldName) {
		object ret = null;
		MemberInfo info = inObj.GetType ().GetMember (fieldName).First();

        if (info is FieldInfo)
            ret = ((FieldInfo)info).GetValue(inObj);
        else
            ret = ((PropertyInfo)info).GetValue(inObj, null);
        return ret;
	}

	public static MemberInfo[] GetProperties(object inObj) {
		BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public |
			BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
		Type t = inObj.GetType ();
        MemberInfo[] members = t.GetMembers(flags);
        return members.Where(p => p.MemberType == MemberTypes.Field || p.MemberType == MemberTypes.Property).Cast<MemberInfo>().ToArray();
	}
}