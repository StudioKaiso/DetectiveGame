using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour {
    //Initilize Components
    public static List<string> wordsToFind, wordsWritten;
    public static string writtenDoc;

    private void Start() {
        //Keep this object through scenes
        if (GameObject.FindObjectsOfType<Score>().Length > 1) { Destroy(this.gameObject); }
        else { DontDestroyOnLoad(this.gameObject); }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
