using UnityEngine;

public class DespawnPoint : MonoBehaviour
{
    void Awake() => transform.position = new Vector3(
        0.125f,
        0f,
        GameController.Generation.Height / 2
    );

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>() != null) GameController.LoseGame();
    }
}
