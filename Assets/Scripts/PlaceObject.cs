using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(ARPlaneManager),
    typeof(ARRaycastManager))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    private ARRaycastManager arRaycastManager;
    private ARPlaneManager arPlaneManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private List<GameObject> placedObjects = new List<GameObject>();

    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;

        // Raycast for planes to create a new object
        if (arRaycastManager.Raycast(finger.currentTouch.screenPosition,
            hits, TrackableType.PlaneWithinPolygon))
        {
            Pose pose = hits[0].pose;

            // Check if there is already an object around this position. If so, delete it
            foreach (GameObject obj1 in placedObjects)
            {
                if (Vector3.Distance(obj1.transform.position, pose.position) < 0.05f)
                {
                    placedObjects.Remove(obj1);
                    Destroy(obj1);
                    return;
                }
            }
            
            GameObject obj = Instantiate(prefab, pose.position, pose.rotation);
            placedObjects.Add(obj);
        }
    }

    public void DeleteAllPlacedPrefabs()
    {
        foreach (GameObject obj in placedObjects)
        {
            Destroy(obj);
        }
        placedObjects.Clear();
    }

    public void TogglePlanes(bool value)
    {
        foreach (ARPlane plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
        arPlaneManager.enabled = value;
    }

    public void TogglePlanes()
    {
        TogglePlanes(!arPlaneManager.enabled);
    }
}
