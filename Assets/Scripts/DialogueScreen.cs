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
        dialogueBox.audioSource.clip = clip;
        dialogueBox.audioSource.Play(0);
    }
}
