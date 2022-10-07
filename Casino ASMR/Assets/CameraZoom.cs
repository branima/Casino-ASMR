using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Transform cam;
    CameraRotation camRotScript;

    int currActive;
    Transform currActiveTransform;

    bool reposition;
    float repoTime;

    public static CameraZoom Instance;
    private void Awake()
    {
        Instance = this;
        currActive = 0;
        currActiveTransform = transform.GetChild(currActive);
        reposition = false;
        repoTime = 0f;
        camRotScript = cam.GetComponentInParent<CameraRotation>();
    }

    void Update()
    {
        if (reposition)
        {
            //UnityEngine.Debug.Log("Zdravo " + (cam.position != currActiveTransform.position));
            repoTime += Time.deltaTime * 0.1f;
            if (cam.position != currActiveTransform.position)
                cam.position = Vector3.Lerp(cam.position, currActiveTransform.position, repoTime);

            if (cam.position == currActiveTransform.position)
            {
                reposition = false;
                camRotScript.enabled = true;
            }
        }
    }

    public void ChangeCamera()
    {
        //UnityEngine.Debug.Log(transform.name + ", " + currActive);
        //string callingFuncName = new StackFrame(1).GetMethod().Name;
        //UnityEngine.Debug.Log(callingFuncName);
        camRotScript.enabled = false;
        currActive++;
        if (currActive == transform.childCount)
            currActive = 0;
        //UnityEngine.Debug.Log(currActive);
        currActiveTransform = transform.GetChild(currActive);
        reposition = true;
        repoTime = 0f;
    }
}
