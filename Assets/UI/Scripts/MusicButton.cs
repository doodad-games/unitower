using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MusicButton : MonoBehaviour, IPointerClickHandler
{
    #pragma warning disable CS0649
    [SerializeField] TextMeshProUGUI _text;
    #pragma warning restore CS0649

    public void OnPointerClick(PointerEventData eventData)
    {
        MusicController.Toggle();
        UpdateText();
    }

    void Start() => UpdateText();

    void UpdateText() =>
        _text.text = "Music: " + 
            (MusicController.MusicOn ? "On" : "Off");
}
