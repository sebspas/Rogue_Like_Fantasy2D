using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Vector3 cameraPos;
    public Transform myPlay;
	
	// Update is called once per frame
	void Update () {
        transform.position = myPlay.position + cameraPos;
	}
}