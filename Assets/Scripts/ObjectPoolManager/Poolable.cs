using UnityEngine;
using UnityEngine.Events;

public interface Poolable
{

    public string Key { get; set; }
    public MonoBehaviour MonoBehaviour { get;}
    public bool Pooled { get; set; }

    public void SendToPool()
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


}
