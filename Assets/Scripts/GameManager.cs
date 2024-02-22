using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{


    public MyObject cubePrefab;
    public MyObject spherePrefab;

    private Queue<MyObject> _cubes = new Queue<MyObject>();
    private Queue<MyObject> _spheres = new Queue<MyObject>();


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    [ContextMenu("GetCubeFromPool")]
    public void GetCubeFromPool()
    {
        MyObject cube = cubePrefab.GetFromPool();
        _cubes.Enqueue(cube);
    }
    [ContextMenu("AddCubeToPool")]
    public void AddCubeToPool()
    {
        if (_cubes.Count > 0)
        {
            MyObject cube = _cubes.Dequeue();
            if(cube != null)
            {
                cube.AddToPool();
            }
            else
            {
                AddCubeToPool();
            }
                
        }
    }


    [ContextMenu("GetSphereFromPool")]
    public void GetSphereFromPool()
    {
        MyObject sphere = spherePrefab.GetFromPool();
        _spheres.Enqueue(sphere);
    }
    [ContextMenu("AddSphereToPool")]
    public void AddSphereToPool()
    {
        if (_spheres.Count > 0)
        {
            MyObject spheres = _spheres.Dequeue();
            if (spheres != null)
            {
                spheres.AddToPool();
            }
            else
            {
                AddSphereToPool();
            }

        }
    }

    [ContextMenu("ChangeScene")]
    public void ChangeScene()
    {
        SceneManager.LoadScene("Test2");
    }



}
