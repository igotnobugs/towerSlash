using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platinio.UI;

public class PopUpScript : MonoBehaviour {
    public bool StartHidden = true;
    [SerializeField] private Vector2 hiddenPosition = Vector2.zero;
    [SerializeField] private Vector2 shownPosition = Vector2.zero;
    [SerializeField] private RectTransform canvas = null;
    [SerializeField] private float time = 0.5f;
    [SerializeField] private Ease enterEase = Ease.EaseInOutExpo;
    [SerializeField] private Ease exitEase = Ease.EaseInOutExpo;

    private bool isVisible = false;

    private RectTransform thisRect = null;

    private void Start() {
        thisRect = GetComponent<RectTransform>();
        isVisible = !StartHidden;
        if (StartHidden) {
            thisRect.anchoredPosition = thisRect.FromAbsolutePositionToAnchoredPosition(hiddenPosition, canvas);
            gameObject.SetActive(false);
        }
        else {
            thisRect.anchoredPosition = thisRect.FromAbsolutePositionToAnchoredPosition(shownPosition, canvas);
            gameObject.SetActive(true);
        }

    }

    public void Show() {
        if (isVisible)
            return;

        gameObject.SetActive(true);

        thisRect.MoveUI(shownPosition, canvas, time).SetEase(enterEase).SetOnComplete(delegate {
            isVisible = true;
        });

    }

    public void Hide() {
        if (!isVisible)
            return;

        thisRect.MoveUI(hiddenPosition, canvas, time).SetEase(exitEase).SetOnComplete(delegate {
            isVisible = false;
            gameObject.SetActive(false);
        });
    }

    public bool GetVisibility() {
        return isVisible;
    }

}
