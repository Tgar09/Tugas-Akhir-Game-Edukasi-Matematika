using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;           // Delay antar burst
    public float projectileSpeed = 5f;
    public int burstCount = 3;           // Jumlah peluru dalam 1 burst
    public float burstInterval = 0.2f;   // Jeda antar peluru dalam burst
    public float startDelay = 5f;        // Delay sebelum tembakan pertama

    private float fireCooldown = 0f;
    private float delayTimer;
    private bool isShooting = false;

    void Start()
    {
        delayTimer = startDelay;
    }

    void Update()
    {
        // Tunggu sampai start delay selesai
        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        fireCooldown -= Time.deltaTime;

        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null && fireCooldown <= 0f && !isShooting)
        {
            StartCoroutine(BurstShoot(target.transform));
            fireCooldown = fireRate;
        }
    }

    IEnumerator BurstShoot(Transform target)
    {
        isShooting = true;

        for (int i = 0; i < burstCount; i++)
        {
            FireAtTarget(target);
            yield return new WaitForSeconds(burstInterval);
        }

        isShooting = false;
    }

    void FireAtTarget(Transform target)
    {
        Vector2 direction = (target.position - firePoint.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
    }
}
