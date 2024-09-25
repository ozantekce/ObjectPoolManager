using UnityEngine;

public interface IPoolableObject
{
    public string PrefabID { get; set; }
    public MonoBehaviour MonoBehaviour { get; }
    public bool Pooled { get; set; }

    public void AddToPool()
    {
        ObjectPoolManager.Instance.AddToPool(this);
    }

    public virtual void OnCreated()
    {
    }

    public virtual void OnAddToPool()
    {
    }

    public virtual void OnGetFromPool()
    {
    }

    #region Static Methods
    public static void AddToPool(IPoolableObject poolable) => ObjectPoolManager.Instance.AddToPool(poolable);

    public static T GetFromPool<T>(T poolable) where T : IPoolableObject => (T) ObjectPoolManager.Instance.GetFromPool(poolable);

    public static T GetFromPool<T>(string key) where T : IPoolableObject => (T)ObjectPoolManager.Instance.GetFromPool(key);

    public static bool RegisterPrefab(IPoolableObject poolable) => ObjectPoolManager.Instance.RegisterPrefab(poolable);

    public static void ResetPoolManager() => ObjectPoolManager.Instance.ResetPoolManager();
    #endregion





}

