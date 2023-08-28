using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueManager : MonoBehaviour {
    //Initialize Variables
    private int goodPositions;

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

    //Initialize Events
    public static event System.Action onFinishLoading;

    public static event System.Action onGameStart;

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
    }

    private void Start() {
        map = GetComponent<RectTransform>();

        //Load Clues
        cluesFile = Resources.Load<TextAsset>("TextFiles/Clues");
        LoadClues();

        //Subscribe to events
        Loading.onLoadingEnd += () => { Invoke("StartGame", 0.1f); };

        Clue.onPlaced += () => {
            goodPositions ++;

            if (goodPositions >= clues.Count) {
                Debug.Log("Loading Complete!");
                if (onFinishLoading != null) onFinishLoading();
            }
        };
    }

    private void StartGame() { if (onGameStart != null) onGameStart(); Debug.Log("GameStart!"); }

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
}
