using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StickBehaviour : MonoBehaviour, IManipulationHandler {
    private StickManager stickManagerScript;
    //private ObjectSettings settingsScript;

    private List<Collision> collisions = new List<Collision>();

    private Coroutine stickCoroutine;
    private Coroutine unStickCoroutine;

    private Vector3 stickPosition;
    private Vector3 stickNormal;

    private bool isStick = false;

    [Header("Stick entities")]
    public GameObject stickManagerObject;
    public GameObject stickObject;
    public GameObject stickTrigger;
    public GameObject stickCanvas;

    public GameObject player;

    private void Awake() {
        //settingsScript = gameObject.GetComponent<ObjectSettings>();


        stickManagerScript = stickManagerObject.GetComponent<StickManager>();
    }

    private void Start() {
        this.stickPosition = stickTrigger.transform.position;
        this.stickNormal = stickTrigger.transform.forward;
    }

    private void Update() {
        #if UNITY_EDITOR
        Debug.DrawLine(stickObject.transform.position, stickObject.transform.position - stickObject.transform.forward, Color.cyan, 0.1f, false);
        #endif

        if (isStick) {
            Stick();
        } else {
            // set unstick coroutine rotation
            this.stickPosition = stickTrigger.transform.position;
            this.stickNormal = stickTrigger.transform.forward;

            UnStick();
        }
    }

    private void OnCollisionEnter(Collision col) {
        Vector3 colPoint = col.contacts[0].point;
        Vector3 colNormal = col.contacts[0].normal;

        #if UNITY_EDITOR
        // draw stick normal
        Debug.DrawLine(colPoint, colNormal, Color.green, 2, false);
        #endif

        if (Vector3.Dot(colPoint - Camera.main.transform.position, colNormal) < 0) {
            stickObject.transform.parent = stickManagerObject.transform;

            this.stickPosition = colPoint;
            this.stickNormal = CalculateAwerageContactNormal(transform.position, -colNormal);

            isStick = true;
        }

        collisions.Add(col);
    }

    private void OnCollisionExit(Collision col) {
        if(collisions.Count > 0) {
            for (int i = 0; i < collisions.Count; i++) {
                if (collisions[i].collider == col.collider) {
                    Vector3 colPoint = collisions[i].contacts[0].point;
                    Vector3 colNormal = collisions[i].contacts[0].normal; 

                    #if UNITY_EDITOR
                    // draw unstick normal
                    Debug.DrawLine(colPoint, colNormal, Color.yellow, 2, false);
                    #endif

                    RaycastHit hit;
                    if (Physics.Raycast(new Ray(transform.position, -colNormal), out hit)) {
                        stickObject.transform.parent = gameObject.transform;
                        isStick = false;
                    }

                    collisions.RemoveAt(i);
                }
            }
        }
    }

    private Vector3 CalculateAwerageContactNormal(Vector3 RayOrigin, Vector3 RayDirection) {
        Ray mainRay = new Ray(RayOrigin, RayDirection);

        #if UNITY_EDITOR
        Debug.DrawLine(mainRay.origin, mainRay.direction * stickManagerScript.stickRayLength, Color.magenta, 2, false);
        #endif

        Vector3 collisionNormalAwerage = new Vector3();
        int collisionNormalCount = 0;
        for (int i = 0; i < stickManagerScript.stickRayCount; i++) {
            Vector3 norm = mainRay.origin + Random.insideUnitSphere;

            /*Vector3 norm = new Vector3(
                Random.Range(mainRay.direction.normalized.x - stickManagerScript.stickRayCastDispersion, mainRay.direction.normalized.x + stickManagerScript.stickRayCastDispersion),
                Random.Range(mainRay.direction.normalized.y - stickManagerScript.stickRayCastDispersion, mainRay.direction.normalized.y + stickManagerScript.stickRayCastDispersion),
                Random.Range(mainRay.direction.normalized.z - stickManagerScript.stickRayCastDispersion, mainRay.direction.normalized.z + stickManagerScript.stickRayCastDispersion));
            */
            RaycastHit hit;
            Physics.Raycast(new Ray(mainRay.origin, norm), out hit, stickManagerScript.stickRayLength);

            if(hit.normal != Vector3.zero) {
                collisionNormalAwerage += hit.normal;
            }

            #if UNITY_EDITOR
            Debug.DrawLine(mainRay.origin, norm.normalized * stickManagerScript.stickRayLength, Color.red, 2, false);
            #endif
        }

        return -1 * (collisionNormalAwerage / (collisionNormalCount == 0 ? 1 : collisionNormalCount));
    }

    public void Stick() {
        // stop unstick coroutine
        if(unStickCoroutine != null) {
            StopCoroutine(unStickCoroutine);
            unStickCoroutine = null;
        }

        // start stick coroutine
        if (stickCoroutine == null) {
            stickCoroutine = StartCoroutine(StickObject(stickObject));
        }
    }

    public void UnStick() {
        // stop stick coroutine
        if (stickCoroutine != null) {
            StopCoroutine(stickCoroutine);
            stickCoroutine = null;
        }

        // start unstick coroutine
        if (unStickCoroutine == null) {
            unStickCoroutine = StartCoroutine(StickObject(stickObject));
        }
    }

    public IEnumerator StickObject(GameObject gO) {
        while (true) {
            gO.transform.forward = Vector3.Lerp(gO.transform.forward, this.stickNormal, stickManagerScript.stickSpeed);
            gO.transform.position = Vector3.Lerp(gO.transform.position, this.stickPosition, stickManagerScript.stickSpeed);

            yield return null;
        } 
    }

    public void OnManipulationCompleted(ManipulationEventData eventData) {
        //refresh trigger and object rotation after drop
        this.gameObject.transform.localEulerAngles += stickObject.transform.localEulerAngles;
        this.gameObject.transform.localPosition += stickObject.transform.localPosition;
        stickObject.transform.localEulerAngles = Vector3.zero;
        stickObject.transform.localPosition = Vector3.zero;

        stickTrigger.transform.rotation = stickObject.transform.rotation;
    }

    public void OnManipulationStarted(ManipulationEventData eventData) {}
    public void OnManipulationUpdated(ManipulationEventData eventData) {}
    public void OnManipulationCanceled(ManipulationEventData eventData) {}
}
