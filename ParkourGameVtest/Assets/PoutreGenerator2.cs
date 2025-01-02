using UnityEngine;

public class PoutreGenerator2 : MonoBehaviour
{
    public GameObject poutrePrefab;
    public float distanceMin = 5f;
    public float distanceMax = 10f;
    public float rayonGeneration = 20f;
    public float cellSize = 2f; // Taille de la cellule pour la répartition spatiale

    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Assurez-vous que votre joueur a un tag "Player"
        GeneratePoutres();
    }

    void GeneratePoutres()
    {
        Vector3 playerPosition = playerTransform.position;

        // Calcul du nombre de cellules dans chaque direction
        int numCellsX = Mathf.CeilToInt(rayonGeneration * 2 / cellSize);
        int numCellsZ = Mathf.CeilToInt(rayonGeneration * 2 / cellSize);

        // Création d'un tableau pour marquer les cellules occupées
        bool[,] cellOccupied = new bool[numCellsX, numCellsZ];

        for (int i = 0; i < 10; i++) // Génère 10 poutres par exemple
        {
            bool poutrePlacée = false;

            // Tant que la poutre n'est pas placée, continuez à essayer
            while (!poutrePlacée)
            {
                // Sélectionnez une cellule aléatoire
                int randomCellX = Random.Range(0, numCellsX);
                int randomCellZ = Random.Range(0, numCellsZ);

                // Convertir les coordonnées de la cellule en position dans l'espace
                Vector3 randomCellPosition = new Vector3(
                    (randomCellX + 0.5f) * cellSize - rayonGeneration,
                    0,
                    (randomCellZ + 0.5f) * cellSize - rayonGeneration
                );

                // Ajouter la position du joueur pour obtenir la position de la cellule dans le monde
                Vector3 poutrePosition = randomCellPosition + playerPosition;

                // Vérifier si la poutre est dans la plage de distances
                float distanceToPlayer = Vector3.Distance(poutrePosition, playerPosition);
                if (distanceToPlayer >= distanceMin && distanceToPlayer <= distanceMax)
                {
                    // Vérifier si la cellule est libre
                    if (!cellOccupied[randomCellX, randomCellZ])
                    {
                        // Instancier la poutre à la position calculée
                        GameObject newPoutre = Instantiate(poutrePrefab, poutrePosition, Quaternion.identity);
                        // Assurez-vous de régler la rotation et d'autres propriétés selon vos besoins

                        // Marquer la cellule comme occupée
                        cellOccupied[randomCellX, randomCellZ] = true;

                        // La poutre est placée
                        poutrePlacée = true;
                    }
                }
            }
        }
    }
}
