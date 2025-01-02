using UnityEngine;
using System.Collections.Generic;

public class PoutreGenerator : MonoBehaviour
{
    public GameObject poutrePrefab;
    public float distanceDevantJoueur = 20f; // Distance devant le joueur où les poutres seront générées
    public float rayonGeneration = 20f;
    public float cellSize = 5f;
    public int nombrePoutres = 10;

    private Dictionary<Vector2Int, bool> grid = new Dictionary<Vector2Int, bool>();
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Assurez-vous que votre joueur a un tag "Player"
        InitializeGrid();
        GeneratePoutres();
    }

    void InitializeGrid()
    {
        Vector3 playerPosition = playerTransform.position + playerTransform.forward * distanceDevantJoueur;

        for (float x = playerPosition.x - rayonGeneration; x < playerPosition.x + rayonGeneration; x += cellSize)
        {
            for (float z = playerPosition.z - rayonGeneration; z < playerPosition.z + rayonGeneration; z += cellSize)
            {
                grid[new Vector2Int((int)x, (int)z)] = false;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (grid.Count == 0) // Si la grille n'a pas encore été initialisée, ne rien faire
            return;

        Gizmos.color = Color.yellow;

        foreach (var cell in grid)
        {
            Vector3 cellCenter = new Vector3(cell.Key.x, playerTransform.position.y, cell.Key.y);
            Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, 0.1f, cellSize));
        }
    }

    void GeneratePoutres()
    {
        Vector3 playerPosition = playerTransform.position + playerTransform.forward * distanceDevantJoueur;

        for (int i = 0; i < nombrePoutres; i++) // Génère 10 poutres par exemple
        {
            Vector3 randomPosition = FindRandomUnoccupiedCell(playerPosition);

            if (randomPosition != Vector3.zero)
            {
                GameObject newPoutre = Instantiate(poutrePrefab, randomPosition, Quaternion.identity);
                MarkCellOccupied(randomPosition);
            }
        }
    }

    Vector3 FindRandomUnoccupiedCell(Vector3 playerPosition)
    {
        List<Vector2Int> unoccupiedCells = new List<Vector2Int>();

        foreach (var cell in grid)
        {
            if (!cell.Value) // Si la cellule n'est pas occupée
            {
                unoccupiedCells.Add(cell.Key);
            }
        }

        if (unoccupiedCells.Count > 0)
        {
            Vector2Int randomCell = unoccupiedCells[Random.Range(0, unoccupiedCells.Count)];
            return new Vector3(randomCell.x, playerPosition.y, randomCell.y);
        }

        return Vector3.zero;
    }

    void MarkCellOccupied(Vector3 position)
    {
        Vector2Int cell = new Vector2Int((int)position.x, (int)position.z);
        grid[cell] = true;
    }
}
