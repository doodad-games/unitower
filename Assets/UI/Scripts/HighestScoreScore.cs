using TMPro;
using UnityEngine;

public class HighestScoreScore : MonoBehaviour
{
    void Awake() =>
        GetComponent<TextMeshProUGUI>().text =
            PlayerPrefs.GetInt(C.PPHighestScore, 0).ToString();
}
