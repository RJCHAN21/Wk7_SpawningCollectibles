using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Prefabs)")]
    [SerializeField] GameObject collectiblePrefab;
    [SerializeField] GameObject obstaclePrefab;

    [Header("Spawn Settings")]
    [SerializeField] float spawnInterval = 2.0f;
    [SerializeField] float minY = -2f, maxY = 2f;

    [Header("Object Settings")]
    [SerializeField] float collectibleRotationSpeed = 20f;
    [SerializeField] float obstacleRotationSpeed = 50f;

    Coroutine _spawnRoutine;

    private struct Spawnable
    {
        public GameObject prefab;
        public float rotationSpeed;
    }

    void OnEnable()
    {
        if (_spawnRoutine == null)
            _spawnRoutine = StartCoroutine(SpawnLoop());
    }

    void OnDisable()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    IEnumerator SpawnLoop()
    {
        while (GameManager.Instance == null)
            yield return null;

        while (!GameManager.isGameOver)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (GameManager.isGameOver) break;

            if (Random.value > 0.5f) SpawnCollectible();
            else SpawnObstacle();
        }

        _spawnRoutine = null;
    }

    void SpawnCollectible()
    {
        Spawnable spawnable = new Spawnable
        {
            prefab = collectiblePrefab,
            rotationSpeed = collectibleRotationSpeed
        };

        Vector2 spawnPosition = new Vector2(Random.value > 0.5f ? -5f : 5f, Random.Range(minY, maxY));
        GameObject collectible = Instantiate(spawnable.prefab, spawnPosition, Quaternion.identity);
        collectible.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(spawnPosition.x > 0 ? -2f : 2f, 0f);
        collectible.GetComponent<Rigidbody2D>().angularVelocity = spawnable.rotationSpeed * (Random.value > 0.5f ? 1f : -1f);
    }

    void SpawnObstacle()
    {
        Spawnable spawnable = new Spawnable
        {
            prefab = obstaclePrefab,
            rotationSpeed = obstacleRotationSpeed
        };

        Vector2 spawnPosition = new Vector2(Random.value > 0.5f ? -5f : 5f, Random.Range(minY, maxY));
        GameObject obstacle = Instantiate(spawnable.prefab, spawnPosition, Quaternion.identity);
        obstacle.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(spawnPosition.x > 0 ? -2f : 2f, 0f);
        obstacle.GetComponent<Rigidbody2D>().angularVelocity = spawnable.rotationSpeed * (Random.value > 0.5f ? 1f : -1f);
    }
}
