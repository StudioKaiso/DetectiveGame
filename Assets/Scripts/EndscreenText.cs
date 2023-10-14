using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndscreenText : MonoBehaviour {
    //Initialize Components
    [SerializeField] private TextMeshProUGUI endDoc;

    private void Start() {
        if (GameObject.FindObjectOfType<Score>() != null) {
            endDoc.text = Score.writtenDoc;

            for (int i = 0; i < Score.wordsWritten.Count; i++) {
                if (Score.wordsWritten[i] == Score.wordsToFind[i]) {
                    endDoc.text = endDoc.text.Replace(
                        $"<b><u>{Score.wordsWritten[i]}</u></b>",
                        $"<color=#287F14><b><u>{Score.wordsWritten[i]}</u></b></color>"
                    );
                } else {
                    endDoc.text = endDoc.text.Replace(
                        $"<b><u>{Score.wordsWritten[i]}</u></b>",
                        $"<color=#680300><b><u>{Score.wordsWritten[i]}</u></b></color>"
                    );
                }
            }
        }
    }
}
