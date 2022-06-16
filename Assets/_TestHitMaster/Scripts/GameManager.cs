using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Platform firstPlatform;
    [SerializeField] private Platform finishPlatform;

    public GameStates gameStates;
    
    void Start()
    {
        MainCanvas.instance.ChangeScreen(Screens.StartScreen);

        gameStates = GameStates.StartGame;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && gameStates == GameStates.StartGame)
        {
            firstPlatform.isActive = true;
            gameStates = GameStates.PlayGame;
            MainCanvas.instance.ChangeScreen(Screens.GameScreen);
        }
        else if (finishPlatform.isActive)
        {
            gameStates = GameStates.EndGame;
            gameObject.SetActive(false);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt(Loader.NUMBER_LEVEL));
    }

    public void Next()
    {
        PlayerPrefs.SetInt(Loader.TRUE_NUMBER_LEVEL, PlayerPrefs.GetInt(Loader.TRUE_NUMBER_LEVEL) + 1);

        if (PlayerPrefs.GetInt(Loader.NUMBER_LEVEL) == 1)
        {
            PlayerPrefs.SetInt(Loader.NUMBER_LEVEL, 2);
        }
        else
        {
            PlayerPrefs.SetInt(Loader.NUMBER_LEVEL, 1);
        }

        Restart();
    }
}

public enum GameStates
{
    StartGame,
    PlayGame,
    EndGame
}