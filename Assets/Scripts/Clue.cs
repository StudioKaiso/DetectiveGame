using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Clue : MonoBehaviour {
    //Initialize Variables
    [Header("Variables")]
    [SerializeField] private string clueName;
    [SerializeField] private string clueMessage;
    public bool hasBeenFound;

    //Initialize Components
    private RectTransform rect;
    private Image clueImage;

    [Header("Clue Components")] 
    [SerializeField] private GameObject foundClue;
    private Animator anim;

    //Initialize Events
    public static event System.Action onPlaced;

    public delegate void FindClueAction(FoundClue target, string clueName, string clueMessage);
    public static event FindClueAction onClueFound;

    private void OnDisable() {
        onPlaced = null;
        onClueFound = null;
    }

    private void Awake() {
        rect = GetComponent<RectTransform>();
        clueImage = GetComponent<Image>();
        anim = GetComponent<Animator>();

        GetComponent<Collider2D>().enabled = false;

        //Subscribe to Events
        ClueManager.onCreateClue += (createdClue, id, name, message) => {
            if (createdClue == this) {
                gameObject.name = $"Clue {id}";
                clueName = name;
                clueMessage = message;
            }
        };

        ClueManager.onPlaceCluesDown += (points, clueList, messages) => {
            for(int i = 0; i < clueList.Count; i++) {
                if (clueList[i] == this) {
                    rect.anchoredPosition = points[i].anchoredPosition;
                    Invoke("FinishPlacing", 0.05f);
                }
            }
        };

        FoundClue.onClickClue += (foundName, foundMessage) => {
            if (clueName == foundName && clueMessage == foundMessage) {
                anim.Play("clue_idle");
            }
        };

        ClueManager.onGameStart += () => { 
            GetComponent<Collider2D>().enabled = true; 
        };
    }

    private void FinishPlacing() {
        if (onPlaced != null) { onPlaced(); }
    }

    private void DiscoverClue() {
        hasBeenFound = true;

        GameObject foundClueObject = Instantiate(foundClue, GameObject.FindGameObjectWithTag("Map").transform);
        foundClueObject.GetComponent<RectTransform>().anchoredPosition = rect.anchoredPosition;
        foundClueObject.transform.SetAsFirstSibling();
        foundClueObject.name = $"Found {gameObject.name}";

        if (onClueFound != null) {
            onClueFound(foundClueObject.GetComponent<FoundClue>(), clueName, clueMessage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Lens") { 
            if (!hasBeenFound) { DiscoverClue(); }
        }
    }
}
