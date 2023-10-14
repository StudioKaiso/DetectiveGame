using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordChecker : MonoBehaviour {
    //Initialize Components
    [SerializeField] private TextMeshProUGUI word, answer;
    [SerializeField] private Image color;
    [SerializeField] private Color correct, wrong;

    private void Start() {
        if (GameObject.FindObjectOfType<Score>() != null) {
            WordChecker[] wordCheckers = GameObject.FindObjectsOfType<WordChecker>();
            for (int i = 0; i < wordCheckers.Length; i++) {
                if (wordCheckers[i] == this) {
                    wordCheckers[i].gameObject.name = $"Word Checker {i + 1}";
                    word.text = Score.wordsWritten[i];
                    answer.text = Score.wordsToFind[i];

                    if (word.text == answer.text) { color.color = correct; }
                    else { color.color = wrong; }
                }
            }
        }
        
    }

    private void Update() {
        
    }
}
