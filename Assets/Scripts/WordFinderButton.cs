using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordFinderButton : MonoBehaviour {
    //Initialize Variables
    private int index = -1;
    private string word;
    private bool canType;

    //Initialize Components
    [SerializeField] private TMP_InputField inputField;

    //Initialize Events
    public delegate void WordAction(string word, int wordIndex);
    public static event WordAction onValidateWord;

    public static event System.Action onCancelValidation;

    private void OnDisable() {
        onValidateWord = null;
        onCancelValidation = null;
    }

    private void Start() {
        //Subscribe to events
        FinalDocument.onClickWord += (wordIndex) => { 
            index = wordIndex; 
            inputField.text = string.Empty; 
            inputField.Select();
            canType = true;
        };
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return) && canType) ValidateWord();
    }

    public void CancelValidation() {
        if (onCancelValidation != null) { onCancelValidation(); }
        canType = false;
        index = -1;
    }

    public void ValidateWord() {
        if (inputField != null) {
            if (inputField.text != string.Empty && index != -1) {
                canType = false;
                if (onValidateWord != null) { onValidateWord(inputField.text, index); }    
                index = -1;
            } 
        }
    }
}
