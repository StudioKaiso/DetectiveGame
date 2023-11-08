using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicTrigger : MonoBehaviour {
    //Initialize Components
    [SerializeField] private CinematicManager manager;
    
    public void CanClick() => manager.ToggleCanClick(true);
    public void CannotClick() => manager.ToggleCanClick(false);
    public void GoNext() {
        if (GameObject.FindObjectsOfType<CinematicTrigger>()[0] == this) {
            manager.GoToNextScene();    
        }
        
    }
}
