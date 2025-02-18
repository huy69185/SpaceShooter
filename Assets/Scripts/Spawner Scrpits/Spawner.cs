using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject asteroids;

    [SerializeField]
    private GameObject star;

    [SerializeField]
    private GameObject healthItem;  // Prefab máu

    [SerializeField]
    private GameObject weaponCrate; // Prefab kho vũ khí

    private BoxCollider2D box;

    private float lastWeaponCrateSpawnTime = 0f; // Track the last weapon crate spawn time
    private float weaponCrateCooldown = 20f; // Cooldown time for weapon crate spawn

    private float lastHealthItemSpawnTime = 0f; // Track the last health item spawn time
    private float healthItemCooldown = 5f; // Cooldown time for health item spawn

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        StartCoroutine(SpawerField());
    }

    IEnumerator SpawerField()
    {
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        float minX = -box.bounds.size.x / 2f;
        float maxX = box.bounds.size.x / 2f;

        int asteroidCount = GetRandomAsteroidCount();
        List<Vector3> asteroidPositions = new List<Vector3>();

        for (int i = 0; i < asteroidCount; i++)
        {
            Vector3 position;
            int attempts = 0;
            do
            {
                position = new Vector3(Random.Range(minX, maxX), transform.position.y, transform.position.z);
                attempts++;
            }
            while (IsOverlapping(position, asteroidPositions) && attempts < 10); // Kiểm tra vị trí trùng lặp

            asteroidPositions.Add(position);
        }

        foreach (var position in asteroidPositions)
        {
            Instantiate(asteroids, position, Quaternion.identity);
        }

        if (Random.Range(0f, 1f) <= 0.95f) // 95% spawn sao
        {
            Vector3 starPosition;
            int attempts = 0;
            do
            {
                starPosition = new Vector3(Random.Range(minX, maxX), transform.position.y, transform.position.z);
                attempts++;
            }
            while (IsOverlapping(starPosition, asteroidPositions) && attempts < 10);

            Instantiate(star, starPosition, Quaternion.identity);
        }

        // 5% chance for health item with cooldown of 5 seconds
        if (Random.Range(0f, 1f) <= 0.05f && Time.time - lastHealthItemSpawnTime >= healthItemCooldown)
        {
            Vector3 healthPosition;
            int attempts = 0;
            do
            {
                healthPosition = new Vector3(Random.Range(minX, maxX), transform.position.y, transform.position.z);
                attempts++;
            }
            while (IsOverlapping(healthPosition, asteroidPositions) && attempts < 10);

            Instantiate(healthItem, healthPosition, Quaternion.identity);

            // Update the last spawn time for health item
            lastHealthItemSpawnTime = Time.time;
        }

        // 5% chance for weapon crate, with cooldown of 20 seconds
        if (Random.Range(0f, 1f) <= 0.05f && Time.time - lastWeaponCrateSpawnTime >= weaponCrateCooldown)
        {
            Vector3 weaponPosition;
            int attempts = 0;
            do
            {
                weaponPosition = new Vector3(Random.Range(minX, maxX), transform.position.y, transform.position.z);
                attempts++;
            }
            while (IsOverlapping(weaponPosition, asteroidPositions) && attempts < 10);

            Instantiate(weaponCrate, weaponPosition, Quaternion.identity);

            // Update the last spawn time for weapon crate
            lastWeaponCrateSpawnTime = Time.time;
        }

        StartCoroutine(SpawerField());
    }

    private bool IsOverlapping(Vector3 position, List<Vector3> existingPositions, float minDistance = 1f)
    {
        foreach (var existing in existingPositions)
        {
            if (Vector3.Distance(position, existing) < minDistance)
            {
                return true;
            }
        }
        return false;
    }

    private int GetRandomAsteroidCount()
    {
        float randomValue = Random.Range(0f, 1f);

        if (randomValue <= 0.005f) return 5;  // 0.5% cho 5 thiên thạch
        else if (randomValue <= 0.045f) return 4;  // 4.5% cho 4 thiên thạch
        else if (randomValue <= 0.2f) return 3;  // 20% cho 3 thiên thạch
        else return 2;  // 75% còn lại cho 2 thiên thạch
    }
}
