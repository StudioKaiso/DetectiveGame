using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour {
    //Initialize Components
    private CanvasGroup group;

    //Initialize Events
    public static event System.Action onVideoEnd;

    private void OnDisable() => onVideoEnd = null;

    private void Start() {
        this.gameObject.SetActive(false);
        group = GetComponent<CanvasGroup>();

        //Subscribe to events
        ClueManager.onNextPhase += () => {
            this.gameObject.SetActive(true);
            StartCoroutine(PlayVideo(.5f, "videoTest.mp4"));
        };
        
    }

    private void StartVideo(string videoName) {
        VideoPlayer video = GetComponent<VideoPlayer>();
        video.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoName);
        video.Play();

        Invoke("EndVideo", 7);
    }

    private IEnumerator PlayVideo(float targetTime, string videoName) {
        float currentTime = 0;

        while (currentTime < targetTime) {
            group.alpha = Mathf.Lerp(0, 1, currentTime / targetTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        
        group.alpha = 1;
        StartVideo(videoName);
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

    private void EndVideo() {
        Debug.Log("End");
        StartCoroutine(FadeOut(.5f));
    }
}
