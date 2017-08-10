using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(PropertyMonitor))]
public class PropertyMonitorInspector : Editor {
	PropertyMonitor t;
	Component targetComponent;
	public int componentIndex;
	int propertyIndex;
	string fieldName;
	
	public override void OnInspectorGUI() {
		t = (PropertyMonitor)target;
		t.target = EditorGUILayout.ObjectField ("Target", t.target, typeof(GameObject), true) as GameObject;
		if (t.target != null) {
			if(t.component != null) {
				int i = getComponents().IndexOf(t.component);
				if(i>=0) componentIndex = i;
			}
			componentIndex = EditorGUILayout.Popup("Component", componentIndex, getComponentNames().ToArray());
			targetComponent = getComponents()[componentIndex];
			List<string> properties = new List<string>();
			PropertyMonitor.GetProperties(targetComponent).ToList().ForEach(p => properties.Add(p.Name));
			if(properties.Count>0) {
				if(t.fieldName != null) {
					int fi = properties.IndexOf(t.fieldName);
					if(fi >= 0) propertyIndex = fi;
				}
				propertyIndex = EditorGUILayout.Popup("Property", propertyIndex, properties.ToArray());
                if(propertyIndex >= properties.Count)
                {
                    propertyIndex = 0;
                }
				fieldName = properties[propertyIndex];
			}
		}
        t.formatting = EditorGUILayout.TextField("Formatting", t.formatting);
		if (GUI.changed) {
			t.component = getComponents()[componentIndex];
			t.fieldName = fieldName;
			EditorUtility.SetDirty (target);
			serializedObject.ApplyModifiedProperties();
		}
	}

	List<string> getComponentNames() {
		List<string> names = new List<string>();
		getComponents().ForEach(c => names.Add(c.GetType().ToString()));
		return names;
	}

	List<Component> getComponents() {
		return t.target.GetComponents<Component> ().ToList();
	}

}