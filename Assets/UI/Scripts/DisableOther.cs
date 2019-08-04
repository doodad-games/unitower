using UnityEngine;
using UnityEngine.EventSystems;

public class DisableOther : MonoBehaviour, IPointerClickHandler
{
    #pragma warning disable CS0649
    [SerializeField] GameObject _other;
    #pragma warning restore CS0649

    public void OnPointerClick(PointerEventData eventData) =>
        _other.SetActive(false);
}