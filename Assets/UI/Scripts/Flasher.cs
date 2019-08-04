using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Flasher : MonoBehaviour
{
    const float FlashDuration = 1f;

    #pragma warning disable CS0649
    [SerializeField] Color _defaultColour;
    [SerializeField] Color _altColour;
    #pragma warning restore CS0649

    Image _img;

    Coroutine _co;

    void Awake() => _img = GetComponent<Image>();

    void OnEnable() =>
        _co = StartCoroutine(Flash());

    void OnDisable()
    {
        StopCoroutine(_co);
        _img.color = _defaultColour;
    }

    IEnumerator Flash()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(FlashDuration);
            _img.color = _altColour;

            yield return new WaitForSecondsRealtime(FlashDuration);
            _img.color = _defaultColour;
        }
    }
}
