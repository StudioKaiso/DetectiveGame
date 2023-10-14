using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ClueManager : MonoBehaviour {
    //Initialize Variables
    private int goodPositions, clickedClues;
    private int phase;

    [Header("Clues")]
    [SerializeField] private GameObject clueRef;
    [SerializeField] private GameObject messageRef;
    [SerializeField] private RectTransform sideMenu;
    [SerializeField] private List<Clue> clues;
    [SerializeField] private List<ClueMessage> messages;
    [SerializeField] private List<RectTransform> points;
    [SerializeField] private TextAsset cluesFile;

    //Initialize Components
    private RectTransform map;
    [Header("Cutscene Info")]
    [SerializeField] private GameObject cutscene;

    //Initialize Events
    public static event System.Action onFinishLoading, onGameStart;
    public static event System.Action onFindAllClues, onNextPhase, onRevealDocument;

    public delegate void PlaceClueAction(List<RectTransform> points, List<Clue> clueList, List<ClueMessage> messages);
    public static event PlaceClueAction onPlaceCluesDown;

    public delegate void GenerateMessage(ClueMessage generatedMessage, int id, string name, string message);
    public static event GenerateMessage onCreateMessage;

    public delegate void GenerateClue(Clue generatedClue, int id, string name, string message);
    public static event GenerateClue onCreateClue;

    private void OnDisable() {
        onFinishLoading = null;
        onGameStart = null;
        onPlaceCluesDown = null;
        onCreateClue = null;
        onCreateMessage = null;
        onFindAllClues = null;
        onNextPhase = null;
        onRevealDocument = null;
    }

    private void Start() {
        map = GetComponent<RectTransform>();

        //Load Clues
        cluesFile = Resources.Load<TextAsset>("TextFiles/Clues");
        LoadClues();

        //Subscribe to events
        Loading.onLoadingEnd += () => StartCutscene(this.transform, 13, 16);

        Clue.onPlaced += () => {
            goodPositions ++;

            if (goodPositions >= clues.Count) {
                if (onFinishLoading != null) onFinishLoading();
            }
        };

        ClueMessage.onClickMessage += () => {
            clickedClues ++;
            Debug.Log(clickedClues);

            if (clickedClues >= clues.Count) {
                if (onFindAllClues != null) { onFindAllClues(); }
            }
        };

        ClueMessage.onClickTutorialClue += (target) => StartCutscene(target.transform, 17);

        CinematicManager.onSequenceEnd += (launcher) => {
            //Load next scene
            if (launcher == this.transform) {
                if (phase == 0) { StartGame(); phase = 1; }
                if (phase == 2) { phase = 3; if (onRevealDocument != null) { onRevealDocument(); } }
            }
        };
    }

    private void StartGame() { if (onGameStart != null) onGameStart(); }

    private void StartCutscene(Transform target, int startScene, int endScene = -1, bool isTransparent = true) {
        if (endScene < 0) { endScene = startScene; }

        if (cutscene != null) {
            GameObject newCutscene = Instantiate(cutscene, GameObject.FindGameObjectWithTag("Map").transform);

            if (isTransparent) {
                newCutscene.GetComponentsInChildren<Image>()[0].color = new Color(0,0,0,0);
            }
            
            newCutscene.GetComponent<CinematicManager>().sceneLauncher = target.transform;
            newCutscene.GetComponent<CinematicManager>().firstScene = startScene;
            newCutscene.GetComponent<CinematicManager>().lastScene = endScene;
        }
    }

    private void LoadClues() {
        int addedClues = 0;
        string nameSplitter = "- ";
        string messageSplitter = "    > ";

        string[] file = cluesFile.text.Split('\n');

        List<string> nameList = new List<string>();
        List<string> messageList = new List<string>();

        //Read through the textfile and find the clues
        for (int i = 0; i < file.Length; i++) {
            if (file[i].StartsWith(nameSplitter)) { 
                nameList.Add(file[i].Replace(nameSplitter, string.Empty)); 
            }

            if (file[i].StartsWith(messageSplitter)) { 
                messageList.Add(file[i].Replace(messageSplitter, string.Empty)); 
            }
        }

        //Generate clues from textfile
        for (int i = 0; i < nameList.Count; i++) {   
            //Create clue object
            GameObject newClue = Instantiate(clueRef, this.transform);
            GameObject newClueMessage = Instantiate(messageRef, sideMenu.transform);
            clues.Add(newClue.GetComponent<Clue>());
            messages.Add(newClueMessage.GetComponent<ClueMessage>());
            addedClues ++;

            if (onCreateClue != null) { 
                onCreateClue(newClue.GetComponent<Clue>(), i, nameList[i], messageList[i]); 
            }

            if (onCreateMessage != null) { 
                onCreateMessage(newClueMessage.GetComponent<ClueMessage>(), i, nameList[i], messageList[i]); 
            }
        }

        //Place clues on the map
        if (addedClues >= nameList.Count) {
            if (onPlaceCluesDown != null) { onPlaceCluesDown(points, clues, messages); }    
        }
    }
    
    public void GoToNextPhase() {
        if (onNextPhase != null) { onNextPhase(); }
        phase = 2;
        StartCutscene(this.transform, 19, 32, false);
    }
}
