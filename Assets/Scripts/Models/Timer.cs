using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    public Text energyText;

    public GameObject itemBar;
    public Text timerText2;
    public Text energyText2;
    
    private float timeRemaining = 59f;

    void Update()
    {
        int energy = PlayerPrefs.GetInt("Energy");
        if (energy >= 200)
        {
            timerText.text = "00:00";
            timerText2.text = "00:00";
            return;
        }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining < 0f)
        {
            timeRemaining = 59f;
            
            PlayerPrefs.SetInt("Energy", energy + 1);
            energyText.text = $"{PlayerPrefs.GetInt("Energy")}/200";

            if (itemBar.activeSelf)
            {
                energyText2.text = $"{PlayerPrefs.GetInt("Energy")}/200";
            }

        }

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if(itemBar.activeSelf)
        {
            timerText2.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
