using UnityEngine;

public class InsetBySafeArea : MonoBehaviour
{
    RectTransform _tfm;
    Rect _lastRect;

    void Awake()
    {
        _tfm = GetComponent<RectTransform>();
        Update();
    }

    void Update()
    {
        var safeArea = Screen.safeArea;

        if (safeArea == _lastRect) return;
        _lastRect = safeArea;
     
        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
     
        _tfm.anchorMin = anchorMin;
        _tfm.anchorMax = anchorMax;
    }
}
