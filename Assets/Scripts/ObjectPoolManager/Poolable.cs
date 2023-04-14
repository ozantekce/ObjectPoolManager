using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface Poolable
{

    [SerializeField]
    public static List<Poolable> poolableObjects = new List<Poolable>();
    public string Key { get; set; }
    public MonoBehaviour MonoBehaviour { get;}
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

    static Poolable() {
        Debug.Log("hi");
    }

    public static void AddToPool(Poolable poolable)
    {
        ObjectPoolManager.Instance.AddToPool(poolable);
    }

    public static T GetFromPool<T>(T poolable) where T: Poolable 
    {
        return (T)ObjectPoolManager.Instance.GetFromPool(poolable);
    }
    



    private class ObjectPoolManager
    {

        private static ObjectPoolManager _instance;

        private Dictionary<string, Queue<Poolable>> _pools = new Dictionary<string, Queue<Poolable>>();

        private Dictionary<string, GameObject> _holders = new Dictionary<string, GameObject>();

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

        private ObjectPoolManager() { }

        public void AddToPool(Poolable poolable)
        {

            string key = poolable.Key;

            if (!_pools.ContainsKey(key))
                AddNewKey(key);

            poolable.MonoBehaviour.transform.SetParent(_holders[key].transform);
            poolable.MonoBehaviour.gameObject.SetActive(false);
            _pools[key].Enqueue(poolable);

            poolable.Pooled = true;
            poolable.OnAddToPool();

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
                AddToPool(gameObject.GetComponent<Poolable>());
            }

            poolable = pool.Dequeue();
            if (isCreated) poolable.OnCreate();
            poolable.MonoBehaviour.transform.SetParent(null);
            poolable.MonoBehaviour.gameObject.SetActive(true);
            poolable.Pooled = false;

            poolable.OnGetFromPool();

            return poolable;

        }

        private void AddNewKey(string key)
        {
            _pools[key] = new Queue<Poolable>();
            _holders[key] = new GameObject(key.ToUpperInvariant() + "S");
        }


    }



}
