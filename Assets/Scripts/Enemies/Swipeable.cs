using System.Collections;
using UnityEngine;

/* Arrows */

public class Swipeable : MonoBehaviour 
{
    public enum ArrowType { Any, Literal, Opposite, Rotating, RotatingClockwise,
        RandomArrow };

    public Transform arrowPosition;
    public ArrowDesign arrowDesign = null;
    public ArrowType arrowType = ArrowType.Literal;
    public float timeToAppear = 0.0f;
    //public float rangeToInteract = 3.0f;   
    public float rotateSpeed = 0.5f;

    public bool isTargeted = false;
    public bool isFailed = false;
    protected bool arrowPhaseDone = false;
    protected int arrowRotation = 0;

    private GameObject player;
    private GameObject arrow = null;
    private SpriteRenderer arrowSprite;
    private ParticleSystem colorParticle = null;

    // 1 is up, counter clockwise, to 4
    public delegate void SwipeableEvent(Swipeable swipeable);
    public event SwipeableEvent OnPassedThreshold;
    public event SwipeableEvent InRange;

    public float yThreshold = 0.0f;
    public float yInRange = 0.0f;

    public void LateUpdate() {
        if (transform.position.y < yInRange) {
            InRange?.Invoke(this);
        }

        if (transform.position.y < yThreshold) {
            OnPassedThreshold?.Invoke(this);
        }
    }

    protected void StartArrowPhase() {
        player = GameObject.FindGameObjectWithTag("Player");

        arrow = Instantiate(arrowDesign.arrowObject, arrowPosition.transform);
        arrowSprite = arrow.GetComponent<SpriteRenderer>();

        switch (arrowType) {
            case ArrowType.Any:
                StartCoroutine(Any());
                break;
            case ArrowType.Literal:
                StartCoroutine(Literal());
                break;
            case ArrowType.Opposite:
                StartCoroutine(Opposite());
                break;
            case ArrowType.Rotating:
                StartCoroutine(Rotating());
                break;
            case ArrowType.RotatingClockwise:
                StartCoroutine(Rotating(true));
                break;
            case ArrowType.RandomArrow:
                StartCoroutine(RandomArrow());
                break;
        }
    }

    #region Arrows Phases

    // Changes the sprite into a dot
    private IEnumerator Any() {
        
        yield return new WaitForSeconds(timeToAppear);
      
        arrowSprite.sprite = arrowDesign.dotSprite;
        ChangeColor(arrowDesign.colorIsLiteral);

        /*bool isInRange = false;
        while (!isInRange) {
            if (player.transform.position.y + rangeToInteract < transform.position.y) {
                yield return new WaitForSeconds(0.01f);
            } else {
                isInRange = true;
                InRanged?.Invoke(this);
            }
        }*/

        MarkInteractable(arrowDesign.colorIsLiteral);
        arrowPhaseDone = true;
        arrowRotation = 0;

        yield return null;
    }
    
    // Assigns Once
    private IEnumerator Literal() {       
        yield return new WaitForSeconds(timeToAppear);

        arrowSprite.sprite = arrowDesign.arrowSprite;
        ChangeColor(arrowDesign.colorIsLiteral);

        arrowRotation = Random.Range(1, 5);
        arrow.transform.Rotate(0, 0, 90 * arrowRotation);

        MarkInteractable(arrowDesign.colorIsLiteral);
        arrowPhaseDone = true;

        yield return null;
    }

    private IEnumerator Opposite() {
        yield return new WaitForSeconds(timeToAppear);

        arrowSprite.sprite = arrowDesign.arrowSprite;
        ChangeColor(arrowDesign.colorIsOpposite);

        arrowRotation = Random.Range(1, 5);
        arrow.transform.Rotate(0, 0, 90 * arrowRotation);

        arrowRotation += 2;
        if (arrowRotation > 4) arrowRotation -= 4;

        MarkInteractable(arrowDesign.colorIsOpposite);
        arrowPhaseDone = true;

        yield return null;
    }

    // Rotate
    private IEnumerator coroutine_RotateSprite; 
    private IEnumerator Rotating(bool clockwise = false) {
        arrowSprite.sprite = arrowDesign.arrowSprite;
        ChangeColor(arrowDesign.colorIsLiteral);
    
        arrowRotation = Random.Range(1, 5);
        arrow.transform.Rotate(0, 0, 90 * arrowRotation);

        /*bool isInRange = false;
        bool isCoroutineStarted = false;
        while (!isInRange) {
            if (player.transform.position.y + rangeToInteract < transform.position.y) {

                if (!isCoroutineStarted) {
                    coroutine_RotateSprite = RotateSprite(clockwise);
                    StartCoroutine(coroutine_RotateSprite);
                    isCoroutineStarted = true;
                }
                yield return new WaitForSeconds(0.01f);
            }
            else {
                StopCoroutine(coroutine_RotateSprite);
                isInRange = true;
            }
        }*/

        MarkInteractable(arrowDesign.colorIsLiteral);

        arrowPhaseDone = true;
        yield return null;
    }

    private IEnumerator RotateSprite(bool clockwise = false) {
        for (; ; ) {
            if (clockwise) {
                arrowRotation -= 1;
                if (arrowRotation < 1) arrowRotation += 4;
                arrow.transform.Rotate(0, 0, -90);
            }
            else {
                arrowRotation += 1;
                if (arrowRotation > 4) arrowRotation -= 4;
                arrow.transform.Rotate(0, 0, 90);
            }
            yield return new WaitForSeconds(rotateSpeed);
        }
    }

    // Randomly choose any of the arrow types
    private IEnumerator RandomArrow() {
        int choose = Random.Range(0, 5);

        switch (choose) {
            case 0:
                arrowType = ArrowType.Any;
                StartCoroutine(Any());
                break;
            case 1:
                arrowType = ArrowType.Literal;
                StartCoroutine(Literal());
                break;
            case 2:
                arrowType = ArrowType.Rotating;
                StartCoroutine(Rotating());
                break;
            case 3:
                arrowType = ArrowType.RotatingClockwise;
                StartCoroutine(Rotating(true));
                break;
            case 4:
                arrowType = ArrowType.Opposite;
                StartCoroutine(Opposite());
                break;
        }

        yield return null;
    }
    #endregion

    
    public virtual Direction GetTargetDirection() {
        if (!arrowPhaseDone) return Direction.None;
        
        switch (arrowRotation) {
            case 0:
                return Direction.Any;
            case 1:
                return Direction.Up;
            case 2:
                return Direction.Left;
            case 3:
                return Direction.Down;
            case 4:
                return Direction.Right;
            default:
                return Direction.None;
        }
    }

    public void MarkAsSuccess() {
        isTargeted = true;
        ChangeColor(arrowDesign.colorIsCorrect);
    }

    public void MarkAsFailed() {
        isFailed = true;
        ChangeColor(arrowDesign.colorIsWrong);
    }

    private void MarkInteractable(Color color) {
        colorParticle = Instantiate(arrowDesign.particle, arrowSprite.transform);
        ChangeColor(color);
    }

    private void ChangeColor(Color color) {
        arrowSprite.color = color;

        if (colorParticle == null) return;
        var main = colorParticle.main;
        main.startColor = color;
    }
}