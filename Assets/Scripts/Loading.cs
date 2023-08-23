using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour {
    //Initialize Components
    private Animator anim;

    //Initialize Events
    public static event System.Action onLoadingEnd;
    
    private void OnDisable() {
        onLoadingEnd = null;
    }

    private void Start() {
        anim = GetComponent<Animator>();

        //Subscribe to Events
        ClueManager.onFinishLoading += () => anim.Play("loading_end");
    }

    public void OnLoadingEnd() {
        if (onLoadingEnd != null) { onLoadingEnd(); }
    }
}