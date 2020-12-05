using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    bool isTouch = false;
    Vector2 startPosition;
    Vector2 lastPosition;
    public Transform cam;
    public Transform player;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {

        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                startPosition = Input.touches[0].position;
            }
            else
            {
                if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    lastPosition = Input.touches[0].position;
                    Debug.Log(Vector2.Distance(startPosition,lastPosition));
                    
                }
            }
        }
    }
}
