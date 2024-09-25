using UnityEngine;

public class MyObject : MonoBehaviour, IPoolableObject
{
    
    public MonoBehaviour MonoBehaviour => this;

    public bool Pooled { get; set; }
    [field: SerializeField] public string PrefabID { get; set; }

    public void OnCreated()
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
