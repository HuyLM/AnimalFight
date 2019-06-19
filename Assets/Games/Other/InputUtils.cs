using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputUtils {

    public static bool PointerOverNothing(TouchPhase phase=TouchPhase.Began) {
        #if UNITY_EDITOR
        return (phase == TouchPhase.Began ? Input.GetMouseButtonDown(0) : Input.GetMouseButtonUp(0)) && 
            !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && !IsPointerOverUIObject();
#else
        return Input.touchCount > 0 && Input.GetTouch(0).phase == phase &&
            !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && !IsPointerOverUIObject();
#endif
    }

    public static bool PointerDown()
    {
#if UNITY_EDITOR
        return (Input.GetMouseButtonDown(0)) &&
            !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && !IsPointerOverUIObject();
#else
        return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began &&
            !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && !IsPointerOverUIObject();
#endif
    }

    public static bool PointerUp()
    {
#if UNITY_EDITOR
        return (Input.GetMouseButtonUp(0)) &&
            !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && !IsPointerOverUIObject();
#else
        return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended &&
            !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && !IsPointerOverUIObject();
#endif
    }

    public static bool PointerHoldOverNothing() {
        return Input.GetMouseButton(0) && 
            !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && !IsPointerOverUIObject();
    }

    public static bool PointerUpOverNothing()
    {
        return Input.GetMouseButtonUp(0) &&
            !EventSystem.current.IsPointerOverGameObject() && !IsPointerOverUIObject();
    }

    public static bool PointerOverNothing (Touch touch)
    {
        return !EventSystem.current.IsPointerOverGameObject (touch.fingerId);
    }

    public static bool PointerOverNothing (Touch touch, TouchPhase phase)
    {
        return touch.phase == phase && !EventSystem.current.IsPointerOverGameObject (touch.fingerId);
    }

    private static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
