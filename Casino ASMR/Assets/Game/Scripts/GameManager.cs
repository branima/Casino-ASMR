using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PathCreation.Examples;
using PathCreation;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI incomePerSecondText;

    public Button addCoinButton;
    public Button addRouteButton;
    public Button incomeButton;
    public Button mergeButton;
    public Button nextAreaButton;
    public Material uiGrayscaleMat;

    public int addCoinPrice = 0;
    public int addRoutePrice = 500;
    public int incomePrice = 0;
    public int mergePrice = 10;
    public int nextAreaPrice = 1000;

    int addCoinModif = 3;
    int addRouteModif = 20;
    int incomeModif = 10;
    int mergeModif = 10;

    public List<GameObject> collectionRamps;
    int activeCollectionRamps;
    int maxActiveCollectionRamps;

    Dictionary<string, int> coinDictionary;
    public List<GameObject> coinPrefabs;
    Dictionary<string, int> coinNamesDictionary;

    List<PathFollower> activeCoins;
    int currNumberOfCoins;
    public int maxNumberOfCoins = 12;
    List<TrailRenderer> activeCoinsTrails;
    List<CoinRoll> activeCoinRollScripts;

    public List<PathCreator> paths;
    public float defaultCoinSpeed = 2f;
    float currCoinSpeed;
    public float speedBoostInterval = 1.5f;
    float lastSpeedBoostClick;
    float pathLength;

    Stack<int> removableCoinIndexes;
    Queue<GameObject> mergeQueue;
    public Transform mergePoint;
    bool merged;
    bool isMerging;
    public ParticleSystem mergeParticles;

    public Transform startPoint;

    public float gapBetweenCoins = 1f;

    public Transform track;

    public List<Transform> upgrade;
    public List<Transform> downgrade;

    Vector3 rampScale;

    public CameraRotation camRotationScript;

    [SerializeField]
    int money;

    void Start()
    {
        SaveSystem.SaveGame(new SaveData(SceneManager.GetActiveScene().buildIndex));

        money = 0;
        moneyText.text = "$0";

        activeCollectionRamps = 1;
        collectionRamps[0].SetActive(true);
        maxActiveCollectionRamps = collectionRamps.Count;

        coinDictionary = new Dictionary<string, int>();
        coinNamesDictionary = new Dictionary<string, int>();
        for (int i = 0; i < coinPrefabs.Count; i++)
            coinNamesDictionary.Add(coinPrefabs[i].name, i);

        activeCoins = new List<PathFollower>();
        activeCoinsTrails = new List<TrailRenderer>();
        activeCoinRollScripts = new List<CoinRoll>();
        lastSpeedBoostClick = -100f;
        currCoinSpeed = defaultCoinSpeed;

        removableCoinIndexes = new Stack<int>();
        mergeQueue = new Queue<GameObject>();

        addCoinButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + addCoinPrice;
        addRouteButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + addRoutePrice;
        incomeButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + incomePrice;
        mergeButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + mergePrice;

        if (nextAreaPrice < 1000)
            nextAreaButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + nextAreaPrice;
        else if (nextAreaPrice < 1000000)
            nextAreaButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (nextAreaPrice * 1f / 1000).ToString("n1") + "K";
        else
            nextAreaButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (nextAreaPrice * 1f / 1000000).ToString("n1") + "M";

        ///FIRST COIN
        GameObject coinInstance = Instantiate(coinPrefabs[0], startPoint.position, coinPrefabs[0].transform.rotation, null);
        PathFollower pathFollowScript = coinInstance.GetComponent<PathFollower>();
        pathFollowScript.speed = currCoinSpeed;
        pathFollowScript.pathCreator = paths[0];
        activeCoins.Add(pathFollowScript);
        activeCoinsTrails.Add(coinInstance.GetComponentInChildren<TrailRenderer>());
        CoinRoll cr = coinInstance.GetComponentInChildren<CoinRoll>();
        cr.enabled = true;
        activeCoinRollScripts.Add(cr);
        currNumberOfCoins = 1;

        if (coinDictionary.ContainsKey(coinPrefabs[0].name))
            coinDictionary[coinPrefabs[0].name]++;
        else
            coinDictionary.Add(coinPrefabs[0].name, 1);

        pathLength = paths[0].path.length;

        rampScale = collectionRamps[0].transform.localScale;

        isMerging = false;
    }

    void Update()
    {
        if (isMerging)
            return;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Time.time - lastSpeedBoostClick > speedBoostInterval)
            {
                currCoinSpeed = defaultCoinSpeed * 2f;
                foreach (PathFollower coin in activeCoins)
                    coin.speed = currCoinSpeed;
                foreach (TrailRenderer trail in activeCoinsTrails)
                    trail.enabled = true;
                foreach (CoinRoll cr in activeCoinRollScripts)
                    cr.SetBoostedSpeed();
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
            foreach (CoinRoll cr in activeCoinRollScripts)
                cr.SetNormalSpeed();
        }

        bool mergeable = false;
        foreach (KeyValuePair<string, int> kvp in coinDictionary)
            if (kvp.Value >= 3 && (coinNamesDictionary[kvp.Key] + 1 < coinNamesDictionary.Count))
                mergeable = true;
        if (mergeable)
            mergeButton.gameObject.SetActive(true);
        else
            mergeButton.gameObject.SetActive(false);

        float incomePerLap = 0f;
        foreach (PathFollower coin in activeCoins)
            incomePerLap += coin.GetComponent<ItemAttributes>().GetRewardMoney();

        incomePerLap *= activeCollectionRamps;
        float incomePerSec = incomePerLap * currCoinSpeed / pathLength;
        incomePerSecondText.text = incomePerSec.ToString("n1") + "/s";
    }

    public void AddMoney(int value)
    {
        money += value;

        if (money < 1000)
            moneyText.text = "$" + money.ToString();
        else if (money < 1000000)
            moneyText.text = "$" + (money * 1f / 1000).ToString("n2") + "K";
        else
            moneyText.text = "$" + (money * 1f / 1000000).ToString("n2") + "M";

        if (money >= addCoinPrice && currNumberOfCoins < maxNumberOfCoins)
            addCoinButton.GetComponent<Image>().material = null;
        else
            addCoinButton.GetComponent<Image>().material = uiGrayscaleMat;

        if (money >= addRoutePrice && paths.Count > 1)
            addRouteButton.GetComponent<Image>().material = null;
        else
            addRouteButton.GetComponent<Image>().material = uiGrayscaleMat;

        if (money >= incomePrice && activeCollectionRamps < maxActiveCollectionRamps)
            incomeButton.GetComponent<Image>().material = null;
        else
            incomeButton.GetComponent<Image>().material = uiGrayscaleMat;

        if (money >= mergePrice)
            mergeButton.GetComponent<Image>().material = null;
        else
            mergeButton.GetComponent<Image>().material = uiGrayscaleMat;

        if (money >= nextAreaPrice)
            nextAreaButton.GetComponent<Image>().material = null;
        else
            nextAreaButton.GetComponent<Image>().material = uiGrayscaleMat;
    }

    public void AddCopper()
    {
        if (money >= addCoinPrice && currNumberOfCoins < maxNumberOfCoins)
        {
            currNumberOfCoins++;
            int oldAddCoinPrice = addCoinPrice;
            addCoinPrice += addCoinModif;
            if (addCoinPrice < 1000)
                addCoinButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + addCoinPrice;
            else if (addCoinPrice < 1000000)
                addCoinButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (addCoinPrice * 1f / 1000).ToString("n2") + "K";
            else
                addCoinButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (addCoinPrice * 1f / 1000000).ToString("n2") + "M";

            AddMoney(-oldAddCoinPrice);

            GameObject coinInstance = Instantiate(coinPrefabs[0], startPoint.position, coinPrefabs[0].transform.rotation, null);

            PathFollower pathFollowScript = coinInstance.GetComponent<PathFollower>();
            pathFollowScript.pathCreator = paths[0];
            TrailRenderer tr = coinInstance.GetComponentInChildren<TrailRenderer>();
            if (currCoinSpeed == defaultCoinSpeed * 2)
                tr.enabled = true;
            activeCoinsTrails.Add(tr);

            CoinRoll cr = coinInstance.GetComponentInChildren<CoinRoll>();
            activeCoinRollScripts.Add(cr);

            CoinSpacing coinSpacing = coinInstance.GetComponent<CoinSpacing>();
            coinSpacing.SetActiveCoins(activeCoins);
            coinSpacing.SetCoinGap(gapBetweenCoins);

            if (coinDictionary.ContainsKey(coinPrefabs[0].name))
                coinDictionary[coinPrefabs[0].name]++;
            else
                coinDictionary.Add(coinPrefabs[0].name, 1);

            if (currNumberOfCoins == maxNumberOfCoins)
                addCoinButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
        }
    }

    public void AddRoute()
    {
        if (money >= addRoutePrice && paths.Count > 1)
        {
            int oldAddRoutePrice = addRoutePrice;
            addRoutePrice += addRouteModif;
            if (addRoutePrice < 1000)
                addRouteButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + addRoutePrice;
            else if (addRoutePrice < 1000000)
                addRouteButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (addRoutePrice * 1f / 1000).ToString("n2") + "K";
            else
                addRouteButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (addRoutePrice * 1f / 1000000).ToString("n2") + "M";

            maxNumberOfCoins += 3;
            if (addCoinPrice < 1000)
                addCoinButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + addCoinPrice;
            else if (addCoinPrice < 1000000)
                addCoinButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (addCoinPrice * 1f / 1000).ToString("n2") + "K";
            else
                addCoinButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (addCoinPrice * 1f / 1000000).ToString("n2") + "M";

            AddMoney(-oldAddRoutePrice);

            if (upgrade.Count > 0)
            {
                ProceduralScale procScale = downgrade[0].GetComponent<ProceduralScale>();
                procScale.Scale(Vector3.zero);
                downgrade.RemoveAt(0);

                Transform child = upgrade[0];
                Vector3 oldScale = child.localScale;
                child.localScale = Vector3.zero;
                child.gameObject.SetActive(true);
                child.GetComponent<ProceduralScale>().Scale(oldScale);
                upgrade.RemoveAt(0);

                paths.RemoveAt(0);
                pathLength = paths[0].path.length;
                foreach (PathFollower coin in activeCoins)
                {
                    coin.pathCreator = paths[0];
                }

                if (child.childCount > 1)
                {
                    for (int i = 1; i < child.childCount; i++)
                    {
                        Transform ramp = child.GetChild(i);
                        if (ramp.GetComponent<MoneyCollectionLogic>() != null)
                        {
                            collectionRamps.Add(ramp.gameObject);
                            maxActiveCollectionRamps++;
                        }
                        //ramp.parent = null;
                        //ramp.localScale = rampScale;
                    }

                    if (incomePrice < 1000)
                        incomeButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + incomePrice;
                    else if (incomePrice < 1000000)
                        incomeButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (incomePrice * 1f / 1000).ToString("n2") + "K";
                    else
                        incomeButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (incomePrice * 1f / 1000000).ToString("n2") + "M";

                    if (money >= incomePrice && activeCollectionRamps < maxActiveCollectionRamps)
                        incomeButton.GetComponent<Image>().material = null;
                    else
                        incomeButton.GetComponent<Image>().material = uiGrayscaleMat;
                }

                if (paths.Count == 1)
                {
                    addRouteButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
                    addRouteButton.GetComponent<Image>().material = uiGrayscaleMat;
                }
            }
        }

        if (upgrade.Count == 0)
            nextAreaButton.gameObject.SetActive(true);
    }

    public void Income()
    {
        if (money >= incomePrice && activeCollectionRamps < maxActiveCollectionRamps)
        {
            int oldIncomePrice = incomePrice;
            incomePrice += incomeModif;
            if (activeCollectionRamps < maxActiveCollectionRamps)
            {
                if (incomePrice < 1000)
                    incomeButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + incomePrice;
                else if (incomePrice < 1000000)
                    incomeButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (incomePrice * 1f / 1000).ToString("n2") + "K";
                else
                    incomeButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (incomePrice * 1f / 1000000).ToString("n2") + "M";
            }
            AddMoney(-oldIncomePrice);

            GameObject ramp = collectionRamps[activeCollectionRamps++];
            Vector3 oldScale = ramp.transform.localScale;
            ramp.transform.localScale = Vector3.zero;
            ramp.SetActive(true);
            ramp.GetComponent<ProceduralScale>().Scale(oldScale);
            //StartCoroutine(Deparent(ramp.transform, 2f));

            if (activeCollectionRamps == maxActiveCollectionRamps)
                incomeButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
        }
    }

    public void MergeCoins()
    {
        merged = false;
        if (money >= mergePrice)
        {
            isMerging = true;
            camRotationScript.enabled = false;

            int oldMergePrice = mergePrice;
            mergePrice += mergeModif;
            if (mergePrice < 1000)
                mergeButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + mergePrice;
            else if (mergePrice < 1000000)
                mergeButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (mergePrice * 1f / 1000).ToString("n2") + "K";
            else
                mergeButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (mergePrice * 1f / 1000000).ToString("n2") + "M";

            AddMoney(-oldMergePrice);

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
                            activeCoins[i].transform.parent = mergePoint;
                            mergeQueue.Enqueue(activeCoins[i].gameObject);
                            removeCnt--;
                            if (removeCnt == 0)
                                break;
                        }
                    }

                    while (removableCoinIndexes.Count > 0)
                    {
                        int idx = removableCoinIndexes.Pop();
                        activeCoins.RemoveAt(idx);
                        activeCoinsTrails[idx].enabled = false;
                        activeCoinsTrails.RemoveAt(idx);
                        activeCoinRollScripts[idx].enabled = false;
                        activeCoinRollScripts.RemoveAt(idx);
                    }

                    removeCnt = 3;
                    while (removeCnt > 0)
                    {
                        GameObject coin = mergeQueue.Dequeue();
                        coin.GetComponent<PathFollower>().enabled = false;
                        coin.transform.LookAt(mergePoint);
                        coin.transform.Rotate(0f, 90f, 0f);
                        coin.AddComponent<CoinRotation>();
                        TravelToTarget ttt = coin.AddComponent<TravelToTarget>();
                        ttt.SetTarget(mergePoint);
                        mergeQueue.Enqueue(coin);
                        removeCnt--;
                    }

                    return;
                }
            }
        }
    }

    public void MergeComplete()
    {
        if (merged)
            return;

        string name = mergeQueue.Peek().name;
        name = name.Remove(name.Length - 7);
        int idx = coinNamesDictionary[name] + 1;

        merged = true;
        mergeParticles.Play();

        while (mergeQueue.Count > 0)
            Destroy(mergeQueue.Dequeue());

        GameObject coinInstance = Instantiate(coinPrefabs[idx], mergePoint.position, coinPrefabs[idx].transform.rotation, null);
        coinInstance.AddComponent<CoinRotation>();
        StartCoroutine(PutCoinOnTrack(coinInstance, 0.5f));
    }

    IEnumerator Deparent(Transform objectToDeparent, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        objectToDeparent.parent = null;
    }

    IEnumerator PutCoinOnTrack(GameObject coinInstance, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        TravelToTarget ttt = coinInstance.AddComponent<TravelToTarget>();
        ttt.SetTarget(startPoint);

        if (addCoinPrice < 1000)
            addCoinButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + addCoinPrice;
        else if (addCoinPrice < 1000000)
            addCoinButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (addCoinPrice * 1f / 1000).ToString("n2") + "K";
        else
            addCoinButton.GetComponentInChildren<TextMeshProUGUI>().text = "$" + (addCoinPrice * 1f / 1000000).ToString("n2") + "M";

        if (money >= addCoinPrice && activeCoins.Count < maxNumberOfCoins)
            addCoinButton.GetComponent<Image>().material = null;
        else
            addCoinButton.GetComponent<Image>().material = uiGrayscaleMat;
    }

    public void StartMergedCoin(GameObject coinInstance)
    {
        string name = coinInstance.name;
        name = name.Remove(name.Length - 7);
        int idx = coinNamesDictionary[name];

        PathFollower pathFollowScript = coinInstance.GetComponent<PathFollower>();
        pathFollowScript.pathCreator = paths[0];
        TrailRenderer tr = coinInstance.GetComponentInChildren<TrailRenderer>();
        if (currCoinSpeed == defaultCoinSpeed * 2)
            tr.enabled = true;
        activeCoinsTrails.Add(tr);

        CoinRoll cr = coinInstance.GetComponentInChildren<CoinRoll>();
        cr.enabled = true;
        activeCoinRollScripts.Add(cr);
        /*
        CoinRoll cr = coinInstance.GetComponentInChildren<CoinRoll>();
        if (currCoinSpeed == defaultCoinSpeed * 2)
            cr.SetBoostedSpeed();
        activeCoinRollScripts.Add(cr);
        */
        currNumberOfCoins -= 2;

        CoinSpacing coinSpacing = coinInstance.GetComponent<CoinSpacing>();
        coinSpacing.SetActiveCoins(activeCoins);
        coinSpacing.SetSpeed(currCoinSpeed);
        coinSpacing.SetCoinGap(gapBetweenCoins);

        if (coinDictionary.ContainsKey(coinPrefabs[idx].name))
            coinDictionary[coinPrefabs[idx].name]++;
        else
            coinDictionary.Add(coinPrefabs[idx].name, 1);

        isMerging = false;
        camRotationScript.enabled = true;
    }

    public void NextLevel()
    {
        if (money > nextAreaPrice)
            LoadLevel((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
    void LoadLevel(int levelIndex) => SceneManager.LoadScene(Mathf.Max(0, levelIndex));
}

