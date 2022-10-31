using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomOutTrigger : MonoBehaviour
{
    void Start() => CameraZoom.Instance.ChangeCamera();
}
