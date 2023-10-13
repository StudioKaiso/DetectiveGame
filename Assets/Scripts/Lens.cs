using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Lens : MonoBehaviour {
    //Initialize Variables
    [SerializeField] private int lensSize = 200;
    [SerializeField] private LayerMask lensLayer;

    private Vector2 dragOffset;
    private bool canPickUp, pickedUp;

    //Initialize Components
    private RectTransform rect;

    private void Start() {
        rect = GetComponent<RectTransform>();

        //Resize the lens
        if (lensSize <= 0) { lensSize = 200; }
        rect.sizeDelta = new Vector2(lensSize * 2, lensSize * 2);

        //Subscribe to Events
        ClueManager.onGameStart += () => canPickUp = true;
        ClueMessage.onClickTutorialClue += (target) => canPickUp = false;
        ClueManager.onNextPhase += () => canPickUp = false;
        SideMenu.onToggleLens += (activate) => canPickUp = activate;
    }

    private void Update() {
        if (canPickUp) ControlLens(); //Move the lens around when the player clicks on it
    }

    private void ControlLens() {
        if (Input.GetMouseButtonDown(0) && !pickedUp) {
            if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition),
            transform.position) < lensSize / 120.0f) {
                pickedUp = true;

                dragOffset = new Vector2(
                    transform.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                    transform.position.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y
                );    
            }
        }

        if (pickedUp) {
            if (Input.GetMouseButton(0)) {
                Vector2 dragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector2(dragPos.x + dragOffset.x, dragPos.y + dragOffset.y);
            } else {
                pickedUp = false;
            }
        }
    }
}
