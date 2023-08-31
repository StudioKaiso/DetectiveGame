using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class ClueMessage : MonoBehaviour, IPointerClickHandler {
    //Initialize Variables
    [SerializeField] private float lineHeight, boxHeight;
    [field:SerializeField] public float margin { get; private set; }
    private int nameLines, bodyLines;
    private float currentSize, size, minSize, maxSize;
    private bool hasExpanded, canExpand;

    private List<ClueMessage> messages;

    //Initialize Components
    public RectTransform rect { get; private set; }
    private TextMeshProUGUI messageText;
    private Image notification;

    //Initialize Events
    public static event System.Action onClickMessage;

    private void OnDisable() {
        onClickMessage = null;
    }

    private void Awake() {
        rect = GetComponent<RectTransform>();
        messageText = GetComponentInChildren<TextMeshProUGUI>();
        notification = GetComponentsInChildren<Image>()[1];

        notification.gameObject.SetActive(false);
        
        ClueManager.onCreateMessage += (createdMessage, id, clueName, clueMessage) => {
            if (createdMessage == this) {
                gameObject.name = $"Message {id}";

                messageText.text = $"<b>{clueName}</b>";
                Canvas.ForceUpdateCanvases();
                minSize = boxHeight + (lineHeight * (messageText.textInfo.lineCount - 1));
                
                messageText.text += $"\n\n{clueMessage}";
                Canvas.ForceUpdateCanvases();
                maxSize = boxHeight + (lineHeight * (messageText.textInfo.lineCount - 1)); 
            }
        };

        ClueManager.onPlaceCluesDown += (points, clueList, messageList) => messages = messageList;

        Clue.onClueFound += (foundClue, clueName, clueMessage) => {
            if (messageText.text == $"<b>{clueName}</b>\n\n{clueMessage}") {
                canExpand = true;
                notification.gameObject.SetActive(true);
            }
        };

        FoundClue.onClickClue += (clueName, clueMessage) => {
            if (messageText.text == $"<b>{clueName}</b>\n\n{clueMessage}") {
                hasExpanded = true; size = maxSize;
               if (notification.gameObject.activeSelf) {
                    if (onClickMessage != null) { onClickMessage(); }
                    notification.gameObject.SetActive(false); 
                }
            }
        };

    }

    private void Start() {
        currentSize = size = minSize;
    }

    private void Update() {
        float vel = 0.0f;
        currentSize = Mathf.SmoothDamp(currentSize, size, ref vel, .03f);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, currentSize);
        

        //Place the message on the right spot, underneath its predecessor according to its size
        if (messages != null) {
            for (int i = 1; i < messages.Count; i++) {
                if (messages[i] == this) {
                    rect.anchoredPosition = new Vector2(
                        rect.anchoredPosition.x, 
                        messages[i-1].rect.anchoredPosition.y - (messages[i-1].rect.sizeDelta.y + margin)
                    );
                }
            }
        }
    }
    
    public void OnPointerClick(PointerEventData data) {
        if (canExpand) {
            if (!hasExpanded) { 
                hasExpanded = true; size = maxSize;

                if (notification.gameObject.activeSelf) {
                    if (onClickMessage != null) { onClickMessage(); }
                    notification.gameObject.SetActive(false); 
                }
            } else { hasExpanded = false; size = minSize; }
        } else {
            hasExpanded = false; size = minSize;
        }
        
    }
}
