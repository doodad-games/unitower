using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) =>
        SoundController.Click();

    public void OnPointerEnter(PointerEventData eventData) =>
        SoundController.Hover();
}