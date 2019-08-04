using UnityEngine;
using UnityEngine.EventSystems;

public static class UIHelper
{
    public static bool OverUI() => 
        Input.touchCount != 0
            ? EventSystem.current.IsPointerOverGameObject(0)
            : EventSystem.current.IsPointerOverGameObject();
}