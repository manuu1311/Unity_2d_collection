using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private float Score;
    public TextMeshProUGUI score;
    public GameObject playButton;
    public GameObject gameOverImg;
    public Bird_script bird;
    public SoundManager soundManager;
    public GameObject homeButton;
    public ExplosionManager explosion;
    public Image healthbar;
    //flag to see if this is the very beginning (for respawn)
    private bool firstSpawn;
    //speed of items
    public static float pipeSpeed=5f;
    public static float jumpCloudSpeed=6f;

    private void Awake()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("Volume", 1f);
        Application.targetFrameRate = (int)PlayerPrefs.GetFloat("FPS", 60f);
        playButton.SetActive(true);
        gameOverImg.SetActive(false);
        homeButton.SetActive(false);
        Pause();
        firstSpawn=true;
    }

    public void Play()
    {
        StartCoroutine(PlaySequence());
    }
    public void Pause(){
        bird.paused=true;
        soundManager.StopMusic();
        Time.timeScale=0f;
        homeButton.SetActive(false);
    }

    public void Resume() {
        soundManager.ResumeMusic();
        Time.timeScale=1f;
        bird.enabled=true;
    }
    public void IncreaseScore(float val)
    {
        Score+=val;
        score.text=Score.ToString();
    }

    public void GameOver()
    {
        Pause();
        soundManager.GameOver();
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        bird.Fade();
        //soundManager.FadeEffect();
        yield return new WaitForSecondsRealtime(0.4f);
        bird.gameObject.SetActive(false);
        ExplodePlayer();
        soundManager.PlayExplosion();

        yield return new WaitForSecondsRealtime(2f);
        gameOverImg.SetActive(true);
        playButton.SetActive(true);
        homeButton.SetActive(true);
    }
    IEnumerator PlaySequence()
    {
        obj_mover[] env= FindObjectsByType<obj_mover>();
        for(int i = 0; i < env.Length; i++)
        {
            Destroy(env[i].gameObject);
        }
        CloudFade[] fades=  FindObjectsByType<CloudFade>();
        for(int i = 0; i < fades.Length; i++){
            Destroy(fades[i].gameObject);
        }
        bird.enabled=true;
        bird.ResetBird();
        bird.paused=true;
        gameOverImg.SetActive(false);
        playButton.SetActive(false);
        homeButton.SetActive(false);
        bird.gameObject.SetActive(true);
        if (!firstSpawn) {
            bird.FadeSpawn();
        }
        Score=0;
        score.text=Score.ToString();
        if (!firstSpawn) {
            yield return new WaitForSecondsRealtime(0.9f);
        }
        firstSpawn=false;
        bird.paused=false;
        Time.timeScale=1f;
        soundManager.PlayMusic();
    }

    private void ExplodePlayer(){
        Vector3 pos = bird.transform.position;
        explosion.transform.position=pos;
        explosion.transform.localScale=bird.transform.localScale*15;
        explosion.Explode();
    }

    void Update() {
        healthbar.fillAmount=bird.GetHealth();
    }
}
