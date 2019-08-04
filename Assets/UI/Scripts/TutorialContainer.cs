using UnityEngine;

public class TutorialContainer : MonoBehaviour
{
    void Awake()
    {
        var tut = GameController.Generation.Tutorial;

        if (tut == 0)
        {
            Destroy(gameObject);
            return;
        }

        Instantiate(
            Resources.Load<GameObject>(
                "Tutorial" + tut.ToString() + "Help"
            ),
            transform
        );
    }
}
