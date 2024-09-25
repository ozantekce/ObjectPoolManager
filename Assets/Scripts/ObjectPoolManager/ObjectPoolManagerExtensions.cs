public static class ObjectPoolManagerExtensions
{
    public static void AddToPool(this IPoolableObject poolable) => ObjectPoolManager.Instance.AddToPool(poolable);

    public static T GetFromPool<T>(this T poolable) where T : IPoolableObject => (T) ObjectPoolManager.Instance.GetFromPool(poolable);

    public static T GetFromPool<T>(this string key) where T : IPoolableObject => (T)ObjectPoolManager.Instance.GetFromPool(key);

    public static bool RegisterPrefab(this IPoolableObject prefab) => ObjectPoolManager.Instance.RegisterPrefab(prefab);
}