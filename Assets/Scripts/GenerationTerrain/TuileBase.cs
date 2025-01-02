using System.Collections;
using UnityEngine;

// TODO: Tuile se detruit avec grappin = pas bon
public class TuileBase : MonoBehaviour
{
    // public GameObject leftWall;
    // public GameObject rightWall;
    // public GameObject frontWall;
    // public GameObject backWall;

    private int timeBeforeDestroy;
    public int TimeBeforeDestroy { get => timeBeforeDestroy; set => timeBeforeDestroy = value; }
    bool _playerEntered = false;

    public SideEnum side;
    public Transform centerOfTile;
    GenerationTerrain _generationTerrain;

    // Start is called before the first frame update
    void Start()
    {
        _generationTerrain = GenerationTerrain.instance;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _generationTerrain.numberOfTilesPassed++;

            _generationTerrain.SpawnTile();

            _playerEntered = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _playerEntered = true;
        }
    }
    public void StartDestroy()
    {
        StartCoroutine(DestroyTile());
    }

    IEnumerator DestroyTile()
    {
        if (_playerEntered) yield break;
        yield return new WaitForSeconds(timeBeforeDestroy);
        Destroy(gameObject);
    }

    public void RetirerMur(GameObject[] tMur)
    {
        foreach (GameObject mur in tMur)
        {
            if (mur != null)
            {
                mur.SetActive(false);
            }
        }
    }
}

public enum SideEnum
{

    Middle,
    Left,
    Right
}
