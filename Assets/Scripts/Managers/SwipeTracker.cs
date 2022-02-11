using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public enum Direction { Any, Left, Right, Up, Down, Tap, Unknown, None };

public class SwipeTracker : Singleton<SwipeTracker> 
{
    public float minSwipeLength = 0.1f;
    private bool isTrackingSwipe = false;
    
    private IEnumerator coroutine_GetSwipes;
    private Direction trackedDirection;

    private InputActions inputAction;

    public delegate void SwipeEvent();
    public event SwipeEvent OnTap;
    //public event SwipeEvent OnDeactivating;

    private Vector2 initialTapPosition;

    private void Awake() {
        inputAction = new InputActions();
    }

    private void Start() {
        inputAction.Mouse.Tap.started += ctx => MouseStarted(ctx);
        inputAction.Mouse.Tap.performed += ctx => MouseTap(ctx);
        //inputAction.Mouse.MousePositionDelta.performed
        inputAction.Enable();
    }

    private void MouseStarted(InputAction.CallbackContext ctx) {
        //OnTap?.Invoke();
        //Debug.Log("End");
        // Debug.Log(inputAction.Mouse.curr);
        //Debug.Log(inputAction.Mouse.MousePosition.ReadValue<Vector2>());
    }

    private void MouseTap(InputAction.CallbackContext ctx) {
        OnTap?.Invoke();
        //Debug.Log("Tapped");
        //isMouseTap = true;
    }

    public void StartTracking() {
        if (isTrackingSwipe) return;

        coroutine_GetSwipes = TrackSwipe();
        StartCoroutine(coroutine_GetSwipes);
        isTrackingSwipe = true;
    }

    public void StopTracking() {       
        isTrackingSwipe = false;
        StopCoroutine(coroutine_GetSwipes);
    }

    public Direction GetSwipeDirection() {
        return trackedDirection;
    }

    private IEnumerator TrackSwipe() {
        for(; ;) {
            trackedDirection = GetDirection();
            yield return new WaitForSeconds(0.01f);
        }
    }

    private Direction GetDirection() {
        if (Input.touchCount > 0) {
            Direction dir = GetSwipeDirection(Input.GetTouch(0));
            return dir;
        }
        return Direction.None;
    }

    private Direction GetSwipeDirection(Touch touch) {
        Vector2 deltaPos = touch.deltaPosition;
        float swipeLength = deltaPos.magnitude;

        //switch (touch.phase) {
            //case TouchPhase.Began:
            //case TouchPhase.Moved:
            //    break;
            //case TouchPhase.Ended:
            //    if (swipeLength < minSwipeLength) return Direction.Tap;
             //   break;
        //}

        if (swipeLength < minSwipeLength) return Direction.None;

        float xAbs = Mathf.Abs(deltaPos.x);
        float yAbs = Mathf.Abs(deltaPos.y);

        Direction dir;
        if (xAbs > yAbs) dir = (deltaPos.x > 0) ? Direction.Right : Direction.Left;
        else dir = (deltaPos.y > 0) ? Direction.Up : Direction.Down;   
        
        return dir;
    }
}

