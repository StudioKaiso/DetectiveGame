using UnityEngine;
using UnityEngine.EventSystems;

public class OpenLink : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private string webLink;

    private void Start() {
        if (webLink == string.Empty || webLink == " ") { webLink = "www.google.com"; }
    }

    public void OnPointerClick(PointerEventData data) {
        Application.OpenURL(webLink);
    }
}