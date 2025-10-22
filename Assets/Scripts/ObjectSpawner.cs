using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType { Gem, BigGem, Enemy }

    public Tilemap tilemap;
    public GameObject[] objectPrefabs;
    public float gemProbability;
    public float bigGemProbability = 0.2f;
    public float enemyProbability = 0.1f;
    
    private int _enemiesCount = 0;
    public float gemLifeTime = 10f;
    public float spawnInterval = 0.5f;

    private List<Vector3> _validSpawnPositions = new List<Vector3>();
    private List<GameObject> _spawnObjects = new List<GameObject>();
    
    private bool _isSpawning = false;

    [FormerlySerializedAs("LevelConfiguration")] public LevelConfiguration levelConfiguration;
    
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // gather valid positions
        GatherValidPositions();
        
        // spawn objects
        StartCoroutine(SpawnObjectsIfNeeded());

        GameController.OnReset += LevelChange;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!tilemap.gameObject.activeInHierarchy)
        {
            //Level change
            LevelChange();
        }
        if (!_isSpawning && ActiveObjectCount() < levelConfiguration.GetMaxObjects())
        {
            StartCoroutine(SpawnObjectsIfNeeded());
        }
    }

    private void LevelChange()
    {
        DestroyAllSpawnedObjects();
        tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        GatherValidPositions();
    }

    private int ActiveObjectCount()
    {
        _spawnObjects.RemoveAll(item => !item.activeSelf);
        return _spawnObjects.Count;
    }

    private IEnumerator SpawnObjectsIfNeeded()
    {
        _isSpawning = true;
        while (ActiveObjectCount() < levelConfiguration.GetMaxObjects())
        {
            // Spawn Object
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
        _isSpawning = false;
    }

    private bool PositionHasObject(Vector3 positionToCheck)
    {
        return _spawnObjects.Any(checkObj =>
            checkObj && Vector3.Distance(checkObj.transform.position, positionToCheck) < 1.0f);
    }

    private ObjectType GetRandomObjectType()
    {
        while (true)
        {
            float randomChoice = Random.value;

            if (randomChoice <= enemyProbability)
            {
                if (_enemiesCount >= levelConfiguration.GetMaxEnemies()) continue;
                _enemiesCount++;
                return ObjectType.Enemy;
            }else if (randomChoice <= (enemyProbability + bigGemProbability))
            {
                return ObjectType.BigGem;
            }
            else
            {
                return ObjectType.Gem;
            }
        }
    }

    private void SpawnObject()
    {
        if (_validSpawnPositions.Count == 0) return;
        
        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;
        while (!validPositionFound && _validSpawnPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, _validSpawnPositions.Count);
            Vector3 potentialPosition = _validSpawnPositions[randomIndex];
            Vector3 leftPosition = potentialPosition + Vector3.left;
            Vector3 rightPosition = potentialPosition + Vector3.right;

            if (!PositionHasObject(leftPosition) && !PositionHasObject(rightPosition))
            {
                spawnPosition = potentialPosition;
                validPositionFound = true;
            }
            
            _validSpawnPositions.RemoveAt(randomIndex);
        }

        if (validPositionFound)
        {
            ObjectType objectType = GetRandomObjectType();
            
            if (_spawnObjects.Count >= levelConfiguration.GetMaxObjects()) return;
            // GameObject gameObject = Instantiate(objectPrefabs[(int)objectType],  spawnPosition, Quaternion.identity);
            var newObj = ObjectPoolManager.SpawnObject(objectPrefabs[(int)objectType], spawnPosition, Quaternion.identity);
            _spawnObjects.Add(newObj);
            
            // destroy gems after a time
            if (objectType != ObjectType.Enemy)
            {
                StartCoroutine(DestroyObjectAfterTime(newObj, gemLifeTime));
            }
        }
    }

    private IEnumerator DestroyObjectAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);

        if (obj)
        {
            _spawnObjects.Remove(obj);
            _validSpawnPositions.Add(gameObject.transform.position);
            if(obj.activeSelf) ObjectPoolManager.ReturnObjectToPool(obj);
        }
    }

    private void DestroyAllSpawnedObjects()
    {
        foreach (var obj in _spawnObjects)
        {
            if (obj && obj.activeSelf)
            {
                ObjectPoolManager.ReturnObjectToPool(obj);
            }
        }
        _spawnObjects.Clear();
        _enemiesCount = 0;
    }
    private void GatherValidPositions()
    {
        _validSpawnPositions.Clear();
        BoundsInt boundsInt = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(boundsInt);

        Vector3 start = tilemap.CellToWorld(new Vector3Int(boundsInt.xMin, boundsInt.yMin, 0));

        for (int x = 0; x < boundsInt.size.x; x++)
        {
            for (int y = 0; y < boundsInt.size.y; y++)
            {
                TileBase tile = allTiles[y * boundsInt.size.x + x];
                if (tile)
                {
                    Vector3 place = start + new Vector3(x + 1f, y + 2f, 0);
                    _validSpawnPositions.Add(place);
                }
            }
        }
    }
}
