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

    // Update is called once per frame
    void Update()
    {
        foreach (PathFollower item in activeCoins)
        {
            float currPos = item.GetDistanceTraveled() % item.pathCreator.path.length;
            if (!(currPos >= coinGap && currPos <= (item.GetDistanceTraveled() - coinGap)))
                return;
        }

        PathFollower pathFollowScript = GetComponent<PathFollower>();
        pathFollowScript.speed = speed;

        Destroy(this);
    }

    public void SetActiveCoins(List<PathFollower> activeCoins) => this.activeCoins = activeCoins;
    public void SetSpeed(float speed) => this.speed = speed;
    public void SetCoinGap(float coinGap) => this.coinGap = coinGap;


}
