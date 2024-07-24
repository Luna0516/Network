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

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);
            
            LayerMask L_mask = LayerMask.GetMask("Monster");

            int mask = (1 << 8) | (1 << 9); // 8번 Monster, 9번 Wall

            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f, L_mask))
            {
                Debug.Log($"Raycast {hit.collider.gameObject.name}");
            }
        }

        /*
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            
            //Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            //Vector3 dir = mousePos - Camera.main.transform.position;
            //dir.Normalize();
            

            // Debug.DrawRay(Camera.main.transform.position, dir * 100.0f, Color.red, 1.0f);
            Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
            {
                Debug.Log($"Raycast {hit.collider.gameObject.name}");
            }
        }
        */

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
