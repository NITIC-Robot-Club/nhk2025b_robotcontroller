using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class ButtonAudio : MonoBehaviour {
    private bool isMute = false;
    private AudioSource audioSource;
    //public Button muteToggleButton;
    //public AudioClip bgmClip;
    public List<ButtonClipPair> buttonClipPairs;

    [System.Serializable]
    public class ButtonClipPair {
        public Button button;
        public AudioClip clip;
    }

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        
        foreach (ButtonClipPair pair in buttonClipPairs) {
            if (pair.button != null && pair.clip != null) {
                AudioClip clip = pair.clip;
                pair.button.onClick.AddListener(() => PlaySound(clip));
            }
        }

        /*if (bgmClip != null) {
            audioSource.clip = bgmClip;
            audioSource.loop = true;
            audioSource.Play();
        }*/
    }

    void Start() {
        audioSource.spatialBlend = 0;
    }

    void Update() {
        //isMute = muteToggleButton.GetComponent<Toggle>().GetisAwake();
        //MuteAction();
    }

    private void PlaySound(AudioClip clip) {
        audioSource.PlayOneShot(clip);
    }

    /*public void MuteAction() {
        if (isMute) {
            audioSource.volume = 0;
        }
        else {
            audioSource.volume = 1;
        }
    }*/
}
