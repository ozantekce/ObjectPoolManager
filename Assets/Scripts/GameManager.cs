using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    public MyObject cube;
    public MyObject sphere;


    public Stack<Poolable> cubes;
    public Stack<Poolable> spheres;

    private void Awake()
    {
        cubes = new Stack<Poolable>();
        spheres = new Stack<Poolable>();

    }




    [ContextMenu("GetCubeFromPool")]
    public void GetCubeFromPool()
    {
        cubes.Push(ObjectPoolManager.Instance.GetFromPool(cube));
    }
    [ContextMenu("AddCubeToPool")]
    public void AddCubeToPool()
    {
        if (cubes.Count > 0)
            ObjectPoolManager.Instance.AddToPool(cubes.Pop());
    }

    [ContextMenu("GetSphereFromPool")]
    public void GetSphereFromPool()
    {
        spheres.Push(ObjectPoolManager.Instance.GetFromPool(sphere));
    }
    [ContextMenu("AddSphereToPool")]
    public void AddSphereToPool()
    {
        if (spheres.Count > 0)
            ObjectPoolManager.Instance.AddToPool(spheres.Pop());
    }


}
