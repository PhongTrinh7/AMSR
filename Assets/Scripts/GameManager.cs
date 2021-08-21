using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class GameManager : Manager<GameManager>
{
    //Dialogue
    public DialogueScreen dialogueScreen;
    public Dialogue[] dialogues;
    public int dialogueIndex = -1;
    public bool dialoguing;
    bool intermission;

    //Stando Powa
    public Camera cam;
    public float flipTime;
    private bool bFlip;
    Quaternion originalRoation;
    Quaternion desiredRotation;

    public GameObject fundam;
    public CinemachineVirtualCamera vcam;
    public PostProcessVolume ppv;
    public GameObject wasted;
    public GameObject lsdFilter;
    public bool gameOver;
    public GameObject ultPrompt;

    public int Enemies
    {
        get
        {
            return enemies;
        }

        set
        {
            enemies = value;
            if (enemies == 0)
            {
                StartDialogue();
                fundam.SetActive(true);
                //StopAllCoroutines();
                StartCoroutine(CameraTarget(fundam, 2f));
            }

            if (enemies == -1)
            {
                StartDialogue();
            }
        }
    }
    public int enemies;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.1f;
        Time.fixedDeltaTime = 0.02f;

        Physics.gravity = new Vector3(0, -27F, 0);

        AudioManager.Instance.Play("Music");
        AudioManager.Instance.PlayOneShot("Thunder");
        AudioManager.Instance.PlayOneShot("Rain");

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        StartDialogue();

        //originalRoation = cam.transform.rotation;
        //desiredRotation = Quaternion.Euler(20, 0, -180);
    }

    // Update is called once per frame
    void Update()
    {
        if (dialoguing)
        {
            if (Input.GetButtonDown("Jump"))
            {
                AdvanceDialogue();
            }
        }

        
        if (gameOver)
        {
            if (Input.GetButtonDown("Jump")){ 
        
                Debug.Log("Restart");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public void StartDialogue()
    {
        PauseGame();
        dialogueScreen.gameObject.SetActive(true);
        dialoguing = true;
        AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
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

        if (dialogues[dialogueIndex].intermission)
        {
            intermission = true;
        }
    }

    public void EndDialogue()
    {
        dialoguing = false;
        dialogueScreen.gameObject.SetActive(false);
        ResumeGame();
    }

    void PauseGame()
    {
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        Time.timeScale = 1.1f;
    }

    void StandoPowa()
    {
        cam.transform.rotation = Quaternion.RotateTowards(originalRoation, desiredRotation, flipTime * Time.deltaTime);

        if (cam.transform.rotation == desiredRotation)
        {
            bFlip = false;
        }
    }

    public IEnumerator CameraTarget(GameObject go, float time=2f)
    {
        while (dialoguing)
            yield return null;

        //StopAllCoroutines();

        vcam.Follow = go.transform;
        //vcam.LookAt = go.transform;

        float dist = vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;
        vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 40f;

        yield return new WaitForSeconds(time);

        vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = dist;

        vcam.Follow = AnguraController.Instance.transform;
        //vcam.LookAt = AnguraController.Instance.transform;
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 1f;
            AudioManager.Instance.Stop("Music", .1f);
            AudioManager.Instance.Play("Wasted");
            AudioManager.Instance.Play("AnglerDeath");

            foreach (Sound s in AudioManager.Instance.sounds)
            {
                if (s.name != "Grief" && s.name != "Wasted" && s.name != "AnglerDeath")
                {
                    s.source.volume = 0;
                }
            }

            ppv.enabled = true;
            Time.timeScale = .1f;
            Time.fixedDeltaTime = .1f;
            //this.enabled = false;
            wasted.SetActive(true);
            gameOver = true;
        }
        AudioManager.Instance.Play("Grief");
    }

    public IEnumerator Trip(float time)
    {
        lsdFilter.SetActive(true);

        yield return new WaitForSecondsRealtime(time);

        //lsdFilter.SetActive(false);
    }
}
