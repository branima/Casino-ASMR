using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitchTrigger : MonoBehaviour
{
    void Start() => CameraSwitch.Instance.ChangeCamera();
}
