using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    #region Menu Items
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject muteButtonUI;

    [SerializeField] private GameObject[] menuItems;
    #endregion

    [SerializeField] private Text score;
    [SerializeField] private Text coins;
    [SerializeField] private Text endScreenCoins;
    [SerializeField] private Text endScreenScore;
    [SerializeField] private Text endScreenFinalScore;

    public bool isMuted;
    public bool gameIsPaused;

    // Start is called before the first frame update
    void Start()
    {
        switchUI(mainMenuUI);
    }

    // Update is called once per frame
    void Update()
    {
        coinInfo();
        scoreInfo();
    }

    public void switchUI(GameObject uiToActivate)
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].SetActive(false);
        }

        uiToActivate.SetActive(true);
    }
    #region Button Functions
    public void tapToStartButton()
    {
        GameManager.instance.gameStart();
        switchUI(inGameUI);
    }

    public void playAgainButton()
    {
        GameManager.instance.gameRestart();
    }

    public void toggleMuteButton()
    {
        isMuted = !isMuted;

        Image muteIcon = GameObject.Find("muteIcon").GetComponent<Image>();

        if (isMuted)
        {
            muteIcon.color = new Color(muteIcon.color.r, muteIcon.color.g, muteIcon.color.b, 0.6f);
        }
        else
        {
            muteIcon.color = new Color(muteIcon.color.r, muteIcon.color.g, muteIcon.color.b, 1.0f);
        }
    }

    public void shopButton()
    {
        switchUI(shopUI);
        muteButtonUI.SetActive(false);
    }

    public void settingsButton()
    {
        switchUI(settingsUI);
        muteButtonUI.SetActive(false);
    }

    public void togglePauseButton()
    {
        gameIsPaused = !gameIsPaused;

        if (gameIsPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void closeButton()
    {
        switchUI(mainMenuUI);
        muteButtonUI.SetActive(true);
    }
    #endregion

    public void endGameCalculations()
    {
        switchUI(endGameUI);

        endScreenCoins.text = "Coins: " + GameManager.instance.coins.ToString("#,#");
        endScreenScore.text = "Distance: " + Mathf.RoundToInt(GameManager.instance.score).ToString("#,#") + " m";
        endScreenFinalScore.text = "Final Score: " + GameManager.instance.finalScore.ToString("#,#");
    }

    private void coinInfo()
    {
        coins.text = GameManager.instance.coins + " ";
    }

    private void scoreInfo()
    {
        score.text = (int)GameManager.instance.score + " m";
    }
}
