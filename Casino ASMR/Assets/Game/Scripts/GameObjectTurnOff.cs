using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTurnOff : MonoBehaviour
{
    public void TurnOff() => transform.parent.gameObject.SetActive(false);
}
