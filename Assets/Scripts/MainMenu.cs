using UnityEngine;
using UnityEngine.UI;

public static class foo
{
    // only an hour left aaaah
    public static bool sfxMuted = false;
    public static float camSpeed = 1;
}

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public Toggle toggle;
    public Slider slider;

    private void Start()
    {
        AdjustSensitivity();
    }
    public void Play ()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void OpenSettings ()
    {
        mainMenu.SetActive(false);
        slider.transform.parent = gameObject.transform;
        settingsMenu.SetActive(true);
    }

    public void CloseSettings ()
    {
        slider.transform.parent = settingsMenu.transform;
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void MuteAudio ()
    {
        if (toggle.isOn) foo.sfxMuted = false;
        else foo.sfxMuted = true;
        Debug.Log(foo.sfxMuted);
    }

    public void AdjustSensitivity ()
    {
       foo.camSpeed = slider.value;
        Debug.Log(foo.camSpeed);
    }

    public void Quit ()
    {
        Application.Quit();
    }
}
