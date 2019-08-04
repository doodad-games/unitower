using UnityEngine;
using UnityEngine.EventSystems;

public class ActivationToggler : MonoBehaviour, IPointerClickHandler
{
    #pragma warning disable CS0649
    [SerializeField] GameObject _toToggle;
    [SerializeField] bool _withCloseSound;
    #pragma warning restore CS0649

    public void OnPointerClick(PointerEventData eventData)
    {
        var newState = !_toToggle.activeSelf;
        _toToggle.SetActive(newState);

        if (_withCloseSound && !newState)
            SoundController.Close();
    }
}
