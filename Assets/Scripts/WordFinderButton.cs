using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordFinderButton : MonoBehaviour {
    //Initialize Variables
    private int index = -1;
    private string word;

    //Initialize Components
    [SerializeField] private TMP_InputField inputField;

    //Initialize Events
    public delegate void WordAction(string word, int wordIndex);
    public static event WordAction onValidateWord;

    private void OnDisable() {
        onValidateWord = null;
    }

    private void Start() {
        //Subscribe to events
        FinalDocument.onClickWord += (wordIndex) => {index = wordIndex; inputField.text = string.Empty; };
        //inputField.onEndEdit.AddListener(EndEdit);
    }

    public void ValidateWord() {
        if (inputField != null) {
            if (onValidateWord != null && index != -1) { onValidateWord(inputField.text, index); }
            index = -1;
        }
    }

    private void EndEdit(string text) {
        word = inputField.text;
        inputField.text = string.Empty;
    }
}
