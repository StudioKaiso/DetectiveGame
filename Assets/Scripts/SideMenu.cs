using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMenu : MonoBehaviour {
    //Initialize Variables
    private bool isOpened, isDone;

    //Initialize Components
    private RectTransform rect;
    [SerializeField] private RectTransform endPhone;

    private void Start() {
        rect = GetComponent<RectTransform>();

        //Subscribe to events
        FoundClue.onClickClue += (clueName, clueMessage) => isOpened = true;
        ClueManager.onFindAllClues += () => Invoke("FinishFindingClues", Random.Range(2.5f, 4.0f));
        ClueManager.onNextPhase += () => isDone = false;
    }

    private void Update() {
        if (isOpened) {
            Vector2 refVelocity = Vector2.zero;
            rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, 
                new Vector2(0.0f, rect.anchoredPosition.y), ref refVelocity, 0.045f
            );
        } else {
            Vector2 refVelocity = Vector2.zero;
            rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, 
                new Vector2(rect.sizeDelta.x, rect.anchoredPosition.y), ref refVelocity, 0.045f
            );
        }

        if (isDone) { 
            Vector2 refVelocity = Vector2.zero;
            endPhone.anchoredPosition = Vector2.SmoothDamp(endPhone.anchoredPosition, 
                new Vector2(endPhone.sizeDelta.x / 1.5f, endPhone.anchoredPosition.y), ref refVelocity, 0.045f
            );
        } else {
            Vector2 refVelocity = Vector2.zero;
            endPhone.anchoredPosition = Vector2.SmoothDamp(endPhone.anchoredPosition, 
                new Vector2(-endPhone.sizeDelta.x / 1.5f, endPhone.anchoredPosition.y), ref refVelocity, 0.045f
            );
        }
    }

    public void ToggleMenu() {
        isOpened = !isOpened;
    }

    private void FinishFindingClues() => isDone = true;
}
