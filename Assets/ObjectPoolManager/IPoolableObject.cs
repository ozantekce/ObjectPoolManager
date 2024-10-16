using UnityEngine;

public interface IPoolableObject
{
    public string PrefabID { get; set; }
    public MonoBehaviour MonoBehaviour { get; }
    public bool Pooled { get; set; }

    public void AddToPool()
    {
        if (ObjectPoolManager.Instance != null)
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
    public static void AddToPool(IPoolableObject poolable)
    {
        if (ObjectPoolManager.Instance != null)
            ObjectPoolManager.Instance.AddToPool(poolable);
    }

    public static T GetFromPool<T>(T poolable) where T : IPoolableObject
    {
        if (ObjectPoolManager.Instance != null)
            return (T)ObjectPoolManager.Instance.GetFromPool(poolable);
        return default;
    }

    public static T GetFromPool<T>(string key) where T : IPoolableObject
    {
        if (ObjectPoolManager.Instance != null)
            return (T)ObjectPoolManager.Instance.GetFromPool(key);
        return default;
    }

    public static bool RegisterPrefab(IPoolableObject poolable)
    {
        if (ObjectPoolManager.Instance != null)
            return ObjectPoolManager.Instance.RegisterPrefab(poolable);
        return false;
    }

    public static void ResetPoolManager()
    {
        if (ObjectPoolManager.Instance != null)
            ObjectPoolManager.Instance.ResetPoolManager();
    }
    #endregion




}

