using UnityEngine;
using UnityEngine.EventSystems;

public class CopyExampleJSONButton : MonoBehaviour, IPointerClickHandler
{
    #pragma warning disable CS0649
    [SerializeField] GameObject _infoPopup;
    #pragma warning restore CS0649

    public void OnPointerClick(PointerEventData eventData)
    {
        GUIUtility.systemCopyBuffer =
            Resources.Load<TextAsset>("example-json").text;

        _infoPopup.SetActive(true);
    }
}
