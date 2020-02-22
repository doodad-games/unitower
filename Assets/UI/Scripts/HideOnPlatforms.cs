using UnityEngine;

public class HideOnPlatforms : MonoBehaviour
{
    #pragma warning disable CS0649
    [SerializeField] bool _web;
    #pragma warning restore CS0649

    void Awake()
    {
        var hide = _web && Application.platform == RuntimePlatform.WebGLPlayer;

        gameObject.SetActive(!hide);
    }
}
