using TMPro;
using UnityEngine;

public class CurrentTowers : MonoBehaviour
{
    #pragma warning disable CS0649
    [SerializeField] Flasher _flasher;
    #pragma warning restore CS0649

    TextMeshProUGUI _text;

    void Awake() =>
        _text = GetComponent<TextMeshProUGUI>();

    void OnEnable()
    {
        GameController.onTowersChanged += UpdateDisplay;
        GameController.onTowersAvailableChanged += UpdateDisplay;
    }

    void Start() => UpdateDisplay();

    void OnDisable()
    {
        GameController.onTowersChanged -= UpdateDisplay;
        GameController.onTowersAvailableChanged -= UpdateDisplay;
    }

    void UpdateDisplay()
    {
        var towers = GameController.Towers;
        var available = GameController.TowersAvailable;

        _text.text = string.Format(
            "Towers: {0}/{1}",
            towers,
            available
        );

        _flasher.enabled = towers < available;
    }
}
