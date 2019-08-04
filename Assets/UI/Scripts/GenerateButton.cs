using UnityEngine;
using UnityEngine.EventSystems;

public class GenerateButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) =>
        GameController.StartGame(null);
}
