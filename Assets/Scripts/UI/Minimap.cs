using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    // Use this for initialization
    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate()
	{
        Vector3 newPosition = Camera.main.transform.position;
        transform.position = newPosition;
        Debug.Log("x: " + transform.position.x + "y: " + transform.position.y);

        // transform.rotation = Quaternion.Euler(90f, Camera.allCameras[1].transform.eulerAngles.y, 0f);
    }
}
