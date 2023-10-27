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
    private int isWritten;
    [SerializeField] private List<string> wordsToFind, wordsWritten;
    private List<List<TMP_CharacterInfo>> charactersToFind;

    //Initialize Components
    private TextAsset documentRef;
    [SerializeField] private TextMeshProUGUI document;
    private TextMeshProUGUI shownDoc;
    [SerializeField] private RectTransform parent;
    [SerializeField] private Button finishButton;

    //Initialize Events
    public delegate void WordAction(int wordIndex);
    public static event WordAction onClickWord;

    public delegate void SceneAction(string sceneName);
    public static event SceneAction onSubmitDocument;

    private void OnDisable() {
        onClickWord = null;
        onSubmitDocument = null;
    }

    private void Start() {
        charactersToFind = new List<List<TMP_CharacterInfo>>();
        wordsWritten = new List<string>();

        if (finishButton != null) { finishButton.interactable = false; }

        if (parent != null) {
            parent.anchoredPosition = new Vector2(parent.anchoredPosition.x, -(parent.sizeDelta.y + 150));
        }
        

        //Load in the final document
        shownDoc = GetComponent<TextMeshProUGUI>();
        documentRef = Resources.Load<TextAsset>("TextFiles/FinalDocument");
        if (document != null) { GenerateDocument(); }

        //Subscribe to events
        ClueManager.onRevealDocument += () => StartCoroutine(RevealDocument());

        WordFinderButton.onValidateWord += (wordToValidate, wordIndex) => {
            if (document != null) {
                FillInWord(wordToValidate, wordIndex);    
            }
        };
    }

    private IEnumerator RevealDocument() {
        Debug.Log("Moev");
        float height = -(parent.sizeDelta.y + 150);
        float timer = 0;

        while (timer < 0.75) {
            height = Mathf.Lerp(-(parent.sizeDelta.y + 150), 0, timer / 0.75f);
            parent.anchoredPosition = new Vector2(parent.anchoredPosition.x, height);
            timer += Time.deltaTime;

            yield return null;
        } 

        height = 0;
        parent.anchoredPosition = new Vector2(parent.anchoredPosition.x, height);
    }

    private void GenerateDocument() { 
        string[] textArray = documentRef.text.Split('\n');
        for (int i = 0; i < textArray.Length; i ++) {
            //Find the list of words to find
            if(documentRef.text.Split('\n')[i].StartsWith("LISTE:")) {
                wordsToFind = new List<string>(
                    documentRef.text.ToLower().Split('\n')[i].Replace("liste: ", string.Empty).Split(" | ")
                );
            }
            
            //Find the first paragraph
            if(documentRef.text.Split('\n')[i].StartsWith("TEXTE:")) {
                document.text = documentRef.text.Split('\n')[i].Replace("TEXTE: ", string.Empty);
                shownDoc.text = documentRef.text.Split('\n')[i].Replace("TEXTE: ", string.Empty);
            }

            //Find and add the remaining paragraphs, starting with an indent
            if(documentRef.text.Split('\n')[i].StartsWith("PARAGRAPHE:")) {
                document.text += documentRef.text.Split('\n')[i].Replace("PARAGRAPHE: ", "\n\n");
                shownDoc.text += documentRef.text.Split('\n')[i].Replace("PARAGRAPHE: ", "\n\n");
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
                for (int j = 0; j < 5; j++) { emptySpace += "_"; }

                document.text = document.text.Replace("(" + wordsToFind[i] + ")", emptySpace);
                shownDoc.text = document.text.Replace("(" + wordsToFind[i] + ")", emptySpace);

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
                    for (int j = 0; j < i; j++) {
                        for (int k = 0; k < 5; k++) { charList.RemoveAt(0); }
                    }
                }

                //Add the created list to characters to find list
                if (charList.Count > 0) { charactersToFind.Add(charList); }
            }
        }
    }

    private void FillInWord(string word, int wordIndex) {
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
        document.ForceMeshUpdate();
        for (int i = wordIndex + 1; i < charactersToFind.Count; i++) {
            List<TMP_CharacterInfo> newCharList = new List<TMP_CharacterInfo>();
            
            foreach (TMP_CharacterInfo character in charactersToFind[i]) {
                newCharList.Add(document.textInfo.characterInfo[character.index + charDifference]);
            }

            charactersToFind[i] = newCharList;
        }

        //Add the word in wordsWritten list
        if (wordsWritten[wordIndex] == "") { isWritten ++; }
        wordsWritten[wordIndex] = word;

        //Update the shown document with the answers in bold and underlined
        shownDoc.text = document.text;
        for (int i = 0; i < charactersToFind.Count; i++) {
            shownDoc.text = shownDoc.text.Insert(
                charactersToFind[i][0].index + (i * 47), "<font=\"Tomkin-Medium SDF\"><i><u>"
            );

            shownDoc.text = shownDoc.text.Insert(
                charactersToFind[i][0].index + (i * 47) + "<font=\"Tomkin-Medium SDF\"><i><u>".Length +
                wordsWritten[i].Length, "</u></i></font>"
            );
        }

        //Check if the player can submit the document
        if (isWritten >= 6) { 
            if (finishButton != null) { finishButton.interactable = true; }
        }
    }

    public void SubmitDocument() {
        int goodAnswers = 0;

        //Create the score object
        if (GameObject.FindObjectsOfType<Score>().Length == 0) {
            Score score = Instantiate(new GameObject(), transform.position, transform.rotation).AddComponent<Score>();

            Score.wordsToFind = wordsToFind;
            Score.wordsWritten = wordsWritten;
            Score.writtenDoc = shownDoc.text;    
        }

        //Check if the player had 4 or more good answers
        for (int i = 0; i < wordsToFind.Count; i++) {
            if (wordsWritten[i] == wordsToFind[i]) { goodAnswers ++; }
        }

        if (goodAnswers >= 4) { if (onSubmitDocument != null) { onSubmitDocument("WinEndScene"); } }
        else { if (onSubmitDocument != null) { onSubmitDocument("FailEndScene"); } }
    }

    //Find the word that is closest to the position
    private int FindCorrespondingWord(Vector2 pos) {
        int result = -1;

        for (int i = 0; i < charactersToFind.Count; i++) {
            foreach (TMP_CharacterInfo character in charactersToFind[i]) {
                if (pos.x > character.bottomLeft.x - 15.0f && pos.x < character.bottomRight.x + 15.0f
                &&  pos.y > character.bottomLeft.y - 15.0f && pos.y < character.bottomLeft.y + 40.0f) {
                    result = i;
                }
            }
        }

        return result;
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
