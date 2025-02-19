using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Các đối tượng cần spawn
    [SerializeField] private GameObject asteroids;  // Prefab của thiên thạch
    [SerializeField] private GameObject star;       // Prefab của ngôi sao
    [SerializeField] private GameObject healthItem; // Prefab của vật phẩm hồi máu
    [SerializeField] private GameObject weaponCrate; // Prefab của thùng vũ khí

    // Collider của vùng spawn
    private BoxCollider2D box;

    // Biến kiểm soát thời gian spawn thùng vũ khí
    private float lastWeaponCrateSpawnTime = 0f; 
    private float weaponCrateCooldown = 20f; // Cooldown 20 giây cho thùng vũ khí

    // Biến kiểm soát thời gian spawn vật phẩm hồi máu
    private float lastHealthItemSpawnTime = 0f;
    private float healthItemCooldown = 5f; // Cooldown 5 giây cho vật phẩm hồi máu

    void Awake()
    {
        // Lấy BoxCollider2D để xác định vùng spawn
        box = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        // Bắt đầu quá trình spawn
        StartCoroutine(SpawerField());
    }

    IEnumerator SpawerField()
    {
        // Chờ ngẫu nhiên từ 1 - 3 giây trước khi spawn đợt tiếp theo
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        // Xác định giới hạn vị trí spawn theo chiều ngang
        float minX = -box.bounds.size.x / 2f;
        float maxX = box.bounds.size.x / 2f;

        // Xác định số lượng thiên thạch sẽ spawn
        int asteroidCount = GetRandomAsteroidCount();
        List<Vector3> asteroidPositions = new List<Vector3>();

        // Spawn thiên thạch với vị trí ngẫu nhiên, tránh trùng lặp
        for (int i = 0; i < asteroidCount; i++)
        {
            Vector3 position;
            int attempts = 0;
            do
            {
                position = new Vector3(Random.Range(minX, maxX), transform.position.y, transform.position.z);
                attempts++;
            }
            while (IsOverlapping(position, asteroidPositions) && attempts < 10); // Kiểm tra xem có bị trùng lặp không

            asteroidPositions.Add(position);
        }

        // Tạo thiên thạch tại các vị trí đã xác định
        foreach (var position in asteroidPositions)
        {
            Instantiate(asteroids, position, Quaternion.identity);
        }

        // 95% spawn một ngôi sao
        if (Random.Range(0f, 1f) <= 0.95f)
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

        // 5% cơ hội spawn vật phẩm hồi máu, kiểm tra cooldown trước khi spawn
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

            // Cập nhật thời điểm spawn vật phẩm hồi máu lần cuối
            lastHealthItemSpawnTime = Time.time;
        }

        // 5% cơ hội spawn thùng vũ khí, kiểm tra cooldown trước khi spawn
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

            // Cập nhật thời điểm spawn thùng vũ khí lần cuối
            lastWeaponCrateSpawnTime = Time.time;
        }

        // Tiếp tục vòng lặp spawn
        StartCoroutine(SpawerField());
    }

    // Kiểm tra xem vị trí spawn có bị trùng lặp với các vật thể khác hay không
    private bool IsOverlapping(Vector3 position, List<Vector3> existingPositions, float minDistance = 1f)
    {
        foreach (var existing in existingPositions)
        {
            if (Vector3.Distance(position, existing) < minDistance)
            {
                return true; // Nếu khoảng cách giữa hai vật thể quá gần, trả về true
            }
        }
        return false;
    }

    // Xác định số lượng thiên thạch ngẫu nhiên theo tỷ lệ nhất định
    private int GetRandomAsteroidCount()
    {
        float randomValue = Random.Range(0f, 1f);

        if (randomValue <= 0.005f) return 5;  // 0.5% spawn 5 thiên thạch
        else if (randomValue <= 0.045f) return 4;  // 4.5% spawn 4 thiên thạch
        else if (randomValue <= 0.2f) return 3;  // 20% spawn 3 thiên thạch
        else return 2;  // 75% còn lại spawn 2 thiên thạch
    }
}
