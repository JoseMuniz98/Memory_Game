using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public MainScript main;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonClick()
    {
        Color thisColor = GetComponent<Button>().colors.normalColor;
        int colorID = main.sequence[main.scoreInRound];
        float totalClicks = PlayerPrefs.GetFloat("TotalClicks", 0);
        totalClicks++;
        PlayerPrefs.SetFloat("TotalClicks", totalClicks);
        main.newClickTime = Time.deltaTime;
        float clickTime = main.newClickTime - main.lastClickTime;
        main.lastClickTime = Time.deltaTime;
        float averageClickTime = PlayerPrefs.GetFloat("ReactionTime", 0);
        float activePlaytime = PlayerPrefs.GetFloat("ActivePlaytime", 0);
        averageClickTime = (activePlaytime + clickTime) / totalClicks;
        PlayerPrefs.SetFloat("ReactionTime", averageClickTime);
        main.clickTimeText.text = PlayerPrefs.GetFloat("ReactionTime", 0).ToString();
        float totalHits = PlayerPrefs.GetFloat("TotalHits", 0);
        if (thisColor == main.setColors[colorID])
        {
            main.scoreInRound++;
            main.totalScore++;
            main.currentTime++;
            totalHits++;
        }
        PlayerPrefs.SetFloat("TotalHits", totalHits);
        float accuracy = totalHits / totalClicks;
        Debug.Log(accuracy);
        PlayerPrefs.SetFloat("Accuracy", accuracy);
        main.accuracyText.text = PlayerPrefs.GetFloat("Accuracy", 0).ToString();
        main.shuffleButtons();
    }

}
