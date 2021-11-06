using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueScreen : MonoBehaviour
{
    public Image left;
    public Image right;
    public DialogueBox dialogueBox;
    public Color32 speakingColor;
    public Color32 fadeColor;
    public GameObject cutsceneImage;
    public AudioSource audioSource;
    public RawImage cutscene;

    public void SetLeftImage(Sprite portrait)
    {
        left.sprite = portrait;
    }

    public void SetRightImage(Sprite portrait)
    {
        right.sprite = portrait;
    }

    public void LeftSpeaking(string s, string n)
    {
        left.color = speakingColor;
        right.color = fadeColor;
        dialogueBox.speakerName.text = n;
        dialogueBox.dialogue.text = s;
    }

    public void RightSpeaking(string s, string n)
    {
        right.color = speakingColor;
        left.color = fadeColor;
        dialogueBox.speakerName.text = n;
        dialogueBox.dialogue.text = s;
    }

    public void Speak(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play(0);
    }

    public void SetCutsceneFrame(Sprite frame)
    {
        cutsceneImage.SetActive(true);
        cutsceneImage.transform.GetChild(0).GetComponent<Image>().sprite = frame;
    }

    public void UnsetCutsceneFrame()
    {
        cutsceneImage.SetActive(false);
        cutsceneImage.transform.GetChild(0).GetComponent<Image>().sprite = null;
    }

    public void PlayVideo()
    {
        cutscene.gameObject.SetActive(true);
    }

    public void StopVideo()
    {
        cutscene.gameObject.SetActive(false);
    }
}
