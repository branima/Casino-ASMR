using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public Button button;

    void Start() => button.onClick.AddListener(DestroyThisUI);


    void DestroyThisUI() => Destroy(gameObject);

}
