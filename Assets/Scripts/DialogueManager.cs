using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class DialogueManager : MonoBehaviour
{
    //Dialogue
    public DialogueScreen dialogueScreen;
    public Dialogue[] dialogues;
    public int dialogueIndex = -1;
    bool intermission;

    public VideoPlayer vidPlayer;

    private void Update()
    {
        if (GameManager.Instance.dialoguing)
        {
            if (Input.GetButtonDown("Jump"))
            {
                AdvanceDialogue();
            }
        }
    }

    public void StartDialogue()
    {
        GameManager.Instance.PauseGame();
        dialogueScreen.gameObject.SetActive(true);
        GameManager.Instance.dialoguing = true;
        AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
        Debug.Log(dialogueIndex);

        if (intermission)
        {
            EndDialogue();
            intermission = false;
            return;
        }

        dialogueIndex++;

        if (dialogueIndex >= dialogues.Length)
        {
            EndDialogue();
            return;
        }

        if (dialogues[dialogueIndex].musicChange != "")
        {
            AudioManager.Instance.StopAll();
            AudioManager.Instance.Play(dialogues[dialogueIndex].musicChange);
        }

        if (dialogues[dialogueIndex].words == "")
        {
            dialogueScreen.dialogueBox.gameObject.SetActive(false);
        }
        else
        {
            dialogueScreen.dialogueBox.gameObject.SetActive(true);
        }

        if (dialogues[dialogueIndex].leftSpeaker == null)
        {
            dialogueScreen.left.gameObject.SetActive(false);
        }
        else
        {
            dialogueScreen.left.gameObject.SetActive(true);
            dialogueScreen.SetLeftImage(dialogues[dialogueIndex].leftSpeaker);
        }

        if (dialogues[dialogueIndex].rightSpeaker == null)
        {
            dialogueScreen.right.gameObject.SetActive(false);
        }
        else
        {
            dialogueScreen.right.gameObject.SetActive(true);
            dialogueScreen.SetRightImage(dialogues[dialogueIndex].rightSpeaker);
        }

        if (dialogues[dialogueIndex].left)
        {
            dialogueScreen.LeftSpeaking(dialogues[dialogueIndex].words, dialogues[dialogueIndex].speakerName);
        }
        else
        {
            dialogueScreen.RightSpeaking(dialogues[dialogueIndex].words, dialogues[dialogueIndex].speakerName);
        }

        dialogueScreen.Speak(dialogues[dialogueIndex].voice);

        if (dialogues[dialogueIndex].cutsceneFrame != null)
        {
            dialogueScreen.SetCutsceneFrame(dialogues[dialogueIndex].cutsceneFrame);
        }
        else
        {
            dialogueScreen.UnsetCutsceneFrame();
        }

        if (dialogues[dialogueIndex].cutscene != "")
        {
            vidPlayer.url = dialogues[dialogueIndex].cutscene;
            vidPlayer.Prepare();
            
            vidPlayer.frame = 1;
            dialogueScreen.PlayVideo();
            vidPlayer.Play();
        }
        else
        {
            vidPlayer.Stop();
            dialogueScreen.StopVideo();
        }

        if (dialogues[dialogueIndex].intermission)
        {
            intermission = true;
        }
    }

    public void EndDialogue()
    {
        GameManager.Instance.dialoguing = false;
        dialogueScreen.gameObject.SetActive(false);
        GameManager.Instance.ResumeGame();
    }
}
