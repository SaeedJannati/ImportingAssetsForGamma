using UnityEngine;

public class PlacementController : MonoBehaviour
{
    #region Fields

    [SerializeField] private Building buildingPrefab;
    [SerializeField] private KeyCode createBuildingKeyCode;
    [SerializeField] LayerMask _layerMask;

    private Building currentCreatedBuilding;

    #endregion

    #region UnityActions

    private void Update()
    {
        CheckForNewObjectCreation();
        CheckMovement();
        CheckForPlaceMent();
    }

    private void CheckForPlaceMent()
    {
        if(currentCreatedBuilding==default)
            return;
        if(!Input.GetKeyDown(KeyCode.Mouse0))
            return;
        if(!currentCreatedBuilding.TryPlace())
            return;
        currentCreatedBuilding = default;
    }

    #endregion

    #region Methods

    void CheckMovement()
    {
        if (currentCreatedBuilding == default)
            return;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray,hitInfo: out RaycastHit hitInfo,layerMask:_layerMask,maxDistance:Mathf.Infinity))
            return;
        currentCreatedBuilding.transform.position = hitInfo.point;
        currentCreatedBuilding.transform.rotation=Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
    }

    private void CheckForNewObjectCreation()
    {
        if (!Input.GetKeyDown(createBuildingKeyCode))
            return;
        if (currentCreatedBuilding == null)
        {
            currentCreatedBuilding = Instantiate(buildingPrefab);
            currentCreatedBuilding.OnCreate();
            return;
        }

        Destroy(currentCreatedBuilding);
    }

    #endregion
}