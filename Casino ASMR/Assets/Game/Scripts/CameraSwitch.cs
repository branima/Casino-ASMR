using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{

    public Transform cam;
    CameraRotation camRotScript;

    int currActive;
    Transform currActiveTransform;

    bool reposition;
    float repoTime;

    public static CameraSwitch Instance;
    private void Awake()
    {
        Instance = this;
        currActive = 0;
        currActiveTransform = transform.GetChild(currActive);
        reposition = false;
        repoTime = 0f;
        camRotScript = cam.GetComponent<CameraRotation>();
    }

    void Update()
    {
        if (reposition)
        {
            //UnityEngine.Debug.Log("Zdravo " + (cam.position != currActiveTransform.position));
            //UnityEngine.Debug.Log(repoTime);
            repoTime += Time.deltaTime * 0.1f;
            if (cam.position != currActiveTransform.position)
                cam.position = Vector3.Lerp(cam.position, currActiveTransform.position, repoTime);

            if (cam.rotation != currActiveTransform.rotation)
                cam.rotation = Quaternion.Lerp(cam.rotation, currActiveTransform.rotation, repoTime);

            //UnityEngine.Debug.Log(cam.rotation.eulerAngles + ", " + currActiveTransform.rotation.eulerAngles);
            //UnityEngine.Debug.Log((cam.position == currActiveTransform.position) + ", " + (cam.rotation == currActiveTransform.rotation));
            if (cam.position == currActiveTransform.position && (cam.rotation == currActiveTransform.rotation || Vector3.Distance(cam.rotation.eulerAngles, currActiveTransform.rotation.eulerAngles) < 0.001f)) //OVO JE KRITICNO ZBOG RELATIVNE ROTACIJE
                                                                                                                                                                                                                 //if (repoTime >= 1f)
            {
                reposition = false;
                camRotScript.enabled = true;
            }
        }
    }

    public void ChangeCamera()
    {
        //Debug.Log("BEN");
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