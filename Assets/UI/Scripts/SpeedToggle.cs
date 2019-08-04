using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpeedToggle : MonoBehaviour, IPointerClickHandler
{
    #pragma warning disable CS0649
    [SerializeField] TextMeshProUGUI _text;
    #pragma warning restore CS0649

    Flasher _flasher;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameController.ToggleSpeed();
        UpdateDisplay();
    }

    void Awake() => _flasher = GetComponent<Flasher>();

    void Start() => UpdateDisplay();

    void UpdateDisplay()
    {
        var speed = GameController.SpeedSetting;

        _text.text = string.Format("Speed: {0}x", speed);

        if (_flasher != null)
            _flasher.enabled = speed == 0;
    }
}
