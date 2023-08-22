using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clue : MonoBehaviour {
    //Initialize Components
    private RectTransform rect;
    [SerializeField] private List<RectTransform> clues;

    //Initialize Events
    public static event System.Action onPlaced;

    private void OnDisable() {
        onPlaced = null;
    }

    private void Start() {
        rect = GetComponent<RectTransform>();

        foreach (GameObject clue in GameObject.FindGameObjectsWithTag("Clue")) {
            clues.Add(clue.GetComponentInParent<RectTransform>());
        }

        //Subscribe to Events
        ClueManager.onPlaceCluesDown += (map) => StartCoroutine(PlaceClue(map)); 
        
    }

    private void Update() {
        
    }

    private IEnumerator PlaceClue(RectTransform map, float minDistance = 350.0f, int margin = 100) {
        int goodPositions = 0;

        if (clues[0] == rect) { 
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
