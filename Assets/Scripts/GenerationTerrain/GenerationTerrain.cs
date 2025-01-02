using System.Collections.Generic;
using UnityEngine;

public class GenerationTerrain : MonoBehaviour
{
    static private GenerationTerrain _instance; //creation du singleton pour la classe niveau
    static public GenerationTerrain instance => _instance;
    public TuileBase[] tTuileBase;
    public int nombreTuilesBase = 10;
    public int timeBeforeDestroy = 10;
    public Vector3 positionToFolow;

    private int _numberOfTilesPassed = 0;
    public int numberOfTilesPassed { get { return _numberOfTilesPassed; } set { _numberOfTilesPassed = value; } }

    Vector3 _nextSpawnPoint;
    Vector3 _leftSpawnPoint;
    Vector3 _rightSpawnPoint;

    Dictionary<Vector2Int, TuileBase> _map = new Dictionary<Vector2Int, TuileBase>();

    void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;
    }

    void Start()
    {
        for (int i = 0; i < nombreTuilesBase; i++)
        {
            SpawnTile(i);
        }
    }

    public void SpawnTile(int indexTuile = 1)
    {
        // Debug.Log("SpawnTile");
        int randomIndex = Random.Range(0, tTuileBase.Length);
        int randomRotation = Random.Range(0, 4) * 90;
        TuileBase tuileTemp = Instantiate(tTuileBase[randomIndex], _nextSpawnPoint, Quaternion.identity);
        Transform allBuildings = tuileTemp.gameObject.transform.GetChild(3);
        allBuildings.transform.RotateAround(allBuildings.GetChild(0).position, Vector3.up, randomRotation);

        tuileTemp.TimeBeforeDestroy = timeBeforeDestroy;
        tuileTemp.side = SideEnum.Middle;
        tuileTemp.gameObject.name = $"{tuileTemp.gameObject.name}";

        if (_numberOfTilesPassed == 3)
        {
            positionToFolow = allBuildings.GetChild(0).position;
        }
        if (_numberOfTilesPassed > 3)
        {
            _numberOfTilesPassed = 0;
        }


        // CheckTile((int)tuileTemp.transform.position.x, (int)tuileTemp.transform.position.z, tuileTemp);

        _nextSpawnPoint = tuileTemp.gameObject.transform.GetChild(0).transform.position;
        _leftSpawnPoint = tuileTemp.gameObject.transform.GetChild(1).transform.position;
        _rightSpawnPoint = tuileTemp.gameObject.transform.GetChild(2).transform.position;



        int randSide = Random.Range(0, 3);

        int chanceToFocus = Random.Range(0, 2);

        if (randSide == 0)
        {
            //left tile
            // GameObject[] tMur = new GameObject[] { tuileTemp.rightWall, indexTuile == 0 ? null : tuileTemp.backWall, chanceToFocus == 1 ? null : tuileTemp.frontWall };
            // tuileTemp.RetirerMur(tMur);
            SpawnSideTile(tuileTemp, _leftSpawnPoint, SideEnum.Left, indexTuile, chanceToFocus);
        }
        else if (randSide == 1)
        {
            //right tile
            // GameObject[] tMur = new GameObject[] { tuileTemp.leftWall, indexTuile == 0 ? null : tuileTemp.backWall, chanceToFocus == 1 ? null : tuileTemp.frontWall };
            // tuileTemp.RetirerMur(tMur);
            SpawnSideTile(tuileTemp, _rightSpawnPoint, SideEnum.Right, indexTuile, chanceToFocus);
        }
        else
        {

            //no additional tile
            // GameObject[] tMur = new GameObject[] { indexTuile == 0 ? null : tuileTemp.backWall, tuileTemp.frontWall };
            // tuileTemp.RetirerMur(tMur);
            // Debug.Log($"{tuileTemp.gameObject.name} Middle");
        }
    }



    private void SpawnSideTile(TuileBase parentTile, Vector3 spawnPoint, SideEnum side, int index, int chanceToFocus)
    {
        int randomIndexCote = Random.Range(0, tTuileBase.Length);
        // Debug.Log($"{parentTile.gameObject.name} {side} {index} ");

        TuileBase sideTile = Instantiate(tTuileBase[randomIndexCote], spawnPoint + new Vector3(side == SideEnum.Left ? -20 : 20, 0, -20), Quaternion.identity);
        sideTile.TimeBeforeDestroy = timeBeforeDestroy;
        sideTile.side = side;
        sideTile.gameObject.name = $"{sideTile.gameObject.name}";


        // Debug.Log($"ChanceToFocus {side}: {chanceToFocus} {sideTile.gameObject.name} {index}");

        // CheckTile((int)sideTile.transform.position.x, (int)sideTile.transform.position.z, sideTile);
        // GameObject[] tMur = new GameObject[] { side == SideEnum.Right ? sideTile.rightWall : sideTile.leftWall, chanceToFocus == 1 ? sideTile.frontWall : null };
        // GameObject[] tMur = new GameObject[] { side == SideEnum.Right ? sideTile.rightWall : sideTile.leftWall, index == 0 ? null : sideTile.backWall, chanceToFocus == 1 ? sideTile.frontWall : null };
        // sideTile.RetirerMur(tMur);

        if (chanceToFocus == 1)
        {
            //focus on the side tile
            if (_numberOfTilesPassed == 3)
            {
                Transform allBuildings = sideTile.gameObject.transform.GetChild(3);
                positionToFolow = allBuildings.GetChild(0).position;
            }

            if (_numberOfTilesPassed > 3)
            {
                _numberOfTilesPassed = 0;
            }

            _nextSpawnPoint = sideTile.gameObject.transform.GetChild(0).transform.position;
            _leftSpawnPoint = sideTile.gameObject.transform.GetChild(1).transform.position;
            _rightSpawnPoint = sideTile.gameObject.transform.GetChild(2).transform.position;
        }
    }

    // void CheckTile(int x, int z, TuileBase tuile, bool isRecursive = true)
    // {
    //     // _map.Add(new MapKey(x, z), 1);
    //     Vector2Int coord = new Vector2Int(x, z);

    //     if (!_map.ContainsKey(coord))
    //     {
    //         _map.Add(coord, tuile);

    //     }

    //     GameObject[] tMur = new GameObject[4];

    //     Vector2Int leftCoord = new Vector2Int(x - 10, z);
    //     Vector2Int rightCoord = new Vector2Int(x + 10, z);
    //     Vector2Int frontCoord = new Vector2Int(x, z + 10);
    //     Vector2Int backCoord = new Vector2Int(x, z - 10);

    //     // TODO : Arrete apres 2, faire que ca fait vraiment tout les tuiles.

    //     //gauche
    //     if (_map.ContainsKey(leftCoord))
    //     {
    //         tMur[0] = tuile.leftWall;
    //         TuileBase tuileLeft = _map[leftCoord];
    //         if (isRecursive) CheckTile(x - 10, z, tuileLeft, false);
    //     }

    //     //droite
    //     if (_map.ContainsKey(rightCoord))
    //     {
    //         tMur[1] = tuile.rightWall;
    //         TuileBase tuileRight = _map[rightCoord];
    //         if (isRecursive) CheckTile(x + 10, z, tuileRight, false);
    //     }

    //     //avant
    //     if (_map.ContainsKey(frontCoord))
    //     {
    //         tMur[2] = tuile.frontWall;

    //     }
    //     //arriere
    //     if (_map.ContainsKey(backCoord))
    //     {
    //         tMur[3] = tuile.backWall;
    //         TuileBase tuileBack = _map[backCoord];
    //         if (isRecursive) CheckTile(x, z - 10, tuileBack, false);
    //     }

    //     // Debug.Log($"{tuile.gameObject.name} {tMur[0]} {tMur[1]} {tMur[2]} {tMur[3]}");

    //     tuile.RetirerMur(tMur);
    // }

}