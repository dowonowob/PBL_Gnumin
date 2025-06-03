using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class EventManager : MonoBehaviour
{  
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject canvas;
    
    
    public GameObject placeObject;

    void Start()
    {   
        
        
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray;
            RaycastHit hitobj;

            ray = arCamera.ScreenPointToRay(touch.position);


            if (Physics.Raycast(ray, out hitobj))
            {   
                if (hitobj.collider.CompareTag("Heart_Up"))
                {  
                    
                    //hitobj.collider.gameobject.setactive(false);
                    canvas.SetActive(true);
                    
                    Vector3 targetPosition = hitobj.collider.transform.position + hitobj.collider.transform.forward * - 0.8f;
                    targetPosition.y = placeObject.transform.position.y + 1.8f;
                    canvas.transform.position = targetPosition;

            
                }
            }
        }

        if (canvas.activeSelf)
        {
            Vector3 directionToCamera = arCamera.transform.position - canvas.transform.position;
            directionToCamera.y = 0; // 수직 회전 방지 (필요 시)
            canvas.transform.rotation = Quaternion.LookRotation(-directionToCamera);
        }
        else {
            placeObject.SetActive(true);
        }
    }
}


