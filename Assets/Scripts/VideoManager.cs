using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour {
    //Initialize Variables
    [SerializeField] private string videoName;

    //Initialize Components
    private CanvasGroup group;
    private VideoPlayer video;
    private RawImage output;

    //Initialize Events
    public static event System.Action onVideoEnd;

    private void OnDisable() => onVideoEnd = null;

    private void Start() {
        output = GetComponentInChildren<RawImage>();
        output.enabled = false;

        video = GetComponent<VideoPlayer>();
        video.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoName);

        this.gameObject.SetActive(false);
        group = GetComponent<CanvasGroup>();

        //Subscribe to events
        video.loopPointReached += EndVideo;

        ClueManager.onNextPhase += () => {
            this.gameObject.SetActive(true);
            StartCoroutine(PlayVideo(.5f));
        };
        
    }

    private IEnumerator PlayVideo(float targetTime) {
        float currentTime = 0;

        while (currentTime < targetTime) {
            group.alpha = Mathf.Lerp(0, 1, currentTime / targetTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        
        group.alpha = 1;
        
        output.enabled = true;
        video.Play();
    }

    private IEnumerator FadeOut(float targetTime) {
        float currentTime = 0;

        while (currentTime < targetTime) {
            group.alpha = Mathf.Lerp(1, 0, currentTime / targetTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        
        group.alpha = 0;
        if (onVideoEnd != null) { onVideoEnd(); }
        this.gameObject.SetActive(false);
    }

    private void EndVideo(VideoPlayer video) {
        output.enabled = false;
        StartCoroutine(FadeOut(.5f));
    }
}
