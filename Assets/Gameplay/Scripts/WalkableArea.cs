using UnityEngine;

public class WalkableArea : MonoBehaviour
{
    void Awake()
    {
        var width = GameController.Generation.Width;
        var height = GameController.Generation.Height;

        transform.position = new Vector3((float)width / 2, 0f, height / 2);

        transform.localScale = new Vector3(width, height, 1f);
        GetComponent<LocalNavMeshBuilder>().m_Size =
            new Vector3(
                width + 4f, 1f, height + 4f
            );
    }
}
