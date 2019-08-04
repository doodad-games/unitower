using UnityEngine;

public class LoseGamePopup : MonoBehaviour
{
    public static LoseGamePopup Instance { get; private set; }

    void OnEnable()
    {
        Instance = this;

        if (!GameController.IsGameOver) gameObject.SetActive(false);
    }
}
