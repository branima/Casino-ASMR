using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PathCreation.Examples;
using PathCreation;

public class GameManager : MonoBehaviour
{

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI incomePerSecondText;

    public GameObject mergeButton;

    public List<GameObject> collectionRamps;
    int activeCollectionRamps;
    int maxActiveCollectionRamps;

    Dictionary<string, int> coinDictionary;
    public List<GameObject> coinPrefabs;
    Dictionary<string, int> coinNamesDictionary;

    [SerializeField]
    List<PathFollower> activeCoins;
    [SerializeField]
    List<TrailRenderer> activeCoinsTrails;
    public PathCreator path;
    public float defaultCoinSpeed = 2f;
    float currCoinSpeed;
    public float speedBoostInterval = 1.5f;
    float lastSpeedBoostClick;

    Stack<int> removableCoinIndexes;

    int money;

    void Start()
    {
        money = 0;
        moneyText.text = "$0";

        activeCollectionRamps = 1;
        collectionRamps[0].SetActive(true);
        maxActiveCollectionRamps = 3;

        coinDictionary = new Dictionary<string, int>();
        coinNamesDictionary = new Dictionary<string, int>();
        for (int i = 0; i < coinPrefabs.Count; i++)
            coinNamesDictionary.Add(coinPrefabs[i].name, i);

        activeCoins = new List<PathFollower>();
        activeCoinsTrails = new List<TrailRenderer>();
        lastSpeedBoostClick = -100f;
        currCoinSpeed = defaultCoinSpeed;

        removableCoinIndexes = new Stack<int>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastSpeedBoostClick > speedBoostInterval)
            {
                currCoinSpeed = defaultCoinSpeed * 2f;
                foreach (PathFollower coin in activeCoins)
                    coin.speed = currCoinSpeed;
                foreach (TrailRenderer trail in activeCoinsTrails)
                    trail.enabled = true;
            }

            lastSpeedBoostClick = Time.time;
        }

        if (Time.time - lastSpeedBoostClick > speedBoostInterval && activeCoins.Count > 0 && currCoinSpeed == defaultCoinSpeed * 2f)
        {
            currCoinSpeed = defaultCoinSpeed;
            foreach (PathFollower coin in activeCoins)
                coin.speed = currCoinSpeed;
            foreach (TrailRenderer trail in activeCoinsTrails)
                trail.enabled = false;
        }

        bool mergeable = false;
        foreach (KeyValuePair<string, int> kvp in coinDictionary)
            if (kvp.Value >= 3)
                mergeable = true;
        if (mergeable)
            mergeButton.SetActive(true);
        else
            mergeButton.SetActive(false);
    }

    public void AddMoney(int value)
    {
        money += value;
        moneyText.text = "$" + money;
    }

    public void AddCoin()
    {
        GameObject coinInstance = Instantiate(coinPrefabs[0], Vector3.zero, coinPrefabs[0].transform.rotation, null);
        PathFollower pathFollowScript = coinInstance.GetComponent<PathFollower>();
        pathFollowScript.speed = currCoinSpeed;
        pathFollowScript.pathCreator = path;
        activeCoins.Add(pathFollowScript);
        activeCoinsTrails.Add(coinInstance.GetComponent<TrailRenderer>());
        if (coinDictionary.ContainsKey(coinPrefabs[0].name))
            coinDictionary[coinPrefabs[0].name]++;
        else
            coinDictionary.Add(coinPrefabs[0].name, 1);

        /*
        foreach (KeyValuePair<string, int> kvp in coinDictionary)
            Debug.Log(kvp.Key + ", " + kvp.Value);
        Debug.Log("-----------------------------");
        */
    }

    public void AddRoute()
    {
        maxActiveCollectionRamps += 3;
    }

    public void Income()
    {
        Debug.Log(activeCollectionRamps + ", " + maxActiveCollectionRamps);
        if (activeCollectionRamps < maxActiveCollectionRamps)
            collectionRamps[activeCollectionRamps++].SetActive(true);
    }

    public void MergeCoins()
    {
        foreach (KeyValuePair<string, int> kvp in coinDictionary)
        {
            //Debug.Log(kvp.Key + ", " + kvp.Value);
            if (kvp.Value >= 3)
            {
                coinDictionary[kvp.Key] -= 3;
                if (coinDictionary[kvp.Key] == 0)
                    coinDictionary.Remove(kvp.Key);

                int removeCnt = 3;
                for (int i = 0; i < activeCoins.Count; i++)
                {
                    if (activeCoins[i].name.Contains(kvp.Key))
                    {
                        removableCoinIndexes.Push(i);
                        Destroy(activeCoins[i].gameObject);
                        removeCnt--;
                        if (removeCnt == 0)
                            break;
                    }
                }

                while (removableCoinIndexes.Count > 0)
                {
                    int idx = removableCoinIndexes.Pop();
                    activeCoins.RemoveAt(idx);
                    activeCoinsTrails.RemoveAt(idx);
                }

                GameObject coinInstance = Instantiate(coinPrefabs[coinNamesDictionary[kvp.Key] + 1], Vector3.zero, coinPrefabs[coinNamesDictionary[kvp.Key] + 1].transform.rotation, null);
                PathFollower pathFollowScript = coinInstance.GetComponent<PathFollower>();
                pathFollowScript.speed = currCoinSpeed;
                pathFollowScript.pathCreator = path;
                activeCoins.Add(pathFollowScript);
                activeCoinsTrails.Add(coinInstance.GetComponent<TrailRenderer>());

                if (coinNamesDictionary[kvp.Key] + 1 < coinNamesDictionary.Count)
                {
                    if (coinDictionary.ContainsKey(coinPrefabs[coinNamesDictionary[kvp.Key] + 1].name))
                        coinDictionary[coinPrefabs[coinNamesDictionary[kvp.Key] + 1].name]++;
                    else
                        coinDictionary.Add(coinPrefabs[coinNamesDictionary[kvp.Key] + 1].name, 1);
                }
                //break;
                return;
            }
        }
    }

    /*
    foreach (KeyValuePair<string, int> kvp in coinDictionary)
        Debug.Log(kvp.Key + ", " + kvp.Value);
    Debug.Log("-----------------------------");
    */
}

