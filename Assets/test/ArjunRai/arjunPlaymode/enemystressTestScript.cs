using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class HazardStressTest
{
    private int enemyCount = 1;
    private GameObject enemyObject;
    private float xPos = 0;

    [OneTimeSetUp]

    public void setup() {
        SceneManager.LoadScene("MyTestSceneCopy");
    
    }

    [UnityTest]

    
    public IEnumerator maxEnemycount()
    {

        enemyObject = GameObject.Find("HazardPrefab");

        // Wait for a few frames to stabilize the framerate
        for (int i = 0; i < 5; i++)
        {
            yield return null;
        }

        // while at least 60 fps
        while (1 / Time.deltaTime > 60)
        {
            yield return null;
            xPos -= 0.05f;
            enemyObject = Object.Instantiate(enemyObject, new Vector3(xPos, -0.1f, 0), Quaternion.identity);
            enemyCount++;
            Debug.Log("Current enemy count:" + enemyCount);

        }
        Debug.Log("Max Enemy Count:" + enemyCount);

    }
}
