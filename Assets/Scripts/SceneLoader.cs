using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneLoader : MonoBehaviour
{
    public int[] ScenesInBuild;
    public Enemy[] enemiesInScene;
    public bool allEnemiesDead = false;
    public float deadEnemies = 0;

    public event Action ReloadScene;
    private void Start()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        ScenesInBuild = new int[sceneCount];
        
        for (int i = 0; i < sceneCount; i++)
        {
            ScenesInBuild[i] = i;
        }
        enemiesInScene = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemiesInScene)
        {
            enemy.EnemyDeathEvent += CountDeadEnemies;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ReloadCurrentScene();
        if (deadEnemies == enemiesInScene.Length) allEnemiesDead = true;
    }

    public void CountDeadEnemies ()
    {
        deadEnemies++;
    }

    private void OnCollisionEnter(Collision other)
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if (other.transform.root.tag == "Player")
        {
            LoadNextLevel(currentScene+1);
        }
    }

    public void ReloadCurrentScene ()
    {
        ReloadScene?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel (int sceneIndex)
    {
       if ((Array.IndexOf(ScenesInBuild, sceneIndex) == -1))
       {
            Debug.Log("You have completed the game wow");
       }
       else
       {
            if (allEnemiesDead)
            {
                SceneManager.LoadScene(sceneIndex);
                ReloadScene?.Invoke();
            }
        }
    }
}
