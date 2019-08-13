using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int scoreValue = 150;

    [Header("Shooting")]
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject projectile;
    [SerializeField] float laserSpeed = 10;
    [SerializeField] GameObject destroyEffect;
    [Header("Sounds")]
    [SerializeField] AudioClip destroySound;
    [SerializeField] AudioClip laserSound;
    [SerializeField] [Range(0, 1)] float volume = 0.75f;
    [SerializeField] [Range(0, 1)] float laserVolume = 0.75f;


    void Start()
    {
        ResetFireCounter();
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if(shotCounter <= 0)
        {
            Fire();
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(
            projectile,
            transform.position,
            Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -laserSpeed);
        ResetFireCounter();
        AudioSource.PlayClipAtPoint(laserSound, Camera.main.transform.position, laserVolume);
    }

    private void ResetFireCounter()
    {
        shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
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
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
        Destroy(gameObject);
        GameObject particles = Instantiate(destroyEffect, transform.position, Quaternion.identity) as GameObject;
        Destroy(particles, 1f);
        AudioSource.PlayClipAtPoint(destroySound, Camera.main.transform.position, volume);
    }
}
