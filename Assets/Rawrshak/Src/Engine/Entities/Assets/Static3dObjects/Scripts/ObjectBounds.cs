using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectBounds : MonoBehaviour
{
    public GameObject targetObject;
    public Material errorMaterial;
    public Material validMaterial;

    private Collider cageCollider;
    private Collider objectCollider;
    private MeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        // Get Object
        if (targetObject)
        {
            objectCollider = targetObject.GetComponent<Collider>();
        }

        cageCollider = gameObject.GetComponent<Collider>();
        mesh = gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cageCollider && objectCollider)
        {
            if (CompletelyInsideBounds(cageCollider.bounds, objectCollider.bounds))
            {
                // Debug.Log("Inside Cage");
                mesh.sharedMaterial = validMaterial;
            }
            else
            {
                // Debug.Log("Outside Cage");
                mesh.sharedMaterial = errorMaterial;
            }
        }
    }

    public void RefreshTargetObject()
    {
        // Get Object
        if (!targetObject)
        {
            Debug.LogError("No Target Object.");
            return;
        }
        objectCollider = targetObject.GetComponent<Collider>();

        if (!objectCollider)
        {
            Debug.LogError("Target Object doesn't have a Collider.");
            return;
        }
        Update();
    }

    public void ClearTargetObject()
    {
        objectCollider = null;
        targetObject = null;
    }

    bool CompletelyInsideBounds(Bounds cage, Bounds obj)
    {
        Vector3 cageMin = cage.min;
        Vector3 cageMax = cage.max;
        Vector3 objMin = obj.min;
        Vector3 objMax = obj.max;
        if (cageMin.x <= objMin.x &&
            cageMin.y <= objMin.y &&
            cageMin.z <= objMin.z &&
            cageMax.x >= objMax.x &&
            cageMax.y >= objMax.y &&
            cageMax.z >= objMax.z)
        {
            return true;
        }
        return false;
    }
}
