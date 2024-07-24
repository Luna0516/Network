using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaycast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Local <-> World <-> Viewport <-> Screen(화면)

        // Debug.Log(Input.mousePosition); // Screen

        // Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition)); // Viewport
        
        /*
        Vector3 look = transform.forward;
        Debug.DrawRay(transform.position + Vector3.up, look * 10, Color.red);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position + Vector3.up, look, 10);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log($"Raycast {hit.collider.gameObject.name}");
        }
        */
    }
}
