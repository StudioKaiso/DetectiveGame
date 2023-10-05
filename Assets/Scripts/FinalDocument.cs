using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public class FinalDocument : MonoBehaviour, IPointerClickHandler {
    //Initialize Variables
    [SerializeField] private List<string> wordsToFind, wordsWritten;
    private List<List<TMP_CharacterInfo>> charactersToFind;

    //Initialize Components
    private TextAsset documentRef;
    private TextMeshProUGUI document;
    [SerializeField] private Canvas canvas;

    //Initialize Events
    public delegate void WordAction(int wordIndex);
    public static event WordAction onClickWord;

    private void OnDisable() {
        onClickWord = null;
    }

    private void Start() {
        charactersToFind = new List<List<TMP_CharacterInfo>>();
        wordsWritten = new List<string>();

        //Load in the final document
        document = GetComponent<TextMeshProUGUI>();
        documentRef = Resources.Load<TextAsset>("TextFiles/FinalDocument");
        GenerateDocument();

        //Subscribe to events
        WordFinderButton.onValidateWord += (wordToValidate, wordIndex) => FillInWord(wordToValidate, wordIndex);
        
    }

    private void GenerateDocument() {
        for (int i = 0; i < documentRef.text.Split('\n').Length; i ++) {
            //Find the list of words to find
            if(documentRef.text.Split('\n')[i].StartsWith("LISTE:")) {
                wordsToFind = new List<string>(
                    documentRef.text.ToLower().Split('\n')[i].Replace("liste: ", string.Empty).Split(" | ")
                );
            }
            
            //Find the first paragraph
            if(documentRef.text.Split('\n')[i].StartsWith("TEXTE:")) {
                document.text = documentRef.text.Split('\n')[i].Replace("TEXTE: ", string.Empty);
            }

            //Find and add the remaining paragraphs, starting with an indent
            if(documentRef.text.Split('\n')[i].StartsWith("PARAGRAPHE:")) {
                document.text += documentRef.text.Split('\n')[i].Replace("PARAGRAPHE: ", "\n\n");
            }
        } 
        
        //Remove the empty word at the end of the words list
        wordsToFind.RemoveAt(wordsToFind.Count - 1);

        //Add the exact same number of words to write
        foreach (string word in wordsToFind) { wordsWritten.Add(""); }

        for (int i = 0; i < wordsToFind.Count; i++) {
            //Replace the word in parentheses by underscores
            if (document.text.ToLower().Contains("(" + wordsToFind[i] + ")")) {
                string emptySpace = "";
                foreach (char character in wordsToFind[i]) { emptySpace += "_"; }

                document.text = document.text.Replace("(" + wordsToFind[i] + ")", emptySpace);

                //Force update the document so the text info is available right away
                document.ForceMeshUpdate();

                //Create a list and put the underscores in them
                List<TMP_CharacterInfo> charList = new List<TMP_CharacterInfo>();
                foreach (TMP_CharacterInfo character in document.textInfo.characterInfo) {
                    if (character.character == '_') {
                        charList.Add(character);
                    }
                } 

                //Remove every underscore that came from the previous words
                if (i > 0) {
                    foreach (string word in wordsToFind) {
                        if (word != wordsToFind[i]) { 
                            foreach (char character in word) { charList.RemoveAt(0); }    
                        }
                    }
                }

                //Add the created list to characters to find list
                if (charList.Count > 0) { charactersToFind.Add(charList); }
            }
        }
    }

    //Find the word that is closest to the position
    private int FindCorrespondingWord(Vector2 pos) {
        int result = -1;

        for (int i = 0; i < charactersToFind.Count; i++) {
            foreach (TMP_CharacterInfo character in charactersToFind[i]) {
                if (pos.x > character.bottomLeft.x - 15.0f && pos.x < character.bottomRight.x + 15.0f
                &&  pos.y > character.bottomLeft.y - 20.0f && pos.y < character.bottomLeft.y + 50.0f) {
                    result = i;
                }
            }
        }

        return result;
    }

    private void FillInWord(string word, int wordIndex) {
        //Add the word in wordsWritten list
         wordsWritten[wordIndex] = word; 

        //Insert the answer into the text at the position of the missing word
        document.text = document.text.Insert(
            charactersToFind[wordIndex][charactersToFind[wordIndex].Count - 1].index + 1, word
        );

        //Remove the underscores
        document.text = document.text.Remove(
            charactersToFind[wordIndex][0].index, charactersToFind[wordIndex].Count
        );

        document.ForceMeshUpdate();

        //Replace the corresponding charatersToFind entry with the new word
        string wordRef = "";
        int startIndex = charactersToFind[wordIndex][0].index;

        //Prepare the variables to offset the other lists
        List<TMP_CharacterInfo> oldCharList = new List<TMP_CharacterInfo>();
        int charDifference = 0;
        
        //Get the word that was just written down on the paper
        for (int i = startIndex; i < startIndex + word.Length; i++) {
            wordRef += document.textInfo.characterInfo[i].character;
        }

        //Check if it corresponds to the word that was written in the inputfield
        if (wordRef == word) {
            List<TMP_CharacterInfo> newCharList = new List<TMP_CharacterInfo>();

            //Replace the old charactersToFind entry with the word written
            for (int i = 0; i < word.Length; i++) {
                newCharList.Add(document.textInfo.characterInfo[startIndex + i]);
            }

            //Keep the old entry to determine the offset for the other charactersToFind lists
            oldCharList = charactersToFind[wordIndex];
            charactersToFind[wordIndex] = newCharList;

            charDifference = newCharList.Count - oldCharList.Count;
        }

        //Apply the offset
        for (int i = wordIndex + 1; i < charactersToFind.Count; i++) {
            List<TMP_CharacterInfo> newCharList = new List<TMP_CharacterInfo>();
            
            foreach (TMP_CharacterInfo character in charactersToFind[i]) {
                newCharList.Add(document.textInfo.characterInfo[character.index + charDifference]);
            }

            charactersToFind[i] = newCharList;
        }
    }

    public void OnPointerClick(PointerEventData data) {
        Vector2 pointerPos = document.transform.InverseTransformPoint(
            Camera.main.ScreenToWorldPoint(data.position)
        ); 

        int foundWord = FindCorrespondingWord(pointerPos);

        if (foundWord >= 0) { 
            if (onClickWord != null) { onClickWord(foundWord); }
        }
    }
}
