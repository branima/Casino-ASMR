using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCollectionLogic : MonoBehaviour
{

    GameManager gameManager;
    Animator animator;
    public MoneyTextPool moneyTextPool;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        moneyTextPool = gameManager.gameObject.GetComponent<MoneyTextPool>();
        animator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        int rewardMoney = other.GetComponentInParent<ItemAttributes>().GetRewardMoney();
        gameManager.AddMoney(rewardMoney);
        //moneyTextPool.SpawnMoney(rewardMoney, other.transform.position);
        moneyTextPool.SpawnMoney(rewardMoney, transform.position);
        animator.SetTrigger("CollectTrigger");
    }
}
