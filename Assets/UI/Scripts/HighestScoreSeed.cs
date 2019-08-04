using TMPro;
using UnityEngine;

public class HighestScoreSeed : MonoBehaviour
{
    void Awake() =>
        GetComponent<TextMeshProUGUI>().text = 
            "Seed: " + 
                PlayerPrefs.GetInt(C.PPHighestScoreSeed, -1).ToString();
}
