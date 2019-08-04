using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #pragma warning disable CS0649
    [SerializeField] TextMeshProUGUI _label;
    #pragma warning restore CS0649

    public void OnPointerEnter(PointerEventData eventData) =>
        _label.fontStyle = _label.fontStyle ^ FontStyles.Underline;

    public void OnPointerExit(PointerEventData eventData) =>
        _label.fontStyle = _label.fontStyle ^ FontStyles.Underline;
}