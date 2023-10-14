using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour {
    //Initialize Variables
    private string nextScene;

    //Initialize Components
    private Animator anim;

    //Initialize Events
    public static event System.Action onLoadingEnd;
    
    private void OnDisable() {
        onLoadingEnd = null;
    }

    private void Start() {
        anim = GetComponent<Animator>();
        
        if (GameObject.FindObjectOfType<ClueManager>() == null) {
            anim.Play("loading_end");
        } else {
            //Subscribe to Clue Manager event
            ClueManager.onFinishLoading += () => anim.Play("loading_end");
            FinalDocument.onSubmitDocument += (sceneName) => { nextScene = sceneName; anim.Play("loading_start"); };
        }

        //Subscribe to Events
        StartMenu.onPressStart += (sceneName) => { nextScene = sceneName; anim.Play("loading_start"); };
    }

    private void OnLoadingEnd() {
        if (onLoadingEnd != null) { onLoadingEnd(); }
    }

    private void OnLoadingStart() {
        SceneManager.LoadScene(nextScene);
    }
}