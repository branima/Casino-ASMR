using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCollectionLogic : MonoBehaviour
{

    GameManager gameManager;
    Animator animator;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        //animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        //animator.SetTrigger("LeverTrigger");
        int rewardMoney = other.GetComponentInParent<ItemAttributes>().GetRewardMoney();
        gameManager.AddMoney(rewardMoney);
    }
}
