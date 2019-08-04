using TMPro;
using UnityEngine;

public class SeedInputField : MonoBehaviour
{
    TMP_InputField _field;

    void Awake() =>
        _field = GetComponent<TMP_InputField>();

    void OnEnable()
    {
        _field.onValueChanged.AddListener(ValueChanged);
        GameController.onSeedChanged += UpdateText;
    }

    void Start()
    {
        UpdateText();
        _field.gameObject.SetActive(false);
        _field.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        _field.onValueChanged.RemoveListener(ValueChanged);
        GameController.onSeedChanged -= UpdateText;
    }

    void ValueChanged(string newSeed) =>
        GameController.Seed = int.Parse(newSeed);

    void UpdateText() =>
        _field.text = GameController.Seed.ToString();
}
