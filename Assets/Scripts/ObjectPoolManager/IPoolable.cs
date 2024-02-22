using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public interface IPoolable
{

    [SerializeField]
    public static List<IPoolable> poolableObjects = new List<IPoolable>();
    public string PoolKey { get; }
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
    public static void AddToPool(IPoolable poolable)
    {
        ObjectPoolManager.Instance.AddToPool(poolable);
    }

    public static T GetFromPool<T>(T poolable) where T : IPoolable
    {
        return (T)ObjectPoolManager.Instance.GetFromPool(poolable);
    }

    public static T GetFromPool<T>(string key) where T : IPoolable
    {
        return (T)ObjectPoolManager.Instance.GetFromPool(key);
    }

    public static bool AddPrefabToPool(IPoolable poolable)
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

        private Dictionary<string, Queue<IPoolable>> _pools = new Dictionary<string, Queue<IPoolable>>();

        //private Dictionary<string, GameObject> _holders = new Dictionary<string, GameObject>();

        private Dictionary<string, IPoolable> _prefabs = new Dictionary<string, IPoolable>();

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

        public bool AddPrefabToPool(IPoolable poolable)
        {
            if (_prefabs.ContainsKey(poolable.PoolKey)) return false;
            _prefabs.Add(poolable.PoolKey, poolable);
            return true;
        }

        public void AddToPool(IPoolable poolable)
        {
            string key = poolable.PoolKey;

            if (!_pools.ContainsKey(key))
                AddNewKey(key);

            //if (_holders[key] == null) _holders[key] = new GameObject(key.ToUpperInvariant() + "S");

            //poolable.MonoBehaviour.transform.SetParent(_holders[key].transform);
            poolable.MonoBehaviour.gameObject.SetActive(false);
            _pools[key].Enqueue(poolable);

            poolable.Pooled = true;
            poolable.OnAddToPool();

        }

        private void AddToPoolNewObject(IPoolable poolable)
        {
            string key = poolable.PoolKey;

            if (!_pools.ContainsKey(key))
                AddNewKey(key);

            //if (_holders[key] == null) _holders[key] = new GameObject(key.ToUpperInvariant() + "S");

            //poolable.MonoBehaviour.transform.SetParent(_holders[key].transform);
            poolable.MonoBehaviour.gameObject.SetActive(false);
            _pools[key].Enqueue(poolable);

            poolable.Pooled = true;

        }


        public IPoolable GetFromPool(IPoolable poolableInstance)
        {
            string key = poolableInstance.PoolKey;

            if (!_pools.ContainsKey(key))
                AddNewKey(key);

            IPoolable poolable;

            Queue<IPoolable> pool = _pools[key];
            bool isCreated = false;
            if (pool.Count == 0)
            {
                isCreated = true;
                GameObject gameObject = GameObject.Instantiate(poolableInstance.MonoBehaviour.gameObject);
                AddToPoolNewObject(gameObject.GetComponent<IPoolable>());
            }

            poolable = pool.Dequeue();
            if (poolable == null || poolable.MonoBehaviour == null)
            {
                isCreated = true;
                GameObject gameObject = GameObject.Instantiate(poolableInstance.MonoBehaviour.gameObject);
                poolable = gameObject.GetComponent<IPoolable>();
            }

            if (isCreated) poolable.OnCreate();
            poolable.MonoBehaviour.transform.SetParent(null);
            poolable.MonoBehaviour.gameObject.SetActive(true);
            poolable.Pooled = false;

            poolable.OnGetFromPool();

            return poolable;

        }

        public IPoolable GetFromPool(string key)
        {
            if (!_prefabs.ContainsKey(key)) return null;
            IPoolable poolable = _prefabs[key];
            return GetFromPool(poolable);
        }

        private void AddNewKey(string key)
        {
            _pools[key] = new Queue<IPoolable>();
            //_holders[key] = new GameObject(key.ToUpperInvariant() + "S");
        }


    }




}

public static class ObjectPoolManagerExtensions
{
    public static void AddToPool(this IPoolable poolable)
    {
        IPoolable.AddToPool(poolable);
    }

    public static T GetFromPool<T>(this T poolable) where T : IPoolable
    {
        return IPoolable.GetFromPool(poolable);
    }

    public static T GetFromPool<T>(this string key) where T : IPoolable
    {
        return IPoolable.GetFromPool<T>(key);
    }

    public static bool AddPrefabToPool(IPoolable poolable)
    {
        return IPoolable.AddPrefabToPool(poolable);
    }


}