using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, ISaveable
{
    public struct EnemySaveData{
        int type;
        int currentHealth;
        Vector3 position;
    }
    public int timeIntervalMin;
    public int timeIntervalMax;
    private int timeIntervalCurr;
    public float timeSince;
    public Vector3 spawnPoint;

    private int enemiesToSpawn;
    private int enemiesSpawned;

    public GameObject enemyBase;
    private List<EnemySaveData> enemies = new List<EnemySaveData>();

    public string SaveID {get;}
    public JsonData SavedData {get;}

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SpawnEnemies");
        GenerateTime();
    }

    // Update is called once per frame
    void Update()
    {
        timeSince+= Time.deltaTime;
        if(timeSince > timeIntervalCurr){
            enemiesToSpawn = Random.Range(2, 4);
            enemiesSpawned = 0;
            StartCoroutine("SpawnEnemies");
            ResetInterval();
        }
    }

    void ResetInterval(){
        timeSince = 0f;
        GenerateTime();
    }

    IEnumerator SpawnEnemies(){
        yield return new WaitForSeconds(.2f);
        GameObject enemy = Instantiate(enemyBase, spawnPoint, Quaternion.identity);
        enemy.GetComponent<Enemy>().SetSettings(RandomEnemyType());
        enemiesSpawned++;
        if(enemiesSpawned < enemiesToSpawn){
            StartCoroutine("SpawnEnemies");
        }
    }

    void GenerateTime(){
        timeIntervalCurr = Random.Range(timeIntervalMin, timeIntervalMax);
    }

    SO_EnemyTypes RandomEnemyType(){
        var enemyTypes = Resources.LoadAll("EnemyTypes", typeof(SO_EnemyTypes));
        var chosenType = enemyTypes[Random.Range(0, enemyTypes.Length)];
        return (SO_EnemyTypes) chosenType;
    }

    public void LoadFromData(JsonData data){
        return null;
    }
}
