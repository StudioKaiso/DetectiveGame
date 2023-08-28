using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMenu : MonoBehaviour {
    //Initialize Variables
    private bool isOpened;

    //Initialize Components
    private RectTransform rect;

    private void Start() {
        rect = GetComponent<RectTransform>();

        //Subscribe to events
        FoundClue.onClickClue += (clueName, clueMessage) => isOpened = true;
    }

    private void Update() {
        if (isOpened) {
            Vector2 refVelocity = Vector2.zero;
            rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, 
                new Vector2(0.0f, rect.anchoredPosition.y), ref refVelocity, 0.075f
            );
        } else {
            Vector2 refVelocity = Vector2.zero;
            rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, 
                new Vector2(rect.sizeDelta.x, rect.anchoredPosition.y), ref refVelocity, 0.075f
            );
        }
    }

    public void ToggleMenu() {
        isOpened = !isOpened;
    }
}
