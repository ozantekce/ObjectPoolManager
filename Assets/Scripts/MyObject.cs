using UnityEngine;

public class MyObject : MonoBehaviour, IPoolable
{

    public string poolkey;

    public MonoBehaviour MonoBehaviour => this;

    private bool _pooled = false;
    public bool Pooled { get => _pooled; set => _pooled = value; }
    public string PrefabID { get => poolkey; set { } }

    public void OnCreate()
    {
        Debug.Log("Create");
        float x = Random.Range(-8f, +8f);
        float y = Random.Range(-8f, +8f);
        float z = Random.Range(-8f, +8f);
        transform.position = new Vector3(x, y, z);
    }
    public void OnAddToPool()
    {
        Debug.Log("Add");
    }
    public void OnGetFromPool()
    {
        Debug.Log("Get");
    }



}
