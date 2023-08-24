using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueManager : MonoBehaviour {
    //Initialize Variables
    private int goodPositions;

    [Header("Clues")]
    [SerializeField] private GameObject clueRef;
    [SerializeField] private List<Clue> clues;
    [SerializeField] private TextAsset cluesFile;

    //Initialize Components
    private RectTransform map;

    //Initialize Events
    public static event System.Action onFinishLoading;

    public static event System.Action onGameStart;

    public delegate void ClueAction(RectTransform map, List<Clue> clueList);
    public static event ClueAction onPlaceCluesDown;

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
        Loading.onLoadingEnd += () => { if (onGameStart != null) onGameStart(); Debug.Log("GameStart!"); };

        Clue.onPlaced += () => {
            goodPositions ++;

            if (goodPositions >= clues.Count - 1) {
                Debug.Log("Loading Complete!");
                if (onFinishLoading != null) onFinishLoading();
            }
        };
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
            clues.Add(newClue.GetComponent<Clue>());
            addedClues ++;

            if (onCreateClue != null) { 
                onCreateClue(newClue.GetComponent<Clue>(), i, nameList[i], messageList[i]); 
            }
        }

        //Place clues on the map
        if (addedClues >= nameList.Count) {
            if (onPlaceCluesDown != null) { onPlaceCluesDown(map, clues); }    
        }
    }
}
