using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text finalScore;
    [SerializeField] private Text highScore;
    [SerializeField] private PlayFabManager playFabManager;
    public Text accuracyText;
    public Text clickTimeText;
    public int currentRound = 1;
    public int sequenceSize = 3;
    public int scoreInRound = 0;
    public int totalScore = 0;
    public float startingTime = 5.0f;
    public float currentTime = 0f;
    public float playtime = 0.0f;
    public float activePlaytime = 0.0f;
    public float endingPlaytime = 0.0f;
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private Button[] buttons;
    public Color wantedColor;
    public Color[] setColors = {Color.green, Color.red, Color.blue, Color.yellow, Color.black};
    public List<int> sequence = new List<int>();
    bool roundStart = false;
    bool roundFailed = false;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject leaderboardScreen;
    public float lastClickTime;
    public float newClickTime;
    public float averageClickTime;

    void Awake()
    {
        mainCanvas = GameObject.Find("Canvas");
        gameOverScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentTime = startingTime;
        timerText.text = currentTime.ToString("0.0");
        startRound();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime <= 0)
        {
            currentTime = 0;
        }

        if ((currentTime == 0) && (roundFailed == false))
        {
            gameOver();
            roundFailed = true;
        }

        if (roundStart == true)
        {
            scoreText.text = totalScore.ToString();
            currentTime -= 1 * Time.deltaTime;
            timerText.text = currentTime.ToString("0.0");
        }
        if (scoreInRound == sequenceSize)
        {
            endRound();
            Debug.Log("Round completed");
        }
        if((roundFailed == false) && (roundStart == true))
        {
            activePlaytime = PlayerPrefs.GetFloat("ActivePlaytime", 0.0f);
            activePlaytime = activePlaytime + Time.deltaTime;
            PlayerPrefs.SetFloat("ActivePlaytime", activePlaytime);
        }
        if ((roundFailed == false))
        {
            playtime = PlayerPrefs.GetFloat("Playtime", 0.0f);
            playtime = playtime + Time.deltaTime;
            PlayerPrefs.SetFloat("Playtime", playtime);
        }
    }


    public void startRound()
    {
        scoreText.text = "Round " + currentRound.ToString();
        disableAllButtons();
        createSequence();
        StartCoroutine(showSequence());
        lastClickTime = Time.deltaTime;
    }

    public void createSequence()
    {
        int i;
        for (i = 0; i < sequenceSize; i++) {
            int nextNumber = Random.Range(0, 4);
            sequence.Add(nextNumber);
        }
    }

    public IEnumerator showSequence()
    {
        int i;
        changeButtonColor(4);
        yield return new WaitForSeconds(1);
        for (i = 0; i < sequenceSize; i++)
        {
            changeButtonColor(sequence[i]);
            yield return new WaitForSeconds(1);
            changeButtonColor(4);
            yield return new WaitForSeconds(0.25f);
        }
        roundStart = true;
        shuffleButtons();
    }

    public void endRound()
    {
        roundStart = false;
        scoreInRound = 0;
        sequence.Clear();
        currentRound++;
        if(currentRound % 3 == 0)
            sequenceSize++;
        startRound();
    }

    public void shuffleButtons()
    {
        disableAllButtons();
        changeButtonColor(4);

        List<int> activeButtons = new List<int>();
        int i = 0;
        while (i < 4)
        {
            int buttonSelected = Random.Range(0, 9);
            if( activeButtons.Contains(buttonSelected) == false)
            {
                activeButtons.Add(buttonSelected);
                wantedColor = setColors[i];
                ColorBlock cb = buttons[buttonSelected].colors;
                cb.normalColor = wantedColor;
                cb.highlightedColor = wantedColor;
                cb.pressedColor = wantedColor;
                buttons[buttonSelected].colors = cb;
                buttons[buttonSelected].enabled = true;
                i++;
            }
        }

    }

    private void gameOver()
    {
        roundStart = false;
        disableAllButtons();
        changeButtonColor(4);
        scoreText.text = " ";
        gameOverScreen.SetActive(true);
        finalScore.text = "Score " + totalScore.ToString();
        if(totalScore > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", totalScore);
        }
        int highScoreString = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log(highScoreString);
        string playtimeString = formatTime(PlayerPrefs.GetFloat("Playtime", 0));
        Debug.Log(playtimeString);
        PlayerPrefs.SetString("PlaytimeString", "0:00:00");
        playFabManager.SendLeaderboard(highScoreString);
        playFabManager.GetLeaderboard();
        playFabManager.sendPlayerData();
        highScore.text = "High Score " + highScoreString.ToString();
    }

    public void restart()
    {
        currentRound = 1;
        scoreInRound = 0;
        totalScore = 0;
        sequenceSize = 3;
        sequence.Clear();
        currentTime = startingTime;
        timerText.text = currentTime.ToString("0.0");
        gameOverScreen.SetActive(false);
        startRound();
    }

    public void quitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void showLeaderboard()
    {
        gameOverScreen.SetActive(false);
        leaderboardScreen.SetActive(true);
    }

    public void closeLeaderboard()
    {
        gameOverScreen.SetActive(true);
        leaderboardScreen.SetActive(false);
    }

    public void changeButtonColor(int colorNum)
    {
        int j;
        for (j = 0; j < 9; j++)
        {
            wantedColor = setColors[colorNum];
            ColorBlock cb = buttons[j].colors;
            cb.normalColor = wantedColor;
            cb.highlightedColor = wantedColor;
            cb.pressedColor = wantedColor;
            buttons[j].colors = cb;
        }
    }

    public void disableAllButtons()
    {
        int i;
        for(i = 0; i < 9; i++)
        {
            buttons[i].enabled = false;
        }
    }

    public string formatTime(float playtime)
    {
        float totalMinutes = playtime / 60;
        float hours = totalMinutes / 60;
        hours = Mathf.Round(hours * 1.0f) * 1f;
        float minutes = totalMinutes % 60;
        minutes = Mathf.Round(minutes * 1.0f) * 1f;
        float seconds = playtime % 60;
        seconds = Mathf.Round(seconds * 100.0f) * 0.01f;
        string playtimeString = hours.ToString() + ":" + minutes.ToString() + ":" + seconds.ToString();
        return playtimeString;
    }

    public IEnumerator waitFor(int numSeconds)
    {
        yield return new WaitForSeconds(numSeconds);
    }
}
