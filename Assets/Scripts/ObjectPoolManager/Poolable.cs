using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public interface Poolable
{

    [SerializeField]
    public static List<Poolable> poolableObjects = new List<Poolable>();
    public string Key { get; set; }
    public MonoBehaviour MonoBehaviour { get; }
    public bool Pooled { get; set; }


    public void AddToPool()
    {
        ObjectPoolManager.Instance.AddToPool(this);
        Pooled = true;
    }

    public virtual void OnCreate()
    {

    }

    public virtual void OnAddToPool()
    {

    }

    public virtual void OnGetFromPool()
    {

    }


    #region StaticMethods
    public static void AddToPool(Poolable poolable)
    {
        ObjectPoolManager.Instance.AddToPool(poolable);
    }

    public static T GetFromPool<T>(T poolable) where T : Poolable
    {
        return (T)ObjectPoolManager.Instance.GetFromPool(poolable);
    }

    public static T GetFromPool<T>(string key) where T : Poolable
    {
        return (T)ObjectPoolManager.Instance.GetFromPool(key);
    }

    public static bool AddPrefabToPool(Poolable poolable)
    {
        return ObjectPoolManager.Instance.AddPrefabToPool(poolable);
    }

    public static void Reset()
    {
        ObjectPoolManager.Instance.ResetObjectPoolManager();
    }

    #endregion

    private class ObjectPoolManager
    {

        private static ObjectPoolManager _instance;

        private Dictionary<string, Queue<Poolable>> _pools = new Dictionary<string, Queue<Poolable>>();

        //private Dictionary<string, GameObject> _holders = new Dictionary<string, GameObject>();

        private Dictionary<string, Poolable> _prefabs = new Dictionary<string, Poolable>();

        public static ObjectPoolManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ObjectPoolManager();
                }
                return _instance;
            }
        }


        public void ResetObjectPoolManager()
        {
            _instance = new ObjectPoolManager();
        }

        private ObjectPoolManager() { }

        public bool AddPrefabToPool(Poolable poolable)
        {
            if (_prefabs.ContainsKey(poolable.Key)) return false;
            _prefabs.Add(poolable.Key, poolable);
            return true;
        }

        public void AddToPool(Poolable poolable)
        {
            string key = poolable.Key;

            if (!_pools.ContainsKey(key))
                AddNewKey(key);

            //if (_holders[key] == null) _holders[key] = new GameObject(key.ToUpperInvariant() + "S");

            //poolable.MonoBehaviour.transform.SetParent(_holders[key].transform);
            poolable.MonoBehaviour.gameObject.SetActive(false);
            _pools[key].Enqueue(poolable);

            poolable.Pooled = true;
            poolable.OnAddToPool();

        }

        private void AddToPoolNewObject(Poolable poolable)
        {
            string key = poolable.Key;

            if (!_pools.ContainsKey(key))
                AddNewKey(key);

            //if (_holders[key] == null) _holders[key] = new GameObject(key.ToUpperInvariant() + "S");

            //poolable.MonoBehaviour.transform.SetParent(_holders[key].transform);
            poolable.MonoBehaviour.gameObject.SetActive(false);
            _pools[key].Enqueue(poolable);

            poolable.Pooled = true;

        }


        public Poolable GetFromPool(Poolable poolableInstance)
        {
            string key = poolableInstance.Key;

            if (!_pools.ContainsKey(key))
                AddNewKey(key);

            Poolable poolable;

            Queue<Poolable> pool = _pools[key];
            bool isCreated = false;
            if (pool.Count == 0)
            {
                isCreated = true;
                GameObject gameObject = GameObject.Instantiate(poolableInstance.MonoBehaviour.gameObject);
                AddToPoolNewObject(gameObject.GetComponent<Poolable>());
            }

            poolable = pool.Dequeue();
            if (poolable == null || poolable.MonoBehaviour == null)
            {
                isCreated = true;
                GameObject gameObject = GameObject.Instantiate(poolableInstance.MonoBehaviour.gameObject);
                Debug.Log("hi");
                poolable = gameObject.GetComponent<Poolable>();
            }

            if (isCreated) poolable.OnCreate();
            poolable.MonoBehaviour.transform.SetParent(null);
            poolable.MonoBehaviour.gameObject.SetActive(true);
            poolable.Pooled = false;

            poolable.OnGetFromPool();

            return poolable;

        }

        public Poolable GetFromPool(string key)
        {
            if (!_prefabs.ContainsKey(key)) return null;
            Poolable poolable = _prefabs[key];
            return GetFromPool(poolable);
        }

        private void AddNewKey(string key)
        {
            _pools[key] = new Queue<Poolable>();
            //_holders[key] = new GameObject(key.ToUpperInvariant() + "S");
        }


    }



}
