using UnityEngine;
using System.Collections;

public class CullingGroupSample : MonoBehaviour
{
    private CullingGroup group = null;
    private BoundingSphere[] bounds;

    [SerializeField] Transform[] targets = null;

    void Start()
    {
        group = new CullingGroup();

        //　Set the camera for culling
        group.targetCamera = Camera.main;

        // Set the center coordinates for measuring distance and distance level
        // 1: 1 meter 2: 5 meters 3: 10 meters, 4: 30 meters, 5: 100 meters or longer: invisible treatment
        group.SetDistanceReferencePoint(Camera.main.transform);
        group.SetBoundingDistances(new float[] { 1, 5, 10, 30, 100 });

        // Set the list to perform visibility determination
        bounds = new BoundingSphere[targets.Length];
        for (int i = 0; i < bounds.Length; i++)
        {
            bounds[i].radius = 1.5f;
        }
        // Register a reference to the list
        group.SetBoundingSpheres(bounds);
        group.SetBoundingSphereCount(targets.Length);
        // Register the callback when the visibility of the object changes
        group.onStateChanged = OnChange;
    }
    void Update()
    {
        // Update the coordinates of the registered object
        for (int i = 0; i < bounds.Length; i++)
        {
            bounds[i].position = targets[i].position;
        }
    }
    void OnDestroy()
    {
        // cleanup
        group.onStateChanged -= OnChange;
        group.Dispose();
        group = null;
    }
    void OnChange(CullingGroupEvent ev)
    {
        // Only activate objects that are not in the view
        targets[ev.index].gameObject.SetActive(ev.isVisible);
        // If the range is 2m or more, deactivate
        if (ev.currentDistance > 2)
        {
            targets[ev.index].gameObject.SetActive(false);
            Debug.Log(targets[ev.index].gameObject.name + " is InVisible ");
        }
    }

}