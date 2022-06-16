using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public const string NUMBER_LEVEL = "NumberLevel";
    public const string TRUE_NUMBER_LEVEL = "TrueNumberLevel";
    void Start()
    {
        if (PlayerPrefs.HasKey(NUMBER_LEVEL))
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt(NUMBER_LEVEL));
        }
        else
        {
            PlayerPrefs.SetInt(NUMBER_LEVEL, 1);
            PlayerPrefs.SetInt(TRUE_NUMBER_LEVEL, 1);
            SceneManager.LoadScene(PlayerPrefs.GetInt(NUMBER_LEVEL));
        }
    }
}
