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
    [SerializeField] private GameObject _borderObject;
    [SerializeField] private Material _borderObjectMat;
    [SerializeField] private Color _placeableColour;
    [SerializeField] private Color _notPlaceableColor;
    #endregion

    #region Unity Actions

    private void OnTriggerEnter(Collider other)
    {
        if ((obstacleLayer.value & 1 << other.gameObject.layer) == 0)
            return;
        _obstaclesTriggering++;
        OnTriggerCountChange();
    }

    private void OnTriggerExit(Collider other)
    {
        if ((obstacleLayer.value & 1 << other.gameObject.layer) == 0)
            return;
        _obstaclesTriggering--;
        OnTriggerCountChange();
    }


    #endregion

    #region Methods
    void OnTriggerCountChange()
    {
        var colour = _obstaclesTriggering > 0 ? _notPlaceableColor : _placeableColour;
        SetMatColour(colour);
    }

    void SetMatColour(Color colour)
    {
        _borderObjectMat.color = colour;
    }
    public void OnCreate()
    {
        _obstaclesTriggering = 0;
        _collider.isTrigger = true;
        SetMatColour(_placeableColour);
        _borderObject.SetActive(true);
    }

    public bool TryPlace()
    {
        if (_obstaclesTriggering != 0)
            return false;
        _collider.isTrigger = false;
        
        _borderObject.SetActive(false);
        return true;
    }


    #endregion

    #region Serialisble classes

    

    #endregion
}