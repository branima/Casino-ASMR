using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInFadeOutProcedural : MonoBehaviour
{
    [SerializeField]
    Vector3 target;

    public float speed = 0.1f;
    float lerpTime;

    void Awake() => target = Vector3.zero;

    void Update()
    {
        if (target == Vector3.zero)
            return;

        lerpTime += speed * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, target, lerpTime);
        if (Vector3.Distance(target, transform.position) < 0.01f)
            target = Vector3.zero;
    }

    public void SetTarget(Vector3 target)
    {
        lerpTime = 0f;
        transform.position = target - Vector3.up * 0.5f;
        this.target = target + Vector3.up * 0.5f;
    }
}
