using UnityEngine;
using UnityEngine.EventSystems;

public class BackToMenuButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) =>
        GameController.EndGame();
}
