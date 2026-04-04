using UnityEngine;
using System.Collections;
using Unity.Mathematics;


public class Pipe_spawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public float tmin=0.5f;
    public float tmax=3f;
    public float prob=0.5f;
    public float maxHeight=3f;
    public float minHeight=1f;
    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // random delay between spawns
            float tPrime = UnityEngine.Random.Range(tmin, tmax);
            yield return new WaitForSeconds(tPrime);
            Spawn();
        }
    }
    private void Spawn()
    {
        bool check= UnityEngine.Random.Range(0f, 1f) > prob;
        int id= UnityEngine.Random.Range(0,prefabs.Length);
        GameObject prefab= prefabs[id];
        GameObject elem= Instantiate(prefab,transform.position, quaternion.identity); 
        if (check)
        {
            elem.transform.position += Vector3.up * UnityEngine.Random.Range(minHeight, maxHeight);
            elem.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
        }
        else
        {
            elem.transform.position += Vector3.up * UnityEngine.Random.Range(-maxHeight, -minHeight);
        }
        
    }
}
