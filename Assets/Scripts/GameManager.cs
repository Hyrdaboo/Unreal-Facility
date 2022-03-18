using System.Diagnostics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Rigidbody player;
    public Transform voidCheck;
    PlayerStats stats;

    private void Start()
    {
        stats = player.GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (player.transform.position.y < voidCheck.transform.position.y)
        {
            stats.TakeDamage(stats.HP);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Confined;
            TimeCounter.Stop();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}

public static class TimeCounter
{
    public static Stopwatch timer = new Stopwatch();
    public static long result;
    public static float milliseconds;
    public static float seconds;
    public static long minutes;

    public static void Start()
    {
        timer.Start();
    }

    public static string GetResult ()
    {
        result = timer.ElapsedMilliseconds;
        milliseconds = (float)result % 1000;
        seconds = (result / 1000) % 60;
        minutes = (result / 1000) / 60;

        return minutes.ToString() + ":" + seconds.ToString() + ":" + milliseconds.ToString();
    }

    public static void Stop ()
    {
        timer.Stop();
        timer.Reset();
    }
}
