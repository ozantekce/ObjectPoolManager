# Unity Object Pooling System

Welcome to a flexible way to manage object pooling in Unity. This system can handle any type of object as long as it implements the Poolable interface.

## Key Features

- **Flexible Design**: Able to pool any type of object that implements the `Poolable` interface.
- **Singleton Object Pool Manager**: Central management of all pooling operations.
- **Poolable Interface**: Essential interface that objects must implement to be managed by the pooling system.
- **Performance Enhancement**: Efficient recycling of GameObjects to reduce the load of Instantiate and Destroy calls.
- **User-Friendly API**: Easy to use and extend.

## Detailed Overview

### Poolable Interface
Objects that need to be pooled should implement the Poolable interface. This allows the system to manage the object's life cycle.

Key methods in the `Poolable` interface include:

- **`AddToPool()`:** A `Poolable` object uses this to add itself to the pool, then sets `Pooled` to true.
- **`OnCreate()`:** Overridable method that runs when a new object is created because the pool is empty.
- **`OnAddToPool()`:** Overridable method that runs when an object is returned to the pool.
- **`OnGetFromPool()`:** Overridable method that runs when an object is fetched from the pool.


### ObjectPoolManager
The `ObjectPoolManager` is a singleton class that manages all pooled objects. It uses a Dictionary to store different queues of `Poolable` objects, each uniquely indexed by a string key. 

This system creates new objects when the pool runs out. If an object or its prefab has been previously added to the pool, the system can create a new one when no objects are left in the pool.

## How to Use

Interact with the `ObjectPoolManager` via static methods provided within the `Poolable` interface:

- **`AddToPool(Poolable poolable)`:** Adds a `Poolable` object to the pool.
- **`GetFromPool<T>(T poolable)`:** Retrieves a `Poolable` object of a specific type from the pool, creating a new one if necessary.
- **`GetFromPool<T>(string key)`:** Retrieves a `Poolable` object using its unique key.
- **`AddPrefabToPool(Poolable poolable)`:** Adds a `Poolable` object's prefab to the pool, enabling the creation of new instances when the pool is empty.
- **`Reset()`:** Clears all objects from the pool.

To use this system:

1. Implement the `Poolable` interface in the objects you wish to pool.
2. Assign a unique key to each object.
3. Use the provided static methods to manage your objects.

## Note

Ensure that your objects (or their prefabs) are added to the pool before attempting to retrieve them. This helps the system to return an object on request, improving performance by reusing existing objects when possible.

This pooling system is useful for Unity projects that create and destroy a lot of objects. You can use and extend this system for your own projects. Contributions are welcome.

**Disclaimer:** This code is primarily for educational purposes and should be thoroughly tested before being used in production environments.
