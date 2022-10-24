using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation.Examples;
using PathCreation;

public class CoinSpacing : MonoBehaviour
{

    List<PathFollower> activeCoins;
    float coinGap;
    float speed;

    public CoinRoll coinRollScript;

    // Update is called once per frame
    void Update()
    {
        if (!(activeCoins != null && coinGap != 0f))
            return;

        foreach (PathFollower item in activeCoins)
        {
            float currPos = item.GetDistanceTraveled() % item.pathCreator.path.length;
            if (!(currPos >= coinGap && currPos <= (item.pathCreator.path.length - coinGap)))
                return;
        }

        PathFollower pathFollowScript = GetComponent<PathFollower>();
        if (activeCoins.Count > 0)
            pathFollowScript.speed = activeCoins[0].speed;
        else
            pathFollowScript.speed = speed;

        activeCoins.Add(pathFollowScript);

        coinRollScript.enabled = true;
        coinRollScript.SetCustomSpeed(activeCoins[0].GetComponentInChildren<CoinRoll>().GetCurrSpeed());

        activeCoins = null;
        Destroy(this);
    }

    public void SetActiveCoins(List<PathFollower> activeCoins) => this.activeCoins = activeCoins;
    public void SetSpeed(float speed) => this.speed = speed;
    public void SetCoinGap(float coinGap) => this.coinGap = coinGap;


}
