using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelToTarget : MonoBehaviour
{
    [SerializeField]
    Transform target;

    public float speed = 0.1f;
    float lerpTime;

    void Awake() => target = null;

    void Update()
    {
        if (target == null)
            return;

        lerpTime += speed * Time.deltaTime;
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position = Vector3.Lerp(transform.position, target.position, lerpTime);
        if (Vector3.Distance(target.position, transform.position) < 0.01f)
        {
            target = null;
            Invoke("MergeComplete", 1f);
        }
    }

    public void SetTarget(Transform target)
    {
        lerpTime = 0f;
        this.target = target;
    }

    public Transform GetTarget() => target;

    public void MergeComplete() => FindObjectOfType<GameManager>().MergeComplete();
}
