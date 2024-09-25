using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{

    private static ObjectPoolManager _instance;
    public static ObjectPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find an existing instance in the scene
                _instance = FindObjectOfType<ObjectPoolManager>();

                // If not found, create a new GameObject and add ObjectPoolManager to it
                if (_instance == null)
                {
                    GameObject obj = new GameObject(nameof(ObjectPoolManager));
                    _instance = obj.AddComponent<ObjectPoolManager>();
                }
            }
            return _instance;
        }
    }

    [field: SerializeField] public List<MonoBehaviour> Prefabs { get; private set; }


    private Dictionary<string, Queue<IPoolableObject>> _pools = new Dictionary<string, Queue<IPoolableObject>>();
    private Dictionary<string, IPoolableObject> _prefabRegistry = new Dictionary<string, IPoolableObject>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        List<MonoBehaviour> initialPrefabs = new List<MonoBehaviour>(Prefabs);
        Prefabs.Clear();
        foreach (var prefab in initialPrefabs)
        {
            if(prefab is IPoolableObject poolable)
            {
                RegisterPrefab(poolable);
            }
        }

    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }


    public void ResetPoolManager()
    {
        _pools.Clear();
        _prefabRegistry.Clear();
        Prefabs.Clear();
    }


    public bool RegisterPrefab(IPoolableObject poolablePrefab)
    {
        if (_prefabRegistry.ContainsKey(poolablePrefab.PrefabID))
            return false;
        _prefabRegistry.Add(poolablePrefab.PrefabID, poolablePrefab);
        Prefabs.Add(poolablePrefab.MonoBehaviour);
        return true;
    }


    public void AddToPool(IPoolableObject poolable)
    {
        string key = poolable.PrefabID;

        if (!_pools.ContainsKey(key))
            AddNewKey(key);

        poolable.MonoBehaviour.gameObject.SetActive(false);
        poolable.MonoBehaviour.transform.SetParent(null);
        _pools[key].Enqueue(poolable);

        poolable.Pooled = true;
        poolable.OnAddToPool();
    }

    private void AddToPoolNewObject(IPoolableObject poolable)
    {
        string key = poolable.PrefabID;
        if (!_pools.ContainsKey(key))
            AddNewKey(key);

        poolable.MonoBehaviour.gameObject.SetActive(false);
        _pools[key].Enqueue(poolable);

        poolable.Pooled = true;
    }


    public IPoolableObject GetFromPool(IPoolableObject prefab)
    {
        string key = prefab.PrefabID;

        if (!_pools.ContainsKey(key))
            AddNewKey(key);

        IPoolableObject poolable;
        Queue<IPoolableObject> pool = _pools[key];
        bool isCreated = false;
        if (pool.Count == 0)
        {
            isCreated = true;
            GameObject gameObject = Object.Instantiate(prefab.MonoBehaviour.gameObject);
            AddToPoolNewObject(gameObject.GetComponent<IPoolableObject>());
        }

        poolable = pool.Dequeue();
        if (poolable == null || poolable.MonoBehaviour == null)
        {
            isCreated = true;
            GameObject gameObject = GameObject.Instantiate(prefab.MonoBehaviour.gameObject);
            poolable = gameObject.GetComponent<IPoolableObject>();
        }

        if (isCreated) poolable.OnCreated();
        poolable.MonoBehaviour.transform.SetParent(null);
        poolable.MonoBehaviour.gameObject.SetActive(true);
        poolable.Pooled = false;

        poolable.OnGetFromPool();

        return poolable;
    }

    public IPoolableObject GetFromPool(string key)
    {
        if (!_prefabRegistry.ContainsKey(key))
        {
            while (_pools.ContainsKey(key) && _pools[key].Count > 0)
            {
                IPoolableObject instance = _pools[key].Dequeue();
                if (instance == null || instance.MonoBehaviour == null)
                    continue;
                instance.MonoBehaviour.gameObject.SetActive(true);
                return instance;
            }

            return null;
        }
        IPoolableObject poolable = _prefabRegistry[key];
        return GetFromPool(poolable);
    }


    private void AddNewKey(string key)
    {
        _pools[key] = new Queue<IPoolableObject>();
    }


}
