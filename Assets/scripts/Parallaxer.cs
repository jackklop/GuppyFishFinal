using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    class PoolObject //Keeps track of all of the objects ofa type (all the pipes, sharks etc). Determines if object is in use because objects will not be instantiated or destroyed
    {
        public Transform transform;
        public bool inUse;
        public PoolObject(Transform t) { transform = t; }
        public void Use() { inUse = true;}
        public void Dispose() { inUse = false; }

    }
    [System.Serializable] //Serializable makes it viewable in inspector
    public struct YSpawnRange //Where pipes spawn in Y
    {
        public float min;
        public float max;

    }

    public GameObject PreFab; //Type of prefab
    public int poolSize; //How many should be spawning?
    public float shiftSpeed; //How fast they move
    public float spawnRate; //Rate of spawn

    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate; //Sets up object at start
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio; //Makes sure that objects aren't being spawned in screen space for all aspects

    float spawnTimer;
    float targetAspect; //Determined by dividing width by height
    PoolObject[] poolObjects;
    GameManager game; //Reference too GameManager

     void Awake()
    {
        Configure();
    }

     void Start()
    {
        game = GameManager.Instance; //Done in start because GameManager is defined within Awake, and start is called after awake which ensures that game always has a non-null reference
    }

     void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

     void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed()
    {
        for (int i = 0; i < poolObjects.Length; i++) 
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }
        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

     void Update()
    {
        if (game.GameOver) return;
        Shift();
        spawnTimer += Time.deltaTime; //Counts time
        if (spawnTimer > spawnRate) //If time to spawn, spawn and reset timer
        {
            Spawn(); 
            spawnTimer = 0;
        }

    }

    void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y; //Sets target aspect. If target aspect changes during gameplay, it resets every time we configure
        poolObjects = new PoolObject[poolSize]; //Create poolObject array
        for (int i = 0; i < poolObjects.Length; i++)
        { 
            GameObject go = Instantiate(PreFab) as GameObject; //Instantiate (only once) and set it as type GameObject
            Transform t = go.transform; //set transform
            t.SetParent(transform); //t should set parent to current object because script is on all parent object
            t.position = Vector3.one * 1000; // Initialize it off screen
            poolObjects[i] = new PoolObject(t);
        }
        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }
    void Spawn()//Spawn is basically putting objects in the right location
    {
        Transform t = GetPoolObject();
        if (t == null) return; //If null, pool size is too small
        Vector3 pos = Vector3.zero;
        pos.x = defaultSpawnPos.x * (Camera.main.aspect / targetAspect);
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
    }

    void SpawnImmediate() //Spawns two versions, at center of screen and off screen so it is smooth
    {
        Transform t = GetPoolObject();
        if (t == null) return; //If null, pool size is too small
        Vector3 pos = Vector3.zero;
        pos.x = immediateSpawnPos.x * (Camera.main.aspect / targetAspect); //Spawns at immediate pos
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
        Spawn();
    }

    void Shift()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime; //-Vector because going left
            CheckDisposeObject(poolObjects[i]); //Check if off screen
        }
    }

    void CheckDisposeObject(PoolObject poolObject)
    {
        if (poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect / targetAspect)) // if position is less than negative spawn position, it should be off screen
        {
            poolObject.Dispose(); //set inUse to false
            poolObject.transform.position = Vector3.one * 1000; //hides from player
        }
    }

    Transform GetPoolObject()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {//get first available object
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }
}
