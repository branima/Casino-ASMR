using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitchTrigger : MonoBehaviour
{
    void Awake() => CameraSwitch.Instance.ChangeCamera();
}
