using TMPro;
using UnityEngine;

public class CurrentSeed : MonoBehaviour
{
    #pragma warning disable CS0649
    [SerializeField] GameObject _gameObjectToDestroy;
    #pragma warning restore CS0649

    void Awake()
    {
        if (!GameController.IsGenerated)
        {
            Destroy(_gameObjectToDestroy);
            return;
        }

        GetComponent<TextMeshProUGUI>().text = 
            "Seed: " + GameController.Seed.ToString();
    }
}
