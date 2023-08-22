using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueManager : MonoBehaviour {
    //Initialize Variables
    private int goodPositions;
    [SerializeField] private List<Clue> clues;

    //Initialize Components
    private RectTransform map;

    //Initialize Events
    public delegate void ClueAction(RectTransform map);
    public static event ClueAction onPlaceCluesDown;

    private void OnDisable() {
        onPlaceCluesDown = null;
    }

    private void Start() {
        map = GetComponent<RectTransform>();

        //Add existing clues to the list
        foreach (GameObject clue in GameObject.FindGameObjectsWithTag("Clue")) {
            clues.Add(clue.GetComponentInParent<Clue>());
        }

        //Place clues on the map
        if (onPlaceCluesDown != null) { onPlaceCluesDown(map); }

        //Subscribe to events
        Clue.onPlaced += () => {
            goodPositions ++;

            if (goodPositions >= clues.Count - 1) {
                Debug.Log("Loading Complete!");
            }
        };
    }

    private void Update() {
        
    }

    
}
