using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;

public class Platform : MonoBehaviour
{
    [SerializeField] private List <EnemyController> enemyControllers;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform[] pointsForMove;
    [SerializeField] private Transform normalHeight;
    [SerializeField] private Platform nextPlatform;

    [SerializeField] public bool isActive;
    [SerializeField] private bool isFinishPlatform;

    [SerializeField] private float[] distanceToEnemyes;
    [SerializeField] private GameObject[] fireWorks;

    private bool isStartingLookPoint;
    private int counterDieEnemies = 0;
    private bool isEnemyMoving;
    private PlayerController playerController; 
    public static float tapTimer;

    private void Start()
    {
        distanceToEnemyes = new float[enemyControllers.Count];
        playerController = player.GetComponent<PlayerController>();
    }
    private void Update()
    {
        if (!isFinishPlatform)
        {
            LookAtPoint(normalHeight);
            CheckEnemyes();
            StartEnemyMove();
            CheckGameOver();

            if (PlayerController.tapTimer > 3 && isActive)
            {
                OnAimClosestEnemy();
            }
        }
        else
        {
            LookAtPoint(normalHeight);
            if (isActive)
            {
                OnFireWorks();
                isActive = false;
                playerController.enabled = false;
            }
        }
    }

    private void OnFireWorks()
    {
        foreach (var item in fireWorks)
        {
            item.SetActive(true);
        }

        MainCanvas.instance.ChangeScreen(Screens.VictoryScreen);
    }
    private void CheckGameOver()
    {
        for(int i = 0; i < enemyControllers.Count; i++)
        {
            var heading = enemyControllers[i].transform.position - player.transform.position;
            var distance = heading.magnitude;
            distanceToEnemyes[i] = distance;

            if (distance <= 0.1f)
            {
                Debug.Log("GameOver");
                MainCanvas.instance.ChangeScreen(Screens.LoseScreen);
                MainCanvas.instance.LightUpWindow(0.4f);
            }
        }
    }

    private void OnAimClosestEnemy()
    {
        var index = Array.IndexOf(distanceToEnemyes, distanceToEnemyes.Min());
        enemyControllers[index].OnAim(true);
    }

    private void CheckEnemyes()
    {
        if (isActive == true)
        {
            foreach (var item in enemyControllers)
            {
                if (item.enemyStates == EnemyStates.Die)
                {
                    counterDieEnemies++;

                    if (counterDieEnemies == enemyControllers.Count)
                    {
                        isActive = false;
                        StartCoroutine(PlayerMover());
                    }
                }
            }

            counterDieEnemies = 0;
        }
    }
    private void LookAtPoint(Transform point)
    {
        if (!isStartingLookPoint && isActive)
        {
            player.gameObject.transform.DOLookAt(point.position, 0.5f);
            isStartingLookPoint = true;
        }   
    }

    private void StartEnemyMove()
    {
        if (isActive && isEnemyMoving == false)
        {
            isEnemyMoving = true;
            PlayerController.tapTimer = 0;

            foreach (var item in enemyControllers)
            {
                item.enemyStates = EnemyStates.Move;
            }
        }
    }

    IEnumerator PlayerMover()
    {
        var points = 0;

        while(true)
        {
            player.transform.DOMove(pointsForMove[points].position, 1f);
            player.transform.DOLookAt(pointsForMove[points].transform.position, 1f);
            tapTimer = 0;
            points++;
            
            yield return new WaitForSeconds(1f);

            if (points == pointsForMove.Length)
            {
                nextPlatform.isActive = true;
                player.GetComponent<PlayerController>().RemoveAllDaggers(1f);
                player.GetComponent<PlayerController>().CreateStartDaggers();
                yield break;
            }
        }   
    }
}
