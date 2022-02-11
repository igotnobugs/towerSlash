using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollDown : MonoBehaviour
{
    public bool startScrollingDown = false;
    public float scrollSpeed = -1.0f;

    // Start is called before the first frame update
    private void Start() {
        GameManager.Instance.OnGameStart += StartScrollingDown;
        GameManager.Instance.OnGameOver += StopScrollingDown;
    }
    private void StartScrollingDown() {
        startScrollingDown = true;
    }

    private void StopScrollingDown() {
        startScrollingDown = false;
    }

    // Update is called once per frame
    private void Update() {
        if (!startScrollingDown) return;
        transform.Translate(0, scrollSpeed * Time.deltaTime, 0);
    }
}
