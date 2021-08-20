using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlScript : MonoBehaviour {

    public Vector3 startingPosition;

    // Start is called before the first frame update
    void Start() {
        gameObject.transform.position = startingPosition;
    }

    // Update is called once per frame
    void Update() {

    }

    public void Move(Vector3 target, float time) {
        iTween.MoveTo(gameObject, target, time);
    }
}
