using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DaggerController : MonoBehaviour
{
    [SerializeField] private Rigidbody daggerRigidbody;
    [SerializeField] private Transform daggerInRoot;
    [SerializeField] private float speed;
    [SerializeField] private CapsuleCollider daggerCollider;
    [SerializeField] private GameObject particles;

    private float angle;
    private const float ROTATE_SPEED = 720f;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!daggerRigidbody.isKinematic)
        {
            angle += ROTATE_SPEED * Time.deltaTime;
            daggerInRoot.localRotation = Quaternion.Euler(-angle, transform.rotation.y, transform.rotation.z);
            transform.LookAt(startPosition);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Ground")
        {
            daggerRigidbody.isKinematic = true;

            particles.SetActive(true);
            StartCoroutine(DisableParticles(0.8f));

            daggerCollider.enabled = false;

            transform.parent = other.gameObject.transform;

            LookToTarget(startPosition);

            if (other.gameObject.tag != "Ground")
            {
                other.gameObject.GetComponentInParent<EnemyController>().ShootOn(other.collider.name);
            }
        }
    }

    public void FlyToTarget(Vector3 target)
    {
        daggerRigidbody.AddForce(target.normalized * speed, ForceMode.Impulse);
    }

    public void LookToTarget(Vector3 target)
    {
        transform.LookAt(target);
        daggerInRoot.transform.LookAt(target);
    }

    public void StartDaggerSetup(Vector3 velocity)
    {
        daggerRigidbody.velocity = velocity;
        daggerRigidbody.isKinematic = false;
    }

    private void OnDisable()
    {
        StartDaggerSetup(Vector3.zero);
        daggerCollider.enabled = true;
    }

    IEnumerator DisableParticles(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);
        particles.SetActive(false);
    }
}
