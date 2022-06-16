using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float forcePowerUp;
    [SerializeField] float forcePowerForward;

    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField] private Transform currentYPOs;
    [SerializeField] Rigidbody enemyHips;
    [SerializeField] Transform playerTransform;

    [SerializeField] Image aim;

    private Rigidbody[] enemyesRigidbody;
    private Animator enemyAnimator;

    private const string RIGHT_LEG_DOWN = "Right Leg";
    private const string RIGHT_LEG_UP = "Right Thigh";
    private const string LEFT_LEG_DOWN = "Left Leg";
    private const string LEFT_LEG_UP = "Left Thigh";
    private const string HEAD = "Head";

    private const float TIME_IN_STATE = 1.5f;
    private const float TIME_MOVE_TO_PLAYER = 20f;

    private float timer;
    private Transform currentPosAfterStandUp;
    private int health = 2;
    private bool isActiveEnemy = true;

    public bool isFirstEnemy;
    public EnemyStates enemyStates;

    private void Start()
    {
        aim.gameObject.SetActive(false);

        enemyesRigidbody = GetComponentsInChildren<Rigidbody>();
        AddTagToRigidbodys();

        enemyAnimator = GetComponent<Animator>();
        currentPosAfterStandUp = transform;

        enemyStates = new EnemyStates();

        enemyStates = EnemyStates.GotUp;

        if(isFirstEnemy == true)
        {
            OnAim(true);
        }
    }

    private void Update()
    {
        if (isActiveEnemy)
        {
            AimSetup();
            Checker();
            StateChanger();
        }
    }
    public void OnAim(bool value)
    {
        aim.gameObject.SetActive(value);
    }

    private void AimSetup()
    {
        aim.gameObject.transform.LookAt(Camera.main.transform);

        if (PlayerController.tapTimer <= 3f && !isFirstEnemy)
        {
            aim.gameObject.SetActive(false);
        }
    }
    public void ShootOn(string nameCollider)
    {
        if (enemyStates == EnemyStates.StandsUp || enemyStates == EnemyStates.Move)
        {
            enemyHips.transform.DOKill();
            transform.DOKill();
        }

        if (enemyStates != EnemyStates.Die)
        {
            enemyStates = EnemyStates.Falls;
            transform.DOKill();
            enemyHips.isKinematic = false;
            timer = 0;
            OnAim(false);

            enemyAnimator.enabled = false;

            foreach (var item in enemyesRigidbody)
            {
                if (item.name == nameCollider)
                {
                    if (item.name == RIGHT_LEG_UP || item.name == RIGHT_LEG_DOWN || item.name == LEFT_LEG_UP || item.name == LEFT_LEG_DOWN)
                    {
                        item.AddForce(Vector3.forward * forcePowerForward, ForceMode.Impulse);
                    }
                    else
                    {
                        item.AddForce((Vector3.up * forcePowerUp) + (Vector3.forward * forcePowerForward), ForceMode.Impulse);
                        if (item.name == HEAD)
                        {
                            ChangeMaterialColor();
                            health -= 2;
                        }
                    }
                    return;
                }
            }
        }   
    }

    private void ChangeMaterialColor()
    {
        skinnedMeshRenderer.materials[0].DOColor(Color.gray, 0.5f);
        skinnedMeshRenderer.materials[1].DOColor(Color.gray, 0.5f);
        aim.gameObject.SetActive(false);
    }

    private void AddTagToRigidbodys()
    {
        foreach (var item in enemyesRigidbody)
        {
            if (item.tag != "Enemy")
            {
                item.tag = "Enemy";
                item.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
        }
    }

    private void Checker()
    {
        var distance = Mathf.Abs(currentYPOs.position.y - enemyHips.transform.position.y);

        if (distance > 3f)
        {
            enemyStates = EnemyStates.Die;
            ChangeMaterialColor();
            MainCanvas.instance.UpdateProgressBar();
            isActiveEnemy = false;
        }
    }

    private void StateChanger()
    {
        if (enemyStates == EnemyStates.Falls)
        {
            if (timer == 0)
            {
                health -= 1;

                if (health <= 0)
                {
                    ChangeMaterialColor();
                }
            }

            timer += Time.deltaTime;

            if (timer >= TIME_IN_STATE && health > 0)
            {
                timer = 0;
                enemyStates = EnemyStates.StandsUp;
            }
            else if (timer >= TIME_IN_STATE)
            {
                enemyStates = EnemyStates.Die;
                MainCanvas.instance.UpdateProgressBar();
                isActiveEnemy = false;
            }
        }
        else if (enemyStates == EnemyStates.StandsUp)
        {
            if (timer == 0)
            {
                enemyHips.isKinematic = true;
                enemyHips.transform.DOLookAt(playerTransform.position, TIME_IN_STATE);
                enemyHips.transform.DOMoveY(currentYPOs.position.y, TIME_IN_STATE);
            }

            timer += Time.deltaTime;

            if (timer >= TIME_IN_STATE)
            {
                currentPosAfterStandUp = enemyHips.transform;
                timer = 0;
                enemyStates = EnemyStates.StandDone;
            }

        }
        else if (enemyStates == EnemyStates.StandDone)
        {
            enemyAnimator.enabled = true;
            transform.position = currentPosAfterStandUp.position - Vector3.up;
            enemyStates = EnemyStates.Move;
        }
        else if (enemyStates == EnemyStates.Move)
        {
            transform.DOMove(playerTransform.position, TIME_MOVE_TO_PLAYER);
            
            enemyStates = EnemyStates.GotUp;
        }
    }
}

public enum EnemyStates
{
    Falls,
    StandsUp,
    StandDone,
    GotUp,
    Move,
    Die
}