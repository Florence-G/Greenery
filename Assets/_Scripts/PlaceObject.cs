using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager),typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void onEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += OnFingerDown;
    }

    private void onDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= OnFingerDown;
    }

    private void OnFingerDown (EnhancedTouch.Finger finger){
        if(finger.index != 0) return;

        if(raycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            foreach(ARRaycastHit hit in hits){
                Pose pose = hit.pose;
                GameObject obj = Instantiate(prefab, pose.position, pose.rotation);
            }
        }
    }


}
