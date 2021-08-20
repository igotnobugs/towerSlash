using System.Collections;
using UnityEngine;

public enum Direction { Any, Left, Right, Up, Down, Tap, Unknown, None };

public class SwipeTracker : MonoBehaviour
{
    public float minSwipeLength = 0.1f;
    private bool isTrackingSwipe = false;
    
    private IEnumerator coroutine_GetSwipes;
    private Direction trackedDirection;
  
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

        switch (touch.phase) {
            case TouchPhase.Began:
            case TouchPhase.Moved:
                break;
            case TouchPhase.Ended:
                if (swipeLength < minSwipeLength) return Direction.Tap;
                break;
        }

        if (swipeLength < minSwipeLength) return Direction.None;

        float xAbs = Mathf.Abs(deltaPos.x);
        float yAbs = Mathf.Abs(deltaPos.y);

        Direction dir;
        if (xAbs > yAbs) dir = (deltaPos.x > 0) ? Direction.Right : Direction.Left;
        else dir = (deltaPos.y > 0) ? Direction.Up : Direction.Down;   
        
        return dir;
    }
}

