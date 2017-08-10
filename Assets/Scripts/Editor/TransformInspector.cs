using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformInspector: Editor
{
    public override void OnInspectorGUI()
    {
        Transform t = (Transform)target;

        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;

        EditorGUI.indentLevel = 0;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("R", GUILayout.Width(18)))
        {
            Undo.RegisterCompleteObjectUndo(t, "Reset Positions " + t.name);
            t.transform.position = Vector3.zero;
        }
        Vector3 position = EditorGUILayout.Vector3Field("Position", t.localPosition);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("R", GUILayout.Width(18)))
        {
            Undo.RegisterCompleteObjectUndo(t, "Reset Rotation " + t.name);
            t.transform.rotation = Quaternion.identity;
        }
        Vector3 eulerAngles = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("R", GUILayout.Width(18)))
        {
            Undo.RegisterCompleteObjectUndo(t, "Reset Scale " + t.name);
            t.transform.localScale = Vector3.one;
        }
        Vector3 scale = EditorGUILayout.Vector3Field("Scale", t.localScale);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Reset Transforms"))
        {
            Undo.RegisterCompleteObjectUndo(t, "Reset Transforms " + t.name);
            t.transform.position = Vector3.zero;
            t.transform.rotation = Quaternion.identity;
            t.transform.localScale = Vector3.one;
        }
        if (GUI.changed)
        {
            Undo.RegisterCompleteObjectUndo(t, "Transform Change");
            t.localPosition = FixIfNaN(position);
            t.localEulerAngles = FixIfNaN(eulerAngles);
            t.localScale = FixIfNaN(scale);
        }
    }

    private Vector3 FixIfNaN(Vector3 v)
    {
        if (float.IsNaN(v.x))
        {
            v.x = 0;
        }
        if (float.IsNaN(v.y))
        {
            v.y = 0;
        }
        if (float.IsNaN(v.z))
        {
            v.z = 0;
        }
        return v;
    }
}