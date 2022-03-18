using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using TMPro;
public class SceenUIManager : MonoBehaviour
{
    public PlayerStats playerStats;
    public Image deathImage;
    public TMP_Text fpstext;
    public float refreshRate = 1;

    private float timer;
    private void Awake()
    {
        playerStats.DeathEvent += OnPlayerDeath;
    }
    private void OnDestroy()
    {
        playerStats.DeathEvent -= OnPlayerDeath;
    }

    public void OnPlayerDeath ()
    {
        Debug.Log("died");
        deathImage.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Time.unscaledTime > timer)
        {
            showFps();
        }
    }

    void showFps ()
    {
        timer = Time.unscaledTime + refreshRate;
        fpstext.text = ((int)(1f / Time.unscaledDeltaTime)).ToString() + "FPS";
    }
}
