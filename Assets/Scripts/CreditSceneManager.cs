using UnityEngine;
using TMPro;

public class CreditSceneManager : MonoBehaviour
{
    public TMP_Text time;
    public TMP_Text description;

    private void Awake()
    {
        time.text = "In " + TimeCounter.GetResult();

        if (TimeCounter.minutes < 1)
        {
            description.text = "You are a GIGACHAD gamer";
        }
        else if (TimeCounter.minutes < 3)
        {
            description.text = "Congrats you are an epic gamer";
        }
        else if (TimeCounter.minutes < 5)
        {
            description.text = "An average score. C'mon you can do it better";
        }
        else if (TimeCounter.minutes < 9)
        {
            description.text = "Seriously??! My Grandma can do faster than you";
        }
        else
        {
            description.text = "Are you even trying??!";
        }
        if (TimeCounter.minutes < 1 && TimeCounter.seconds < 30)
        {
            description.text = "How is this possible?\n Just make sure you are not this fast in bed";
        }

        TimeCounter.Stop();

        Debug.Log("Credit Scene");
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKey(KeyCode.R))
        {
            Cursor.lockState = CursorLockMode.Confined;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
