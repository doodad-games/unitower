using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    void Awake() => transform.position = new Vector3(
        GameController.Generation.Width - 0.125f,
        0f,
        GameController.Generation.Height / 2
    );
}
