using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TapToSpeedUpTutorialLogic : MonoBehaviour
{

    public GameObject arrow1;
    public GameObject arrow2;

    public TextMeshProUGUI tmproText;

    // Update is called once per frame
    void Update()
    {
        if (arrow1 == null && arrow2 == null)
            tmproText.enabled = true;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            Destroy(gameObject);
    }
}
