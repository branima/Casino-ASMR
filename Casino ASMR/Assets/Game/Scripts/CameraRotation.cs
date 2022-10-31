using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{

    public float speed = 1f;

    float lastX;
    float deltaX;

    void Start()
    {
        lastX = 0f;
        deltaX = 0f;
    }

    Vector3 position;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            lastX = Input.mousePosition.x;
        else if (Input.GetMouseButton(0))
        {
            deltaX = Input.mousePosition.x - lastX;
            lastX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
            deltaX = 0f;

        float sideMoveNum = Mathf.Clamp(deltaX * Time.deltaTime * 100f, -1, 1);
        transform.Rotate(0f, speed * sideMoveNum, 0f);
    }
}
