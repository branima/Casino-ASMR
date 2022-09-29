using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyTextPool : MonoBehaviour
{

    public int numberOfInstances;
    public GameObject moneyTextPrefab;
    Queue<GameObject> moneyTextInstances;

    // Start is called before the first frame update
    void Start()
    {
        moneyTextInstances = new Queue<GameObject>();
        for (int i = 0; i < numberOfInstances; i++)
        {
            GameObject moneyInstance = Instantiate(moneyTextPrefab, Vector3.zero, moneyTextPrefab.transform.rotation);
            moneyTextInstances.Enqueue(moneyInstance);
        }
    }

    public void SpawnMoney(int value, Vector3 position)
    {
        GameObject moneyInstance = moneyTextInstances.Dequeue();
        moneyInstance.GetComponent<TextMeshPro>().text = "$" + value.ToString();
        //moneyInstance.transform.position = position;
        moneyInstance.GetComponent<FadeInFadeOutProcedural>().SetTarget(position);
        moneyInstance.SetActive(true);
        moneyTextInstances.Enqueue(moneyInstance);

    }
}
