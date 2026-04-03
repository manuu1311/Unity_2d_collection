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
    public ParticleSystem explosion;
    public Image healthbar;

    private void Awake()
    {
        Application.targetFrameRate=60;
        playButton.SetActive(true);
        gameOverImg.SetActive(false);
        homeButton.SetActive(false);
        Pause();
    }

    public void Play()
    {
        Score=0;
        score.text=Score.ToString();

        bird.gameObject.SetActive(true);
        gameOverImg.SetActive(false);
        playButton.SetActive(false);
        homeButton.SetActive(false);
        Time.timeScale=1f;
        bird.enabled=true;
        bird.ResetBird();
        obj_mover[] env= FindObjectsByType<obj_mover>();
        for(int i = 0; i < env.Length; i++)
        {
            Destroy(env[i].gameObject);
        }
        CloudFade[] fades=  FindObjectsByType<CloudFade>();
        for(int i = 0; i < fades.Length; i++)
        {
            Destroy(fades[i].gameObject);
        }
        soundManager.PlayMusic();
    }
    public void Pause(){
        soundManager.StopMusic();
        Time.timeScale=0f;
        bird.enabled=false;
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
        yield return new WaitForSecondsRealtime(0.8f);
        bird.gameObject.SetActive(false);
        ExplodePlayer();
        soundManager.PlayExplosion();

        yield return new WaitForSecondsRealtime(2f);
        gameOverImg.SetActive(true);
        playButton.SetActive(true);
        homeButton.SetActive(true);
    }

    private void ExplodePlayer(){
        Vector3 pos = bird.transform.position;
        explosion.transform.position=pos;
        explosion.Play();
    }

    void Update() {
        healthbar.fillAmount=bird.GetHealth();
    }
}
