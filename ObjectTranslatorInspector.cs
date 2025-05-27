using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectTranslator))]
public class ObjectTranslatorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var objectTranslator = (ObjectTranslator)target;

        if(GUILayout.Button("Translate Object")) objectTranslator.TranslateObject();
    }
}