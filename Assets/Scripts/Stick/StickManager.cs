using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickManager : MonoBehaviour {
    public float stickSpeed = 1.0f;

    [Header("RayCast settings")]
    public float stickRayCount = 10;

    public float stickRayLength = 10;

    public float stickRayCastDispersion = 0.5f;

}
