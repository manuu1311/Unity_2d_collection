using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEditor;
using Unity.Collections;
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
    private bool paused;
    private float health=1f;
    public float helathdecay=0.05f;

    void Awake()
    {
        rightWing = transform.Find("birdwingright").GetComponent<SpriteRenderer>();
        leftWing = transform.Find("birdwingleft").GetComponent<SpriteRenderer>();
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
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, Flap_str);
            Flap();
            SpawnCloud();
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
        //health decay overtime
        health-=helathdecay*Time.deltaTime;
        if (health < 0f) {
            GameOver();
        }
    }

    public void ResetBird() {
        //reset bird variables
        health=1f;
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
}


