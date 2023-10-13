using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StartMenu : MonoBehaviour, IPointerClickHandler {
    //Initialize Variables
    [SerializeField] private int startScene, endScene;

    //Initialize Components
    [SerializeField] private GameObject cutscene;
    [SerializeField] private Image blackScreen;

    //Initialize Events
    public delegate void LoadAction(string sceneName);
    public static event LoadAction onPressStart;

    private void OnDisable() {
        onPressStart = null;
    }

    public void Start() {
        if (blackScreen != null) { blackScreen.color = new Color(0,0,0,0); }
        
        //Subscribe to events
        CinematicManager.onSequenceEnd += (launcher) => {
            //Load next scene
            if (launcher == this.transform) {
                if (blackScreen != null) { blackScreen.color = new Color(0,0,0,1); }
                if (onPressStart != null) { onPressStart("GameScene"); }
            }
        };
    }

    public void OnPointerClick(PointerEventData data) {
        //Load Cinematic
        if (cutscene != null) {
            GameObject newCutscene = Instantiate(cutscene, this.transform);
            newCutscene.GetComponent<CinematicManager>().sceneLauncher = this.transform;
            newCutscene.GetComponent<CinematicManager>().firstScene = startScene;
            newCutscene.GetComponent<CinematicManager>().lastScene = endScene;

        }
    }

}