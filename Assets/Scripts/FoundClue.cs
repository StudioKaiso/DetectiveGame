using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FoundClue : MonoBehaviour, IPointerClickHandler {
    //Initialize Variables
    [Header("Variables")]
    [SerializeField] private string foundName;
    [SerializeField] private string foundMessage;
    private bool gameStart;

    //Initialize Components
    private RectTransform rect;

    //Initialize Events
    public delegate void FoundClueAction(string clueName, string clueMessage);
    public static event FoundClueAction onClickClue;

    private void OnDisable() {
        onClickClue = null;
    }

    private void Awake() {
        rect = GetComponent<RectTransform>();

        //Subscribe to events
        Clue.onClueFound += (target, clueName, clueMessage) => {
            if (target == this) {
                foundName = clueName;
                foundMessage = clueMessage;

                if (gameObject.name.Contains("Clue 0") && !gameStart) { 
                    gameStart = true;
                    if (onClickClue != null) { onClickClue(foundName, foundMessage); } 
                }
            }
        };
    }

    public void OnPointerClick(PointerEventData data) {
        if (onClickClue != null) { onClickClue(foundName, foundMessage); }
    }
}
