using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    public MyObject cubePrefab;
    public MyObject spherePrefab;

    private Queue<MyObject> _cubes = new Queue<MyObject>();
    private Queue<MyObject> _spheres = new Queue<MyObject>();

    private void Awake()
    {
        Poolable.AddPrefabToPool(cubePrefab);
        Poolable.AddPrefabToPool(spherePrefab);
    }


    [ContextMenu("GetCubeFromPool")]
    public void GetCubeFromPool()
    {
        MyObject cube = Poolable.GetFromPool<MyObject>("cube");
        _cubes.Enqueue(cube);
    }
    [ContextMenu("AddCubeToPool")]
    public void AddCubeToPool()
    {
        if (_cubes.Count > 0)
            Poolable.AddToPool(_cubes.Dequeue());
    }


    [ContextMenu("GetSphereFromPool")]
    public void GetSphereFromPool()
    {
        MyObject sphere = Poolable.GetFromPool<MyObject>("sphere");
        _spheres.Enqueue(sphere);
    }
    [ContextMenu("AddSphereToPool")]
    public void AddSphereToPool()
    {
        if (_spheres.Count > 0)
            Poolable.AddToPool(_spheres.Dequeue());
    }




}
