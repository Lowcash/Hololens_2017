using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickableBehaviour : MonoBehaviour {
    private List<BoxCollider> colliders = new List<BoxCollider>();

    public GameObject stickManagerObject;

    private void Awake() {
        foreach (BoxCollider bC in gameObject.GetComponents<BoxCollider>()) {
            colliders.Add(bC);
        }
    }

    public void setAllCollidersActive(bool active) {
        foreach (BoxCollider bC in colliders) {
            bC.enabled = active;
        }
    }
}
