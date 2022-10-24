using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRoll : MonoBehaviour
{

    public float speed;
    float currSpeed;

    void Awake() => currSpeed = speed;

    void Update() => transform.Rotate(0, 0, -90f * Time.deltaTime * currSpeed);

    public void SetNormalSpeed() => currSpeed = speed;
    public void SetBoostedSpeed() => currSpeed = speed * 2f;
    public void SetCustomSpeed(float custSpeed) => currSpeed = custSpeed;

    public float GetCurrSpeed() => currSpeed;
}
