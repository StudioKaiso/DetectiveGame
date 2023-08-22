using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifiedMap : MonoBehaviour {
    //Initialize Components
    private RectTransform rect;
    [SerializeField] private RectTransform parentMap;

    
    private void Start() {
        rect = GetComponent<RectTransform>();
        
        //Resize the map to fit the screen
        if (parentMap != null) {
            rect.sizeDelta = parentMap.sizeDelta;
            rect.localScale = parentMap.localScale;
        }
    }
}
