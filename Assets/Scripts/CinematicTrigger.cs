using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicTrigger : MonoBehaviour {
    //Initialize Components
    private CinematicManager manager;

    private void Start() => manager = GetComponentInParent<CinematicManager>();
    
    public void CanClick() => manager.ToggleCanClick(true);
    public void CannotClick() => manager.ToggleCanClick(false);
    public void GoNext() {
        if (GameObject.FindObjectsOfType<CinematicTrigger>()[0] == this) {
            manager.GoToNextScene();    
        }
        
    }
}
