using EditorTools;
using UnityEngine;



public class PoolableObject : MonoBehaviour, IPoolableObject
{

    [field: Header("IPoolableObject")]
    [field: SerializeField, AssignIDButton] public string PrefabID { get; set; }
    public bool Pooled { get; set; }


    public MonoBehaviour MonoBehaviour => this;

}
