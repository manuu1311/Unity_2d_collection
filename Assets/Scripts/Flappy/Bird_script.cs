using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Mathematics;

public class Bird_script : MonoBehaviour
{
    public GameObject cloudPrefab;
    public Sprite[] wingSprites;
    private SpriteRenderer leftWing;
    private SpriteRenderer rightWing;
    public Rigidbody2D rigidBody;
    public float Flap_str=5f;
    public InputActionReference jump;
    public InputActionReference pause;
    private WaitForSeconds flapDelay = new WaitForSeconds(0.4f);
    public SoundManager soundManager;
    public GameManager gameManager;
    public bool paused;
    private float health=0.5f;
    public float helathdecay=0.05f;
    //public Animator fadeController;
    public float scale;
    //lerp time to reach final size, health size scale
    public float healthWarpTime;
    public bool active=true;
    public float fadeTimeDeath;
    public float fadeTimeSpawn;
    private Material material;
    private SpriteRenderer rend;
    private Coroutine fadeCoroutine;
    public AnimationCurve scaleCurve;
    public Animator animator;
    void Awake()
    {
        rend=gameObject.GetComponent<SpriteRenderer>();
        material = Instantiate(rend.material);
        rend.material = material;
        rightWing = transform.Find("birdwingright").GetComponent<SpriteRenderer>();
        leftWing = transform.Find("birdwingleft").GetComponent<SpriteRenderer>();
        gameObject.GetComponent<Rigidbody2D>().simulated=true;
        ResetBird();
        animator.enabled=false;
    }

    private void OnEnable()
    {
        // Subscribe to the action
        jump.action.performed += OnJump;
        jump.action.Enable();
        pause.action.performed += OnPause;
        pause.action.Enable();
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        jump.action.performed -= OnJump;
        jump.action.Disable();
    }

    public void OnJump(InputAction.CallbackContext context)
    {   
        if (context.performed)
        {
            if(!paused){
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, Flap_str);
            Flap();
            SpawnCloud();
            }
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {   
        if (context.performed)
        {
            if (paused) {
                gameManager.Resume();
                paused=false;
            }
            else {
                gameManager.Pause();
                paused=true;   
            }
        }
    }

    void SpawnCloud() {
        Vector3 spawnPos = transform.position + new Vector3(0, 0, 1f);
    Instantiate(cloudPrefab, spawnPos, Quaternion.Euler(0, 0, 180));
    }
    void Flap() {
        StartCoroutine(FlapRoutine());
        soundManager.Jump();
    }

    IEnumerator FlapRoutine() {
        //jumping animation
        leftWing.sprite = wingSprites[1];
        rightWing.sprite = wingSprites[1];
        yield return flapDelay;
        leftWing.sprite = wingSprites[0];
        rightWing.sprite = wingSprites[0];
    }

    //handle collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collision with tube or ground
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameOver();
        }
        //collision with coin or fish
        else if (collision.gameObject.CompareTag("Scoring"))
        {
            float rew=collision.gameObject.GetComponent<Score_rew>().rew;
            FindAnyObjectByType<GameManager>().IncreaseScore(rew);
            if (rew < 2f) {
                Heal(0.05f);
                soundManager.StarPickup2();   
            }
            else if(rew<4f){
                soundManager.StarPickup1(); 
                Heal(0.15f);
            }
            else {
                soundManager.FishPickUp();
                Heal(1f);
            }
            Destroy(collision.gameObject);
        }
    }

    private void GameOver() {
        health=0f;
        FindAnyObjectByType<GameManager>().GameOver();
    }

    void Update() {
        if(active){
        //health decay overtime
        health-=helathdecay*Time.deltaTime;
        if (health < 0f) {
            GameOver();
        }
        float tempScale=CalculateScale()*scale;
        Vector3 currentScale = transform.localScale;
        Vector3 newScale = new Vector3(tempScale, tempScale, 1);

        // Smooth transition
        transform.localScale = Vector3.Lerp(currentScale, newScale, Time.deltaTime * healthWarpTime);
        //gameObject.transform.localScale=new Vector3(tempScale,tempScale,1);
        }
        else {
            //rigidBody.simulated = false;;
        }
    }

    public void ResetBird() {
        //reset bird variables
        health=0.5f;
        float tempScale=CalculateScale()*scale;
        transform.localScale=new Vector3(tempScale,tempScale,1);
        Vector3 pos=transform.position;
        pos.y=-1.5f;
        transform.position=pos;
        rigidBody.linearVelocity = new Vector2(0, 0);
    }

    public float GetHealth() {
        return health;
    }

    private void Heal(float amount) {
        health+=amount;
        health=math.min(health,1f);
    }

    public void Fade() {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fader(-1,fadeTimeDeath));
    }
    public void FadeSpawn() {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fader(1,fadeTimeSpawn));
    }
    IEnumerator Fader(int sign, float duration) {
        // sign: 1 = fade 0 -> 1
        //       -1 = fade 1 -> 0

        float start = (sign >= 0) ? 0f : 1f;
        float end   = (sign >= 0) ? 1f : 0f;

        float t = 0f;
    
        // set initial value
        material.SetFloat("_Fade", start);

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;

            float normalized = Mathf.Clamp01(t / duration);
            float value = Mathf.Lerp(start, end, normalized);

            rend.material.SetFloat("_Fade", value);
            rightWing.material.SetFloat("_Fade", value);
            leftWing.material.SetFloat("_Fade", value);

            yield return new WaitForEndOfFrame();
        }

        // ensure final value is exact
        rend.material.SetFloat("_Fade", end);
        rightWing.material.SetFloat("_Fade", end);
        leftWing.material.SetFloat("_Fade", end);
    }

    private float CalculateScale() {
        float scaleValue = scaleCurve.Evaluate(health);
        return scaleValue;
    }
}


