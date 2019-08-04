using UnityEngine;
using UnityEngine.EventSystems;

public class ExitGameButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) =>
        Application.Quit();

    void Awake()
    {
        if (
            Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer
        ) Destroy(gameObject);
    }
}
