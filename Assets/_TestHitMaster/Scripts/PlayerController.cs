using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform leftHand;
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private GameObject daggerInHand;
    [SerializeField] private GameManager gameManager;

    private List <GameObject> daggers;
    private List <DaggerController> daggersControllers;

    private PlayerAnimator playerAnimator;
    private Animator animator;
 
    private int numberOfDagger = 0;
    private int startDaggersCount = 5;
    private Vector3 startPosForDagger;
    private Vector3 target;
    private RaycastHit hit;
    private bool isFly;

    public static float tapTimer;

    private void Start()
    {
        daggers = new List<GameObject>();
        daggersControllers = new List<DaggerController>();

        CreateStartDaggers();

        animator = GetComponent<Animator>();
        playerAnimator = new PlayerAnimator();
    }

    private void Update()
    {
        if (gameManager.gameStates == GameStates.PlayGame)
        {
            tapTimer += Time.deltaTime;
        }
        
        if (Input.GetMouseButtonDown(0) && isFly == false)
        {
            tapTimer = 0;
            isFly = true;
            playerAnimator.OnPunch(animator);

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));

            if (Physics.Raycast(ray, out hit))
            {
                Shoot();
            }
        }

        if (!animator.GetBool("IsAttack"))
        {
            daggerInHand.SetActive(true);
            isFly = false;
        }
    }

    private void AddMoreDaggers()
    {
        for (int i = 0; i < startDaggersCount; i++)
        {
            var GO = Instantiate(daggerPrefab, startPosForDagger, Quaternion.identity);

            daggers.Add(GO);

            daggersControllers.Add(GO.GetComponent<DaggerController>());
            daggersControllers[i + numberOfDagger].StartDaggerSetup(Vector3.zero);

            daggers[i + numberOfDagger].SetActive(false);
        }
    }
    private void Shoot()
    {
        daggers[numberOfDagger].SetActive(true);

        daggerInHand.SetActive(false);

        target = new Vector3(hit.point.x, hit.point.y, hit.point.z) - startPosForDagger;

        daggersControllers[numberOfDagger].FlyToTarget(target);

        numberOfDagger++;

        if (numberOfDagger == daggers.Count)
        {
            AddMoreDaggers();
        }
    }

    public void CreateStartDaggers()
    {
        for (int i = 0; i < startDaggersCount; i++)
        {
            startPosForDagger = leftHand.position;

            daggers.Add(Instantiate(daggerPrefab, startPosForDagger, Quaternion.identity));

            daggersControllers.Add(daggers[i].GetComponent<DaggerController>());
            daggersControllers[i].StartDaggerSetup(Vector3.zero);

            daggers[i].SetActive(false);
        }
    }

    public void RemoveAllDaggers(float timeToDestroyInSeconds)
    {
        foreach (var item in daggers)
        {
            Destroy(item, timeToDestroyInSeconds);
        }

        numberOfDagger = 0;
        daggers.Clear();
        daggersControllers.Clear();
    }
}
