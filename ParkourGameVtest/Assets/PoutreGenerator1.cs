using UnityEngine;

public class PoutreGenerator1 : MonoBehaviour
{
    public GameObject poutrePrefab;
    public float rayonGeneration = 20f;
    public float distanceEntreGenerations = 50f;
    public int nombreDePoutresInitiales = 10;
    public int incrementPoutres = 5;
    public float distanceMinimaleEntrePoutres = 5f;
    public float distanceMaximaleEntrePoutres = 10f;

    private Transform playerTransform;
    private Vector3 lastPlayerPosition;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lastPlayerPosition = playerTransform.position;
        GenerateInitialPoutres();
    }

    void Update()
    {
        if (playerTransform.position.z - lastPlayerPosition.z >= distanceEntreGenerations)
        {
            GeneratePoutres();
            lastPlayerPosition = playerTransform.position;
        }
    }

    void GenerateInitialPoutres()
    {
        for (int i = 0; i < nombreDePoutresInitiales; i++)
        {
            Vector3 randomPosition = GetRandomPositionInFront(playerTransform.position);
            Instantiate(poutrePrefab, randomPosition, Quaternion.identity);
        }
    }

    void GeneratePoutres()
    {
        for (int i = 0; i < incrementPoutres; i++)
        {
            Vector3 randomPosition = GetRandomPositionInFront(playerTransform.position);
            Instantiate(poutrePrefab, randomPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomPositionInFront(Vector3 playerPosition)
    {
        Vector3 playerDirection = (playerPosition - lastPlayerPosition).normalized;
        Vector3 perpendicularDirection = new Vector3(-playerDirection.z, 0, playerDirection.x); // Perpendicular à la direction de déplacement
        Vector3 randomPosition = playerPosition + perpendicularDirection * Random.Range(-rayonGeneration, rayonGeneration);

        for (int i = 0; i < 100; i++)
        {
            randomPosition = playerPosition + perpendicularDirection * Random.Range(-rayonGeneration, rayonGeneration);
            randomPosition.y = 0;

            if (DistanceToNearestPoutre(randomPosition) >= distanceMinimaleEntrePoutres &&
                DistanceToNearestPoutre(randomPosition) <= distanceMaximaleEntrePoutres)
            {
                break;
            }
        }

        return randomPosition;
    }

    float DistanceToNearestPoutre(Vector3 position)
    {
        float minDistance = Mathf.Infinity;
        GameObject[] poutres = GameObject.FindGameObjectsWithTag("Poutre");

        foreach (GameObject poutre in poutres)
        {
            float distance = Vector3.Distance(position, poutre.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }

        return minDistance;
    }
}
