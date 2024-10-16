using System.Collections.Generic;

public static class ObjectPoolManagerExtensions
{

    private static List<IPoolableObject> _cachedList = new List<IPoolableObject>();
    public static void AddToPool(this IPoolableObject poolable)
    {
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.AddToPool(poolable);
        }
    }

    public static T GetFromPool<T>(this T poolable) where T : IPoolableObject
    {
        if (ObjectPoolManager.Instance != null)
        {
            return (T)ObjectPoolManager.Instance.GetFromPool(poolable);
        }
        return default;
    }

    public static T GetFromPool<T>(this string key) where T : IPoolableObject
    {
        if (ObjectPoolManager.Instance != null)
        {
            return (T)ObjectPoolManager.Instance.GetFromPool(key);
        }

        return default;
    }

    public static bool RegisterPrefab(this IPoolableObject prefab)
    {
        if (ObjectPoolManager.Instance != null)
        {
            return ObjectPoolManager.Instance.RegisterPrefab(prefab);
        }
        return false;
    }



    public static void AddToPool<TPoolable>(this ICollection<TPoolable> poolables) where TPoolable : IPoolableObject
    {
        if (ObjectPoolManager.Instance != null && poolables != null && poolables.Count != 0)
        {
            foreach (var poolable in poolables)
            {
                if (poolable != null)
                    _cachedList.Add(poolable);
            }

            foreach (var poolable in _cachedList)
            {
                if (poolable != null && poolable.MonoBehaviour != null && !poolable.Pooled)
                    ObjectPoolManager.Instance.AddToPool(poolable);
            }
            _cachedList.Clear();
        }
    }

}