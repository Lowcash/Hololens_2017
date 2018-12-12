using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class DragInfo : MonoBehaviour, IManipulationHandler, IFocusable {
    private PlaceableManager placeable;

    private void Awake()
    {
        placeable = gameObject.GetComponent<PlaceableManager>();
    }

    public void OnFocusEnter() {
        //Debug.Log("Focus: " + gameObject.name);
    }

    public void OnFocusExit() {
        
    }

    public void OnManipulationCanceled(ManipulationEventData eventData) {
        Debug.Log("Cancel");

        placeable.IsDragging = false;
    }

    public void OnManipulationCompleted(ManipulationEventData eventData) {
        Debug.Log("Complete");

        placeable.IsDragging = false;
    }

    public void OnManipulationStarted(ManipulationEventData eventData) {
        Debug.Log("Hold");

        placeable.IsDragging = true;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData) {
        
    }
}
