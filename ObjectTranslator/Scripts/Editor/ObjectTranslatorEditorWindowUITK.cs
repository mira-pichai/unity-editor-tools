using System;
using System.Collections.Generic;
using UITKUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class ObjectTranslatorEditorWindowUITK : EditorWindow
{
    private const string TranslateAllButtonName = "TranslateAll";
    private const string TranslateSelectionButtonName = "TranslateSelection";
    private const string TranslatableObjectsListViewName = "TranslatableObjectsListView";

    private const string ErrorVisualTreeAssetMissing = "No VisualTreeAsset assigned in the Inspector";
    private const string ErrorCheckUXMLComponentNames =
        "Null Reference Exception thrown. Make sure your UXML component labels are correct.";
    
    [SerializeField] private VisualTreeAsset m_treeVisualAsset;

    private List<GameObject> m_objectsWithComponent;
    private ObjectTranslator m_objectTranslator;
    
    private VisualElement m_Root;
    private ListView m_TranslatableObjectListView;
    private Button m_TranslateAllButton;
    private Button m_TranslateSelectionButton;

    private void OnEnable()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private void OnDisable()
    {
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
    }

    // chonky method. clean this up
    public void CreateGUI()
    {
        m_Root = rootVisualElement;

        if(m_treeVisualAsset == null)
        {
            Debug.Log(ErrorVisualTreeAssetMissing);
            return;
        }

        m_treeVisualAsset.CloneTree(m_Root);
        InitializeFields();
        InitializeObjectsWithComponent();
        MakeListView();
        BindButtonCallbacks();
    }
    
    [MenuItem("Tools/Object Translator UITK")]
    public static void ShowWindow()
    {
        var window = GetWindow<ObjectTranslatorEditorWindowUITK>();
        window.titleContent = new GUIContent("Object Translator UITK");
    }
    
    private void InitializeFields()
    {
        m_TranslateAllButton = m_Root.Q<Button>(TranslateAllButtonName);
        Validation.CheckQuery(m_TranslateAllButton, TranslateAllButtonName);

        m_TranslateSelectionButton = m_Root.Q<Button>(TranslateSelectionButtonName);
        Validation.CheckQuery(m_TranslateSelectionButton, TranslateSelectionButtonName);

        m_TranslatableObjectListView = m_Root.Q<ListView>(TranslatableObjectsListViewName);
        Validation.CheckQuery(m_TranslatableObjectListView, TranslatableObjectsListViewName);
    }

    private void OnHierarchyChanged()
    {
        RefreshObjectList();
        Repaint();
    }

    private void RefreshObjectList()
    {
        m_objectsWithComponent = GetSortedObjectsWithComponent<ObjectTranslator>();
        m_TranslatableObjectListView.Rebuild();
        m_TranslatableObjectListView.ClearSelection();
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
    
    private void InitializeObjectsWithComponent()
    {
        m_objectsWithComponent = GetSortedObjectsWithComponent<ObjectTranslator>();
    }
    
    private void BindButtonCallbacks()
    {
        m_TranslateAllButton.clicked += () =>
        {
            if(m_objectsWithComponent == null) return;
            foreach(var obj in m_objectsWithComponent)
            {
                var translator = obj.GetComponent<ObjectTranslator>();

                Undo.RecordObject(obj.transform, "Translate Object");
                translator.TranslateObject();
            }
        };

        m_TranslateSelectionButton.clicked += () =>
        {
            var selectedObjects = m_TranslatableObjectListView.selectedItems;

            if(selectedObjects == null) return;
            foreach(var item in selectedObjects)
                if(item is GameObject obj)
                {
                    Undo.RecordObject(obj.transform, "Translate Object");
                    obj.GetComponent<ObjectTranslator>()?.TranslateObject();
                }
        };
    }

    private void MakeListView()
    {
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (element, i) =>
        {
            if(i < 0 || i >= m_objectsWithComponent.Count)
            {
                Debug.LogWarning("Index out of bounds in bindItem.");
                return;
            }

            var label = element as Label;
            label.text = m_objectsWithComponent[i].name;
        };

        if(m_TranslatableObjectListView != null)
        {
            m_TranslatableObjectListView.makeItem = makeItem;
            m_TranslatableObjectListView.bindItem = bindItem;
            m_TranslatableObjectListView.itemsSource = m_objectsWithComponent;
            m_TranslatableObjectListView.selectionType = SelectionType.Multiple;
        }
        else
        {
            Debug.Log(ErrorCheckUXMLComponentNames);
        }
    }
}