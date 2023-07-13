using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject enemyPrefab;
    public Transform[] spawnPoints;
    private int enemyIndex;

    public int currentEnemy;


    // Start is called before the first frame update
    void Start()
    {
        
        currentEnemy = spawnPoints.Length;
        for(enemyIndex = 0; enemyIndex <= spawnPoints.Length-1; enemyIndex++)
            Instantiate(enemyPrefab, spawnPoints[enemyIndex].transform.position, Quaternion.identity);
       
    }

    private IEnumerator spawnCo(){
        yield return new WaitForSeconds(10f);
        for(enemyIndex = 0; enemyIndex <= spawnPoints.Length-1; enemyIndex++){
            Instantiate(enemyPrefab, spawnPoints[enemyIndex].transform.position, Quaternion.identity);
            yield return new WaitForSeconds(5f);
        }
    }
   
   
}
