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
        cubes.Push(Poolable.GetFromPool(cube));
    }
    [ContextMenu("AddCubeToPool")]
    public void AddCubeToPool()
    {
        if (cubes.Count > 0)
            Poolable.AddToPool(cubes.Pop());
    }

    [ContextMenu("GetSphereFromPool")]
    public void GetSphereFromPool()
    {
        spheres.Push(Poolable.GetFromPool(sphere));
    }
    [ContextMenu("AddSphereToPool")]
    public void AddSphereToPool()
    {
        if (spheres.Count > 0)
            Poolable.AddToPool(spheres.Pop());
    }


}
