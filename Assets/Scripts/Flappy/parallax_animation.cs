using UnityEngine;

public class Sky_animation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float animation_speed;
    private MeshRenderer mesh;
    void Awake()
    {
        mesh=GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        mesh.material.mainTextureOffset+=new Vector2(GameManager.backgroundSpeed*Time.deltaTime,0);
    }
}
