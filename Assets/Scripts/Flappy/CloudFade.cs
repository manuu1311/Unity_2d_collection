using UnityEngine;
using System.Collections;

public class CloudFade : MonoBehaviour
{
    public float fadeDuration = 1.5f; // how long the fade lasts
    public bool active=true;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeAndDestroy());
    }

    IEnumerator FadeAndDestroy()
    {
        Color originalColor = sr.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject); // remove the cloud after fading
    }
    void Update() {
        if(active){
            Vector3 pos=transform.position;
            pos.x-=GameManager.jumpCloudSpeed*Time.deltaTime;
            pos.y+=0.5f*Time.deltaTime;
            transform.position=pos;
    }
    }
}