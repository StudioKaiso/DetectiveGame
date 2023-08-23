using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clue : MonoBehaviour {
    //Initialize Variables
    [SerializeField] private string clueName;
    [SerializeField] private string clueMessage;

    //Initialize Components
    private RectTransform rect;

    [Header("Other Clues")]
    [SerializeField] private List<RectTransform> clues;

    //Initialize Events
    public static event System.Action onPlaced;

    private void OnDisable() {
        onPlaced = null;
    }

    private void Awake() {
        rect = GetComponent<RectTransform>();

        //Subscribe to Events
        ClueManager.onCreateClue += (createdClue, id, name, message) => {
            if (createdClue == this) {
                gameObject.name = $"Clue {id}";
                clueName = name;
                clueMessage = message;
            }
        };

        ClueManager.onPlaceCluesDown += (map, clueList) => {
            clues.Clear();

            foreach (Clue clue in clueList) {
                clues.Add(clue.GetComponent<RectTransform>());
            }

            if (clues.Count == clueList.Count) { StartCoroutine(PlaceClue(map)); }
        };
        
    }

    private void Update() {
        
    }

    private IEnumerator PlaceClue(RectTransform map, float minDistance = 350.0f, int margin = 100) {
        int goodPositions = 0;

        if (clues[0] == rect) { 
            clues.Remove(clues[0]);
            if (onPlaced != null) { onPlaced(); }
            yield break; 
        }

        for (int i = 0; i < clues.Count; i++) {
            if (clues[i] == rect) { clues.Remove(clues[i]); }
        }

        while (goodPositions < clues.Count) {
            for (int i = 0; i < clues.Count; i++) {
                if (Vector2.Distance(clues[i].anchoredPosition, rect.anchoredPosition) < minDistance) {
                    rect.anchoredPosition = new Vector2(
                        Random.Range(-(map.sizeDelta.x / 2) + margin, (map.sizeDelta.x / 2) - margin),
                        Random.Range(-(map.sizeDelta.y / 2) + margin, (map.sizeDelta.y / 2) - margin)
                    );
                }   
            }

            for (int i = 0; i < clues.Count; i++) {
                if (Vector2.Distance(clues[i].anchoredPosition, rect.anchoredPosition) > minDistance) { 
                    goodPositions ++;
                } else { goodPositions = 0; }
            } yield return null;
        }

        if (onPlaced != null) { onPlaced(); }
    }
}
