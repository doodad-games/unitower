using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialStartButton : MonoBehaviour, IPointerClickHandler
{
    #pragma warning disable CS0649
    [SerializeField] int _tutorial;
    #pragma warning restore CS0649

    public void OnPointerClick(PointerEventData eventData) =>
        GameController.StartGame(
            JsonUtility.FromJson<GenerationData>(
                Resources.Load<TextAsset>("tutorial" + _tutorial.ToString())
                    .text
            )
        );
}
