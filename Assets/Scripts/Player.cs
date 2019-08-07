using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // config param
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 1f;
    [SerializeField] float health = 500;

    [Header("Projectile")]
    [SerializeField] GameObject projectile;
    [SerializeField] float laserSpeed = 10f;
    [SerializeField] float laserFireingPeriod = 0.1f;

    Coroutine firingCoroutine;

    float xMin;
    float xMax;
    float yMin;
    float yMax;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
        StartCoroutine(FireContinuously());
    }


    // Update is called once per frame
    void Update()
    {
        Move();
    }

    IEnumerator FireContinuously()
    {
        WaitForSeconds delay = new WaitForSeconds(laserFireingPeriod);

        while (true)
        {
            if (Input.GetButton("Fire1"))
            {
                GameObject laser = Instantiate(
                    projectile,
                    transform.position,
                    Quaternion.identity) as GameObject;

                laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, laserSpeed);

                yield return delay;
            }

            yield return null; // come back in the next frame
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
            DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
            if (!damageDealer) { return; }
            ProcessHit(damageDealer);
        
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        var newXpos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYpos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        transform.position = new Vector2(newXpos, newYpos);
    }


    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;

    }
}
