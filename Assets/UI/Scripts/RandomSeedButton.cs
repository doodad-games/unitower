using UnityEngine;
using UnityEngine.EventSystems;

public class RandomSeedButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) =>
        GameController.ShuffleSeed();
}
