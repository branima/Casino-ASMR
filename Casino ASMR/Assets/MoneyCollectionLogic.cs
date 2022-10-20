using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCollectionLogic : MonoBehaviour
{

    GameManager gameManager;
    Animator animator;
    public MoneyTextPool moneyTextPool;
    ParticleSystem moneyConfettiParticles;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        moneyTextPool = gameManager.gameObject.GetComponent<MoneyTextPool>();
        animator = GetComponentInChildren<Animator>();
        moneyConfettiParticles = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        int rewardMoney = other.GetComponentInParent<ItemAttributes>().GetRewardMoney();
        gameManager.AddMoney(rewardMoney);
        //moneyTextPool.SpawnMoney(rewardMoney, other.transform.position);
        moneyTextPool.SpawnMoney(rewardMoney, transform.position + Vector3.up * 0.5f);
        animator.SetTrigger("CollectTrigger");
        moneyConfettiParticles.Play();
    }
}
