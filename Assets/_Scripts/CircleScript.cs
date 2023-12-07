using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CircleOnARPlane : MonoBehaviour
{
    public GameObject circlePrefab; // faites glisser votre objet "Circle" ici dans l'inspecteur

    private void Start()
    {
        ARPlaneManager arPlaneManager = FindObjectOfType<ARPlaneManager>();
        // arPlaneManager.planesChanged += OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (var plane in args.added)
        {
            PlaceCircleOnPlane(plane);
        }
    }

    private void PlaceCircleOnPlane(ARPlane plane)
    {
        Vector3 planeCenter = plane.transform.position;

        // Créer une instance du cercle et le placer au centre du plan
        GameObject circle = Instantiate(circlePrefab, planeCenter, Quaternion.identity);

        // Assurez-vous que le cercle est orienté correctement par rapport au plan (par exemple, rotation normale du plan)
        circle.transform.rotation = Quaternion.LookRotation(plane.normal);
    }
}
