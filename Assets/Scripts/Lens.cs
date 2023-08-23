using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lens : MonoBehaviour {
    //Initialize Variables
    [SerializeField] private int lensSize = 200;

    //Initialize Components
    private RectTransform rect;

    private void Start() {
        rect = GetComponent<RectTransform>();

        //Resize the lens
        if (lensSize <= 0) { lensSize = 200; }
        rect.sizeDelta = new Vector2(lensSize * 2, lensSize * 2);
    }

    private void Update() {
        
    }
}
