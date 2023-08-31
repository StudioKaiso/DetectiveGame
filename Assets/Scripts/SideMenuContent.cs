using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMenuContent : MonoBehaviour {
    //Initialize Components
    private RectTransform rect;
    private List<ClueMessage> clues;

    private void Start() {
        rect = GetComponent<RectTransform>();

        //Subscribe to events
        ClueManager.onPlaceCluesDown += (points, clueList, messageList) => clues = messageList;
    }
    private void Update() {
        if (clues != null) {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, CalculateSize());
        }
    }

    private float CalculateSize() {
        float newSize = 0;
        foreach (ClueMessage clue in clues) {
            newSize += clue.rect.sizeDelta.y + clue.margin;
        } return newSize;
    }
}
