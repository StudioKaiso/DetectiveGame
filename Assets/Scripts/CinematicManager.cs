using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class CinematicManager : MonoBehaviour, IPointerClickHandler {
    //Initialize Variables
    public int firstScene, lastScene;
    private int currentScene;
    private bool canClick;

    //Initialize Components
    [HideInInspector] public Transform sceneLauncher;
    [SerializeField] private CanvasGroup group, image;
    private Animator anim;

    //Initialize Events
    public delegate void SequenceAction(Transform launcher);
    public static event SequenceAction onSequenceEnd;

    private void OnDisable() => onSequenceEnd = null;

    private void Start() {
        currentScene = firstScene;

        group = GetComponent<CanvasGroup>();
        if (GetComponentsInChildren<CanvasGroup>().Length > 1) {
            image = GetComponentsInChildren<CanvasGroup>()[1];
            anim = GetComponentInChildren<Animator>();
        }

        group.alpha = 0;
        image.alpha = 0;

        anim.Play(currentScene.ToString("00"));
        anim.speed = 0;

        //Fade In the sequences
        StartCoroutine(
            ToggleSequence(group, 1, (alpha) => group.alpha = alpha,
            () => StartCoroutine(ToggleSequence(
                    image, 1, (alpha) => image.alpha = alpha, () => anim.speed = 1
                ))
            )
        );
    }

    private IEnumerator ToggleSequence(CanvasGroup target, float end, 
    System.Action<float> callback, System.Action action = null) {
        float time = 0;
        float start = target.alpha;

        if (start == end) { yield break; }

        while (time < 0.35f) {
            callback(Mathf.Lerp(start, end, time / 0.35f));
            time += Time.deltaTime;

            yield return null;
        } callback(end);

        if (action != null) { action(); }
    }

    private void DisplayNextSequence() {
        if (currentScene <= lastScene) {
            canClick = false;
            anim.Play(currentScene.ToString("00"));
        } else {
            //End sequences if theres nothing left to play
            StartCoroutine(
                ToggleSequence(image, 0, 
                    (alpha) => image.alpha = alpha,
                    () => {
                        if (onSequenceEnd != null) onSequenceEnd(sceneLauncher);
                        StartCoroutine(ToggleSequence(group, 0, (alpha) => group.alpha = alpha, 
                            () => group.blocksRaycasts = false)
                        );
                    }
                )
            );

            //Fade out the cinematics
        }
    }

    public void ToggleCanClick(bool value) => canClick = value;

    public void GoToNextScene() {
        currentScene ++;
        DisplayNextSequence();
    }

    public void OnPointerClick(PointerEventData data) {
        if (canClick) { 
            currentScene ++;
            DisplayNextSequence();
        }
    }

}
