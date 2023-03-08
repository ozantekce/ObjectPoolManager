using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{

    private static ObjectPoolManager _instance; 

    private Dictionary<string, Queue<Poolable>> _pools = new Dictionary<string, Queue<Poolable>>();

    private Dictionary<string,GameObject> _holders = new Dictionary<string, GameObject>();

    public static ObjectPoolManager Instance { get => _instance; set => _instance = value; }

    private void Awake()
    {
        _instance = this;
    }

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
        
        Queue < Poolable > pool = _pools[key];
        bool isCreated = false;
        if (pool.Count==0)
        {
            isCreated = true;
            GameObject gameObject = Instantiate(poolableInstance.MonoBehaviour.gameObject);
            AddToPool(gameObject.GetComponent<Poolable>());
        }

        poolable = pool.Dequeue();
        if(isCreated)poolable.OnCreate();
        poolable.MonoBehaviour.transform.SetParent(null);
        poolable.MonoBehaviour.gameObject.SetActive(true);
        poolable.Pooled = false;

        poolable.OnGetFromPool();

        return poolable;

    }


    private void AddNewKey(string key)
    {
        _pools[key] = new Queue<Poolable>();
        _holders[key] = new GameObject(key.ToUpper()+"S");
    }


}
