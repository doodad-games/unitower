using TMPro;
using UnityEngine;

public class CurrentScore : MonoBehaviour
{
    TextMeshProUGUI _text;

    void Awake() => _text = GetComponent<TextMeshProUGUI>();

    void OnEnable() =>
        GameController.onScoreChanged += UpdateText;

    void Start() => UpdateText();

    void OnDisable() =>
        GameController.onScoreChanged -= UpdateText;

    void UpdateText() =>
        _text.text = "Score: " + GameController.Score.ToString() +
            (GameController.Generation.EnemiesToSpawn == 0
                ? ""
                : "/" + GameController.Generation.EnemiesToSpawn.ToString()
            );
}
