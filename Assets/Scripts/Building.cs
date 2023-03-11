using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Building : MonoBehaviour
{
    #region Fields

    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Collider _collider;
    [SerializeField] private int _obstaclesTriggering = 0;
    [SerializeField]  private List<Material> _materials;
    #endregion

    #region Unity Actions

    private void OnTriggerEnter(Collider other)
    {
        if ((obstacleLayer.value & 1 << other.gameObject.layer) == 0)
            return;
        _obstaclesTriggering++;
    }

    private void OnTriggerExit(Collider other)
    {
        if ((obstacleLayer.value & 1 << other.gameObject.layer) == 0)
            return;
        _obstaclesTriggering--;
    }

    #endregion

    #region Methods

    public void OnCreate()
    {
        _obstaclesTriggering = 0;
        _collider.isTrigger = true;
    }

    public bool TryPlace()
    {
        if (_obstaclesTriggering != 0)
            return false;
        _collider.isTrigger = false;
        return true;
    }

    [Button]
    void ExtractMaterials()
    {
        _materials = GetComponentsInChildren<Renderer>().Select(i=>i.sharedMaterial).ToList();
    }

    #endregion
}