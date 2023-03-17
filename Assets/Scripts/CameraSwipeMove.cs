using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraSwipeMove : MonoBehaviour
{
    #region Fields

    private Camera _mainCam;
    private Vector3 _lastPos;

    #endregion

    #region Properties

    public Action<Vector3> onSwipe { get; set; } = default;

    #endregion

    #region Unity actions

    private void Start()
    {
        _mainCam = Camera.main;
    }

    #endregion

    #region Methots

    public void OnBeginDrag(BaseEventData data)
    {
        var mousePos = data.currentInputModule.input.mousePosition;
        _lastPos = mousePos;
    }

    public void OnDrag(BaseEventData data)
    {
        var mousePos = data.currentInputModule.input.mousePosition;
        var deltaPos =(Vector3) mousePos - _lastPos;
        _lastPos = mousePos;
        onSwipe?.Invoke(deltaPos);
    }

    public void OnDragEnd(BaseEventData data)
    {
    }

    Vector3 CalcPointerPosition(BaseEventData data)
    {
        var mousePos = data.currentInputModule.input.mousePosition;
        var pos = _mainCam.ScreenToWorldPoint(mousePos);
        pos.z = 0.0f;
        return pos;
    }

    #endregion
}