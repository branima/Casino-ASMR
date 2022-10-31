using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralScale : MonoBehaviour
{

    Vector3 target;

    float lerpTime;
    bool scale;

    public float speed = 1f;
    float localModif = 0.1f;

    void Awake()
    {
        scale = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!scale)
            return;

        lerpTime += speed * localModif * Time.deltaTime;
        //Debug.Log(transform.name + ": " + transform.localScale + ", " + target + ", " + lerpTime);
        transform.localScale = Vector3.Lerp(transform.localScale, target, lerpTime);
        if (Vector3.Distance(transform.localScale, target) < 0.00001f)
        {
            //transform.localScale = target;
            scale = false;

            //Debug.Log("Ben");
            if (name.Contains("Ramp"))
                transform.parent = null;

            if (target == Vector3.zero)
                gameObject.SetActive(false);
        }
    }

    public void Scale(Vector3 target)
    {
        //Debug.Log("ALOOOO" + transform.name + ", " + transform.localScale + ", " + target);
        this.target = target;
        lerpTime = 0f;
        scale = true;
    }
}
