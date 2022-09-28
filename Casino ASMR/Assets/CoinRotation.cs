using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRotation : MonoBehaviour
{

    public float speed = 10f;

    void Update() => transform.Rotate(0f, 0f, 25f * speed * Time.deltaTime);
}
