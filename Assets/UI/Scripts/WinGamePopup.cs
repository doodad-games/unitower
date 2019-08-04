using UnityEngine;

public class WinGamePopup : MonoBehaviour
{
    public static WinGamePopup Instance { get; private set; }

    void OnEnable()
    {
        Instance = this;

        if (!GameController.IsGameOver) gameObject.SetActive(false);
    }
}
