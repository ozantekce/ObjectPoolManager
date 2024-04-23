using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines an interface for poolable objects that can be reused from a pool to manage resource allocation efficiently.
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Identifier for the prefab associated with the object, used for pooling mechanisms.
    /// </summary>
    public string PrefabID { get; set; }

    /// <summary>
    /// Reference to the MonoBehaviour component associated with this poolable object.
    /// </summary>
    public MonoBehaviour MonoBehaviour { get; }

    /// <summary>
    /// Flag indicating whether the object is currently pooled.
    /// </summary>
    public bool Pooled { get; set; }

    /// <summary>
    /// Adds this object to its pool and sets its Pooled status to true.
    /// </summary>
    public void AddToPool()
    {
        ObjectPoolManager.Instance.AddToPool(this);
        Pooled = true;
    }

    /// <summary>
    /// Called when the object is created. Override this method to include initialization logic.
    /// </summary>
    public virtual void OnCreate()
    {
    }

    /// <summary>
    /// Called when the object is added to the pool. Override this method to include logic that should execute when the object is pooled.
    /// </summary>
    public virtual void OnAddToPool()
    {
    }

    /// <summary>
    /// Called when the object is retrieved from the pool. Override this method to include logic that should execute when the object is activated.
    /// </summary>
    public virtual void OnGetFromPool()
    {
    }

    #region StaticMethods
    /// <summary>
    /// Static method to add a poolable object to the pool.
    /// </summary>
    /// <param name="poolable">The poolable object to add.</param>
    public static void AddToPool(IPoolable poolable)
    {
        ObjectPoolManager.Instance.AddToPool(poolable);
    }

    /// <summary>
    /// Retrieves a poolable object of a specified type from the pool.
    /// </summary>
    /// <param name="poolable">The poolable object to retrieve.</param>
    /// <typeparam name="T">The type of the poolable object.</typeparam>
    /// <returns>The retrieved object.</returns>
    public static T GetFromPool<T>(T poolable) where T : IPoolable
    {
        return (T)ObjectPoolManager.Instance.GetFromPool(poolable);
    }

    /// <summary>
    /// Retrieves a poolable object from the pool using a specified key.
    /// </summary>
    /// <param name="key">The key associated with the object in the pool.</param>
    /// <typeparam name="T">The type of the poolable object.</typeparam>
    /// <returns>The retrieved object.</returns>
    public static T GetFromPool<T>(string key) where T : IPoolable
    {
        return (T)ObjectPoolManager.Instance.GetFromPool(key);
    }

    /// <summary>
    /// Adds a prefab to the pool, making it available for instantiation and reuse.
    /// </summary>
    /// <param name="poolable">The poolable object that serves as the prefab.</param>
    /// <returns>True if the prefab was added successfully, false if it already exists in the pool.</returns>
    public static bool AddPrefabToPool(IPoolable poolable)
    {
        return ObjectPoolManager.Instance.AddPrefabToPool(poolable);
    }

    /// <summary>
    /// Resets the object pool manager, clearing all pools.
    /// </summary>
    public static void Reset()
    {
        ObjectPoolManager.Instance.ResetObjectPoolManager();
    }
    #endregion

    /// <summary>
    /// Manages the pooling of objects to optimize resource allocation and performance.
    /// </summary>
    private class ObjectPoolManager
    {
        /// <summary>
        /// Singleton instance of the ObjectPoolManager.
        /// </summary>
        private static ObjectPoolManager _instance;

        /// <summary>
        /// Dictionary mapping prefab IDs to queues of poolable objects.
        /// </summary>
        private Dictionary<string, Queue<IPoolable>> _pools = new Dictionary<string, Queue<IPoolable>>();

        /// <summary>
        /// Dictionary mapping prefab IDs to their corresponding prefab instances.
        /// </summary>
        private Dictionary<string, IPoolable> _prefabs = new Dictionary<string, IPoolable>();

        /// <summary>
        /// Provides a global access point to the ObjectPoolManager instance.
        /// </summary>
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

        /// <summary>
        /// Resets the ObjectPoolManager, clearing all existing pools and prefabs.
        /// </summary>
        public void ResetObjectPoolManager()
        {
            _instance = null;
        }

        /// <summary>
        /// Constructs a new instance of ObjectPoolManager.
        /// </summary>
        private ObjectPoolManager() { }

        /// <summary>
        /// Adds a prefab to the pool if it does not already exist.
        /// </summary>
        /// <param name="poolable">The poolable instance to add as a prefab.</param>
        /// <returns>True if the prefab was successfully added, false if it already exists.</returns>
        public bool AddPrefabToPool(IPoolable poolable)
        {
            if (_prefabs.ContainsKey(poolable.PrefabID)) return false;
            _prefabs.Add(poolable.PrefabID, poolable);
            return true;
        }

        /// <summary>
        /// Adds an object to the pool and sets it as inactive.
        /// </summary>
        /// <param name="poolable">The poolable object to add to the pool.</param>
        public void AddToPool(IPoolable poolable)
        {
            string key = poolable.PrefabID;

            if (!_pools.ContainsKey(key))
                AddNewKey(key);

            poolable.MonoBehaviour.gameObject.SetActive(false);
            _pools[key].Enqueue(poolable);

            poolable.Pooled = true;
            poolable.OnAddToPool();
        }

        /// <summary>
        /// Helper method to add a new object to the pool.
        /// </summary>
        /// <param name="poolable">The poolable object to add.</param>
        private void AddToPoolNewObject(IPoolable poolable)
        {
            string key = poolable.PrefabID;
            if (!_pools.ContainsKey(key))
                AddNewKey(key);

            poolable.MonoBehaviour.gameObject.SetActive(false);
            _pools[key].Enqueue(poolable);

            poolable.Pooled = true;
        }

        /// <summary>
        /// Retrieves an object from the pool, creating a new instance if necessary.
        /// </summary>
        /// <param name="poolableInstance">The instance of the poolable object to retrieve.</param>
        /// <returns>The retrieved poolable object.</returns>
        public IPoolable GetFromPool(IPoolable poolableInstance)
        {
            string key = poolableInstance.PrefabID;

            if (!_pools.ContainsKey(key))
                AddNewKey(key);

            IPoolable poolable;
            Queue<IPoolable> pool = _pools[key];
            bool isCreated = false;
            if (pool.Count == 0)
            {
                isCreated = true;
                GameObject gameObject = Object.Instantiate(poolableInstance.MonoBehaviour.gameObject);
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

        /// <summary>
        /// Retrieves an object from the pool using its prefab key.
        /// </summary>
        /// <param name="key">The key associated with the prefab to retrieve.</param>
        /// <returns>The poolable object, if available; otherwise, null.</returns>
        public IPoolable GetFromPool(string key)
        {
            if (!_prefabs.ContainsKey(key)) return null;
            IPoolable poolable = _prefabs[key];
            return GetFromPool(poolable);
        }

        /// <summary>
        /// Adds a new key to the pool dictionary if it doesn't already exist.
        /// </summary>
        /// <param name="key">The key to add.</param>
        private void AddNewKey(string key)
        {
            _pools[key] = new Queue<IPoolable>();
        }
    }





}

/// <summary>
/// Contains extension methods for IPoolable objects to interact with the ObjectPoolManager.
/// </summary>
public static class ObjectPoolManagerExtensions
{
    /// <summary>
    /// Extension method to add a poolable object to the pool.
    /// </summary>
    /// <param name="poolable">The poolable object to add.</param>
    public static void AddToPool(this IPoolable poolable)
    {
        IPoolable.AddToPool(poolable);
    }

    /// <summary>
    /// Extension method to retrieve a poolable object of a specified type from the pool.
    /// </summary>
    /// <param name="poolable">The poolable object to retrieve.</param>
    /// <typeparam name="T">The type of the poolable object.</typeparam>
    /// <returns>The retrieved object.</returns>
    public static T GetFromPool<T>(this T poolable) where T : IPoolable
    {
        return IPoolable.GetFromPool(poolable);
    }

    /// <summary>
    /// Extension method to retrieve a poolable object from the pool using a specified key.
    /// </summary>
    /// <param name="key">The key associated with the object in the pool.</param>
    /// <typeparam name="T">The type of the poolable object.</typeparam>
    /// <returns>The retrieved object.</returns>
    public static T GetFromPool<T>(this string key) where T : IPoolable
    {
        return IPoolable.GetFromPool<T>(key);
    }

    /// <summary>
    /// Extension method to add a prefab to the pool.
    /// </summary>
    /// <param name="poolable">The poolable object that serves as the prefab.</param>
    /// <returns>True if the prefab was added successfully, false if it already exists in the pool.</returns>
    public static bool AddPrefabToPool(IPoolable poolable)
    {
        return IPoolable.AddPrefabToPool(poolable);
    }
}