using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class GameManager : Manager<GameManager>
{
    //Dialogue
    public bool dialoguing;
    public DialogueManager dialogueManager;

    //Stando Powa
    public Camera cam;

    public GameObject fundam;
    public CinemachineVirtualCamera vcam;
    public PostProcessVolume ppv;
    public GameObject wasted;
    public GameObject lsdFilter;
    public bool gameOver;
    public GameObject ultPrompt;

    public CinemachineTargetGroup targetGroup;
    public CinemachineImpulseSource impulseSource;

    public int Enemies
    {
        get
        {
            return enemies;
        }

        set
        {
            Debug.Log(enemies);
            enemies = value;
            if (enemies == 0)
            {
                dialogueManager.StartDialogue();
                fundam.SetActive(true);
                StartCoroutine(CameraTarget(fundam, 2f));
            }

            if (enemies == -5)
            {
                dialogueManager.StartDialogue();
            }
        }
    }
    public int enemies;

    public GameObject player1;
    public GameObject player2;
    public GameObject player2TextPrompt;
    public GameObject player2Bars;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.1f;
        Time.fixedDeltaTime = 0.015f;

        Physics.gravity = new Vector3(0, -27F, 0);
        Physics.IgnoreLayerCollision(8, 8);
        Physics.IgnoreLayerCollision(14, 15);

        AudioManager.Instance.Play("Music");
        AudioManager.Instance.Play("Thunder");
        AudioManager.Instance.Play("Rain");

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;

        dialogueManager.StartDialogue();

        //originalRoation = cam.transform.rotation;
        //desiredRotation = Quaternion.Euler(20, 0, -180);
    }

    // Update is called once per frame
    void Update()
    {        
        if (gameOver)
        {
            if (Input.GetButtonDown("Jump"))
            {
                //SetScene();

            }
        }

        if (!player2.activeInHierarchy && Input.GetButtonDown("Mire1"))
        {
            PlayerTwoJoin();
        }

        if (Input.GetKeyDown("o"))
        {
            impulseSource.GenerateImpulse();
            ScreenLock();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.1f;
    }

    /*
    void StandoPowa()
    {
        cam.transform.rotation = Quaternion.RotateTowards(originalRoation, desiredRotation, flipTime * Time.deltaTime);

        if (cam.transform.rotation == desiredRotation)
        {
            bFlip = false;
        }
    }
    */

    public void PlayerTwoJoin()
    {
        player2TextPrompt.SetActive(false);
        player2Bars.SetActive(true);
        player2.transform.position = player1.transform.position;
        player2.SetActive(true);
        //Instantiate(player2, player1.transform.position, Quaternion.identity);
        targetGroup.m_Targets[1].weight = 1;
    }

    public IEnumerator CameraTarget(GameObject go, float time=2f)
    {
        while (dialoguing)
            yield return null;

        vcam.Follow = go.transform;

        float dist = vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;
        vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 40f;

        yield return new WaitForSeconds(time);

        vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = dist;

        vcam.Follow = targetGroup.transform;
    }

    public void ScreenLock()
    {
        vcam.Follow = null;
        //Physics.IgnoreCollision();
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
            wasted.SetActive(true);
            gameOver = true;
        }
        AudioManager.Instance.Play("Grief");
    }

    public IEnumerator Trip(float time)
    {
        impulseSource.GenerateImpulse();
        lsdFilter.SetActive(true);

        yield return new WaitForSecondsRealtime(time);

        //lsdFilter.SetActive(false);
    }

    public void SetScene(int i)
    {
        StartCoroutine(SetSceneCo());
    }

    private IEnumerator SetSceneCo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return null;
    }
}
