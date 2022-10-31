using System.Collections;
using System.Collections.Generic;
using Tabtale.TTPlugins;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    void Awake()
    {
        TTPCore.Setup(); ///Uncomment this line when the clik is integrated
        SaveData saveData = SaveSystem.LoadGame();
        int levelIdx = saveData != null ? saveData.level : 1;
        SceneManager.LoadScene(levelIdx);
    }
}
