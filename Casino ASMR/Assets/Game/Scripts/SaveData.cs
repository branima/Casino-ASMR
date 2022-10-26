using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
[XmlRoot(ElementName = "SaveData")]
public class SaveData
{
    public int level;

    public SaveData(int level) => this.level = level;

    private SaveData() { }
}
