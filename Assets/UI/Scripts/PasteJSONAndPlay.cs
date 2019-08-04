using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PasteJSONAndPlay : MonoBehaviour, IPointerClickHandler
{
    #pragma warning disable CS0649
    [SerializeField] GameObject _failPopup;
    #pragma warning restore CS0649

    public void OnPointerClick(PointerEventData eventData)
    {
        var json = GUIUtility.systemCopyBuffer;

        GenerationData generation = null;

        try { generation = JsonUtility.FromJson<GenerationData>(json); }
        catch (Exception e) { Debug.LogError(e); }

        if (
            generation == null ||
            !TA.CheckGenerationValidity(generation)
        )
        {
            Abort();
            return;
        }

        GameController.StartGame(generation);
    }

    void Abort()
    {
        ScreenShake.Do();
        SoundController.Error();
        _failPopup.SetActive(true);
    }
}
