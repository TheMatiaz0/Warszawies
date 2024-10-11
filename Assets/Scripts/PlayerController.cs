using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject Building;

    [SerializeField]
    Camera PlayerCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 HitPoint = PlayerCamera.ScreenToViewportPoint(Input.mousePosition);

        Ray ray = PlayerCamera.ViewportPointToRay(PlayerCamera.ScreenToViewportPoint(Input.mousePosition));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000)) 
        {
            Vector3 newPosition = hit.point;
            if(newPosition.x < 0) newPosition.x = newPosition.x - newPosition.x % 10 - 5;
            else newPosition.x = newPosition.x - newPosition.x % 10 + 5;


            if (newPosition.z < 0) newPosition.z = newPosition.z - newPosition.z % 10 - 5;
            else newPosition.z = newPosition.z - newPosition.z % 10 + 5;
           
            
            Building.transform.position = newPosition;
        }
        
        

    }
}
