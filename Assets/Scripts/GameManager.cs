using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private Player player;
    private UI UI;

    #region Coin Info
    public int coins;
    #endregion

    #region Score Info
    private bool canIncreaseScore;
    public float score;
    public float finalScore;
    #endregion

    [SerializeField] private string mainScene;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        UI = GameObject.Find("Canvas").GetComponent<UI>();
    }

    // Update is called once per frame
    void Update()
    {
        checkForScore();
    }

    public void gameStart()
    {
        player.canRun = true;
    }

    public void gameRestart()
    {
        SceneManager.LoadScene(mainScene);
    }

    public void gameEnds()
    {
        finalScore = (int)(score + (coins * 10));

        UI.endGameCalculations();

        // Stops Time.
        Time.timeScale = 0;
    }

    public void checkForScore()
    {
        if (canIncreaseScore)
        {
            score = player.rb.transform.position.x;
        }

        if (player.rb.velocity.x > 0)
        {
            canIncreaseScore = true;
        }
        else
        {
            canIncreaseScore = false;
        }
    }
}
