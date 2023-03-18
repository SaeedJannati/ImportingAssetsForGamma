using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    #region Fields

    private Vector3 xAxis;
    private Vector3 yAxis;
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private Transform _camTransform;
    [SerializeField] private CameraSwipeMove _cameraSwipeMove;

    #endregion

    #region Unity actions

    private void Start()
    {
        RegisterToEvents();
        CalcAxis();
    }

    private void OnDestroy()
    {
        UnregisterFromEvents();
    }

    #endregion

    #region Methods

    void RegisterToEvents()
    {
        _cameraSwipeMove.onSwipe += OnSwipe;
    }

    void UnregisterFromEvents()
    {
        _cameraSwipeMove.onSwipe -= OnSwipe;
    }

    void CalcAxis()
    {
        var twoRoot = Mathf.Sqrt(2.0f);
        xAxis = twoRoot / 2 * new Vector3(1, 0, -1);
        yAxis = twoRoot / 2 * new Vector3(1, 0, 1);
    }

    async void OnSwipe(Vector3 deltaMove)
    {
        if (Input.touchCount > 1)
            return;
        var deltaPos = Time.deltaTime * moveSpeed * (deltaMove.x * xAxis + deltaMove.y * yAxis);

        _camTransform.position += deltaPos;
    }

    #endregion
}