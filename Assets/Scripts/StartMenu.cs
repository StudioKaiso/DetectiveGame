using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StartMenu : MonoBehaviour, IPointerClickHandler {
    //Initialize Events
    public delegate void LoadAction(string sceneName);
    public static event LoadAction onPressStart;

    private void OnDisable() {
        onPressStart = null;
    }

    public void OnPointerClick(PointerEventData data) {
        //Load next scene
        if (onPressStart != null) { onPressStart("GameScene"); }
    }

}