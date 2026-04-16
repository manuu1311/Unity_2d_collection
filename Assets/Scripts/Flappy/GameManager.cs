using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO.Pipes;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Utilities;

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
    public static float objSpeed=5f;
    public static float jumpCloudSpeed=6f;
    public static float backgroundSpeed=0.02f;
    public ParticleSystem fireworks;
    public float scoreToWin;
    public Pipe_spawner pipeSpawner;
    public Pipe_spawner shellSpawner;
    public Pipe_spawner fishSpawner;
    public float warpFadeDuration;
    public float warpDuration;
    public float fadeTimeDeath=0.4f;
    public float fadeTimeSpawn=1f;
    public float warpFadeTimeSpawn=0.2f;
    public float warpFadeTimeDeath=0.2f;

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
        if (Score > scoreToWin) {
            EndGame();
        }
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
        bird.Fade(fadeTimeDeath);
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
        DestroyEnv();
        bird.enabled=true;
        bird.ResetBird();
        bird.paused=true;
        gameOverImg.SetActive(false);
        playButton.SetActive(false);
        homeButton.SetActive(false);
        bird.gameObject.SetActive(true);
        if (!firstSpawn) {
            bird.FadeSpawn(fadeTimeSpawn);
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

    public void Warp() {
        StartCoroutine(WarpSequence(warpFadeDuration,warpDuration));
    }

    private void DestroyEnv() {
        obj_mover[] env= FindObjectsByType<obj_mover>();
        for(int i = 0; i < env.Length; i++)
        {
            Destroy(env[i].gameObject);
        }
        CloudFade[] fades=  FindObjectsByType<CloudFade>();
        for(int i = 0; i < fades.Length; i++){
            Destroy(fades[i].gameObject);
        }
        Score_rew[] rews=  FindObjectsByType<Score_rew>();
        for(int i = 0; i < rews.Length; i++)
        {
            Destroy(rews[i].gameObject);
        }
    }
    public void EndGame() {
        StartCoroutine(GameEndSequence());
    }
    //end game sequence
    IEnumerator GameEndSequence() {
        //disable all spawners
        pipeSpawner.enabled=false;
        fishSpawner.enabled=false;
        shellSpawner.enabled=false;
        //remove health decay
        bird.helathdecay=0f;

        yield return new WaitForSeconds(15);
        //fade bird and spawn it in default coordinates
        bird.Fade(fadeTimeDeath);
        bird.paused=true;
        bird.rigidBody.simulated=false;
        yield return new WaitForSeconds(2);
        bird.ResetBird();
        bird.transform.localScale=new Vector3(0.04f,0.04f,1f);
        bird.FadeSpawn(fadeTimeSpawn);
        yield return new WaitForSeconds(1);
        StartCoroutine(AnimationSpeedDecreaser(2f));
        fishSpawner.enabled=true;
        fishSpawner.tmin=0.5f;
        fishSpawner.tmax=1f;
        fireworks.Play();
        bird.animator.enabled=true;
        bird.animator.SetTrigger("EndGame");
    }

    IEnumerator AnimationSpeedDecreaser(float duration) {
        float start=0.02f;
        float end=0f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;

            float normalized = Mathf.Clamp01(t / duration);
            float value = Mathf.Lerp(start, end, normalized);
            backgroundSpeed=value;
            yield return new WaitForEndOfFrame();
        }

        backgroundSpeed=0f;
    }
    IEnumerator WarpSequence(float fadeDuration, float warpDuration) {
        float mult=1.5f;
        float originalbg=backgroundSpeed;
        float originalobj=objSpeed;
        float startbg=0.02f;
        float endbg=startbg*mult;
        float startobj=5f;
        float endobj=startobj*mult;
        float t = 0f;
        //disable bird actions and gravity
        bird.paused=true;
        bird.rigidBody.linearVelocity=Vector2.zero;
        bird.rigidBody.simulated=false;
        bird.Fade(warpFadeTimeDeath);
        yield return new WaitForSeconds(warpFadeTimeDeath/2);
        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float normalized = Mathf.Clamp01(t / fadeDuration);
            float valuebg = Mathf.Lerp(startbg, endbg, normalized);
            backgroundSpeed=valuebg;
            float valueobj = Mathf.Lerp(startobj, endobj, normalized);
            objSpeed=valueobj;
            yield return new WaitForEndOfFrame();
        }
        soundManager.Warp();
        yield return new WaitForSeconds(warpDuration);
        t=0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float normalized = Mathf.Clamp01(t / fadeDuration);
            float valuebg = Mathf.Lerp(endbg,startbg, normalized);
            backgroundSpeed=valuebg;
            float valueobj = Mathf.Lerp(endobj,startobj, normalized);
            objSpeed=valueobj;
            yield return new WaitForEndOfFrame();
        }

        backgroundSpeed=originalbg;
        objSpeed=originalobj;
        //enable bird actions and gravity
        bird.paused=false;
        bird.FadeSpawn(fadeTimeSpawn);
        //give more time to jump
        yield return new WaitForSeconds(warpFadeTimeSpawn/2);
        bird.rigidBody.simulated=true;
    }
}
