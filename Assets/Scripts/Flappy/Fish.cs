using Unity.Mathematics;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private float yfirst;
    private float ylast;
    private float height;
    private float xspeed;
    private float yvelocity;
    private float xfirst;
    private float rotation;
    public float rotationspeed;
    private float scale;
    public ParticleSystem first_splash;
    public ParticleSystem second_splash;
    private SoundManager soundManager;
    //if object is active: after ylast is reached, it will become inactive
    private bool active;
    void Awake() {
        yfirst=UnityEngine.Random.Range(-4f,-2f);
        ylast=yfirst;
        xfirst=UnityEngine.Random.Range(3.5f,9f);
        xspeed=UnityEngine.Random.Range(0.2f,2f);
        yvelocity=UnityEngine.Random.Range(2f,7f);
        rotation=-6f*yvelocity+3f*xspeed;
        scale=UnityEngine.Random.Range(0.1f,0.3f);
        active=true;
        soundManager = FindAnyObjectByType<SoundManager>();
    }
    void Start()
    {
        Vector3 pos= new Vector3(xfirst,yfirst,0);
        transform.position=pos;
        transform.localScale=new Vector3(scale,scale,1);
        first_splash.Play();
        soundManager.FirstSplash();
    }


    void Update()
    {
        if (active) {
            Vector3 pos3=transform.position;
            pos3.x-=(GameManager.objSpeed-xspeed)*Time.deltaTime;
            yvelocity-=9.8f*Time.deltaTime;
            pos3.y+=yvelocity*Time.deltaTime;
            transform.position=pos3;
            rotation+=rotationspeed*Time.deltaTime;
            transform.rotation=Quaternion.Euler(0,0,rotation);
            if(pos3.y < ylast){
                //final state reached
                active=false;
                transform.rotation=Quaternion.Euler(0,0,0);
                second_splash.Play();
                soundManager.SecondSplash();
                GetComponent<SpriteRenderer>().enabled = false;
                Destroy(gameObject,2f);
            }   
        }
        else {
            Vector3 pos3=transform.position;
            pos3.x-=(GameManager.objSpeed-xspeed)*Time.deltaTime;
            transform.position=pos3;
        }
    }
}
