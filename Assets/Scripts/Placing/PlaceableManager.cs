using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableManager : MonoBehaviour {
    public Placeable placeableScript;

    public bool IsDragging { get; set; }

    private Collider _collider;

    private void Awake()
    {
        _collider = gameObject.GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        if (IsDragging)
        {
            if (IsHitDetected())
            {
                placeableScript.enabled = true;
                placeableScript.OnPlacementStart();
            }
            else
            {
                placeableScript.OnPlacementStop();
                placeableScript.enabled = false;
            }
        }
        else
        {
            placeableScript.OnPlacementStop();
            placeableScript.enabled = false;
        }
    }

    private bool IsHitDetected()
    {
        var colliders = Physics.OverlapBox(_collider.bounds.center, _collider.bounds.extents, Quaternion.identity, 1 << 31);

        return colliders.Length > 0;
    }
}
