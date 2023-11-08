using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideMenu : MonoBehaviour {
    //Initialize Variables
    private bool isOpened, isDone, gameStart, doneTutorial;
    [SerializeField] private int margin;

    //Initialize Components
    private RectTransform rect;
    private RectTransform menuButton;
    [SerializeField] private GameObject cutscene;
    [SerializeField] private RectTransform endPhone;

    //Initialize Events
    public delegate void LensAction(bool activate);
    public static event LensAction onToggleLens;

    private void OnDisable() {
        onToggleLens = null;
    }

    private void Start() {
        rect = GetComponent<RectTransform>();
        menuButton = GetComponentsInChildren<RectTransform>()[1];

        //Subscribe to events
        ClueManager.onGameStart += () => gameStart = true;

        FoundClue.onClickClue += (clueName, clueMessage) => isOpened = true;
        ClueManager.onFindAllClues += () => Invoke("FinishFindingClues", Random.Range(2.5f, 4.0f));
        ClueManager.onNextPhase += () => {
            isDone = false; isOpened = true;
            gameStart = false; Debug.Log("gamestart false");
        };

        ClueMessage.onCloseMenu += () => ToggleMenu();

        CinematicManager.onSequenceEnd += (launcher) => {
            if (launcher == this.transform) {
                if (onToggleLens != null) { onToggleLens(true); }
            }
        };
    }

    private void Update() {
        if (isOpened) {
            if (menuButton != null) { menuButton.localScale = new Vector2(-1, 1); }

            Vector2 refVelocity = Vector2.zero;
            rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, 
                new Vector2(0.0f, rect.anchoredPosition.y), ref refVelocity, 0.045f
            );
        } else {
            if (menuButton != null) { menuButton.localScale = new Vector2(1, 1); }

            Vector2 refVelocity = Vector2.zero;
            rect.anchoredPosition = Vector2.SmoothDamp(rect.anchoredPosition, 
                new Vector2(rect.sizeDelta.x - margin, rect.anchoredPosition.y), ref refVelocity, 0.045f
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

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void ToggleMenu() {
        if (gameStart) {
            if (!doneTutorial) {
                isOpened = false;
                doneTutorial = true;

                if (cutscene != null) {
                    GameObject newCutscene = Instantiate(cutscene, GameObject.FindGameObjectWithTag("Map").transform);
                    newCutscene.GetComponentsInChildren<Image>()[0].color = new Color(0,0,0,0);

                    newCutscene.GetComponent<CinematicManager>().sceneLauncher = this.transform;
                    newCutscene.GetComponent<CinematicManager>().firstScene = 19;
                    newCutscene.GetComponent<CinematicManager>().lastScene = 19;
                }
            } else {
                isOpened = !isOpened;
                if (onToggleLens != null) { onToggleLens(!isOpened); }
            }
        }
    }

    private void FinishFindingClues() => isDone = true;
}
