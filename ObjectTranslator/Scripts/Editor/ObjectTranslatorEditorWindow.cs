using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectTranslatorEditorWindow : EditorWindow
{
    private List<GameObject> _objectsWithComponent;
    private ObjectTranslator _objectTranslator;

    private void OnGUI()
    {
        // _objectsWithComponent = FindObjectsWithComponent();
        _objectsWithComponent = GetSortedObjectsWithComponent<ObjectTranslator>();

        GUILayout.Label("Translate Objects", EditorStyles.boldLabel);

        GUILayout.Space(10);
        GUILayout.Label("All Objects", EditorStyles.boldLabel);

        if(GUILayout.Button("Run on All"))
        {
            if(_objectsWithComponent != null)
                TranslateAllObjects();
            else
                Debug.Log("There are no objects with the component");
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Run Per Object", EditorStyles.boldLabel);

        if(_objectsWithComponent != null)
            foreach(var obj in _objectsWithComponent)
                if(GUILayout.Button(obj.name))
                {
                    TranslateSelectedObject(obj);
                    Debug.Log("Run on Selected Object was pressed");
                }
    }

    [MenuItem("Tools/Object Translator UGUI")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ObjectTranslatorEditorWindow));
    }
    
    private List<GameObject> GetSortedObjectsWithComponent<T>() where T : Component
    {
        // cr question: is FindObjectsByType the best option here?
        var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var objectsWithComponent = new List<GameObject>();

        foreach(var obj in allObjects)
        {
            if(obj.GetComponent<T>() != null)
                objectsWithComponent.Add(obj);
        }

        Debug.Log($"Found {objectsWithComponent.Count} objects with {typeof(T).Name}");
        
        //Sort objects so "xx11" is after "xx2"
        objectsWithComponent.Sort((a, b) => EditorUtility.NaturalCompare(a.name, b.name));

        return objectsWithComponent;
    }

    // Original function
    /*private List<GameObject> FindObjectsWithComponent()
    {
        var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID);
        var objectsWithComponent = new List<GameObject>();

        foreach(var obj in allObjects)
            if(obj.GetComponent<ObjectTranslator>() != null)
                objectsWithComponent.Add(obj);

        Debug.Log($"Found {objectsWithComponent.Count} objects with ObjectTranslator");

        return objectsWithComponent;
    }*/

    private void TranslateAllObjects()
    {
        foreach(var obj in _objectsWithComponent)
        {
            var translator = obj.GetComponent<ObjectTranslator>();

            if(translator != null)
                translator.TranslateObject();
        }
    }

    private void TranslateSelectedObject(GameObject obj)
    {
        var translator = obj.GetComponent<ObjectTranslator>();

        if(translator != null)
            translator.TranslateObject();
    }
    
}