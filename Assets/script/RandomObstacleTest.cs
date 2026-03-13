using UnityEngine;
using System.Collections;

public class RandomObstacleTest : MonoBehaviour
{
    public GameObject obstaclePrefab;

    public int obstacleCount = 30;
    public int testRuns = 3;

    public float mapWidth = 20f;
    public float mapHeight = 20f;

    public float delayBetweenRuns = 5f;

    IEnumerator Start()
    {
        for (int run = 1; run <= testRuns; run++)
        {
            Debug.Log("Run : " + run + "  (Spawn Obstacles)");

            SpawnObstacles();

            // รอ 5 วินาที
            yield return new WaitForSeconds(delayBetweenRuns);

            Debug.Log("Run : " + run + "  (Clear Obstacles)");
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objs)
            {
                Destroy(obj);
            }
        }

        Debug.Log("Test Finished");
    }

    void SpawnObstacles()
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            float x = Random.Range(-mapWidth, mapWidth);
            float y = Random.Range(-mapHeight, mapHeight);

            Vector2 pos = new Vector2(x, y);

            GameObject obj = Instantiate(obstaclePrefab, pos, Quaternion.identity);
            obj.tag = "Obstacle";
        }

        Debug.Log("Spawned : " + obstacleCount + " obstacles");
    }

    void ClearObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (GameObject obj in obstacles)
        {
            Destroy(obj);
        }

        Debug.Log("Cleared : " + obstacles.Length + " obstacles");
    }
}