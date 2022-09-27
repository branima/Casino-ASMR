using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTextLogic : MonoBehaviour
{

    public void TurnOff() => gameObject.SetActive(false);

    void Update()
    {
        transform.LookAt(-Camera.main.transform.position);
    }
}
