using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordFinderBox : MonoBehaviour {
    //Initalize Variables
    private Vector2 targetPos;

    //Initialize Components
    private RectTransform rect;

    private void Start() {
        targetPos = new Vector2(targetPos.x, -1000.0f);
        rect = GetComponent<RectTransform>();

        //Subscribe to events
        FinalDocument.onClickWord += (wordIndex) => TogglePosition();
        WordFinderButton.onValidateWord += (wordToValidate, wordIndex) => TogglePosition(false);
        
    }

    private void Update() {
        Vector2 refVelocity = Vector2.zero;
        rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, targetPos, ref refVelocity, 0.05f);
    }

    private void TogglePosition(bool value = true) {
        if (value) { targetPos = Vector2.zero; }
        else { targetPos = new Vector2(targetPos.x, -1000.0f); }
    }
}
