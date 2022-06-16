using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas instance;

    [SerializeField] private GameObject[] screens;
    [SerializeField] private List<EnemyController> allEnemyControllers;

    [SerializeField] private Image progressBar;

    [SerializeField] private Image losePanel;
    [SerializeField] private Image retryButtonImage;

    [SerializeField] public TextMeshProUGUI numberLevel;
    [SerializeField] public TextMeshProUGUI numberNextLevel;
    [SerializeField] public TextMeshProUGUI startLevelTxt;

    private float fill = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        progressBar.fillAmount = fill;

        numberLevel.text = PlayerPrefs.GetInt(Loader.TRUE_NUMBER_LEVEL).ToString();
        numberNextLevel.text = (PlayerPrefs.GetInt(Loader.TRUE_NUMBER_LEVEL) + 1).ToString();
        startLevelTxt.text = "LEVEL " + PlayerPrefs.GetInt(Loader.TRUE_NUMBER_LEVEL).ToString();
    }

    public void ChangeScreen(Screens screen)
    {
        foreach (var item in screens)
        {
            item.SetActive(false);
        }

        int numberScreen = (int)screen;

        screens[numberScreen].SetActive(true);
    }

    public void UpdateProgressBar()
    {
        fill += 1 / (float)allEnemyControllers.Count;
        progressBar.DOFillAmount(fill, 0.3f);
    }

    public void LightUpWindow(float timeInSeconds)
    {
        retryButtonImage.DOFade(1, timeInSeconds);
        losePanel.DOFade(1, timeInSeconds);
    }
}

public enum Screens
{
    StartScreen,
    GameScreen,
    LoseScreen,
    VictoryScreen
}