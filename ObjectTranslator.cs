using UnityEngine;

public class ObjectTranslator : MonoBehaviour
{
    [Header("Translate")] [SerializeField] [ContextMenuItem("Translate", nameof(TranslateObject))]
    private Vector3 translation = Vector3.up;

    private void Awake()
    {
        Debug.Log("Awake");
        TranslateObject();
    }

    [ContextMenu("Translate Object")]
    public void TranslateObject()
    {
        transform.Translate(translation);
    }

    [ContextMenu("Reset Translation")]
    public void ResetTranslation()
    {
        transform.position = Vector3.zero;
    }
}