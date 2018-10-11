using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//获取资源时传入的委托
public delegate void DelegateGetPrefabResources<T>(T xx) where T : UnityEngine.Object;

public class ResourcesManager : MonoBehaviour
{
    //单例模式 资源管理
    public static ResourcesManager gResourcesManagerInstantiate;
    private void Awake()
    {
        if (gResourcesManagerInstantiate == null)
        {
            gResourcesManagerInstantiate = this;
        }
        else
        {
            Debug.LogError("重复实例资源管理类");
        }
    }

    // 资源对象
    private struct ResourceObject<T> where T : UnityEngine.Object
    {
        public Type type;
        public string name;
        public Stack<DelegateGetPrefabResources<T>> events;
        public bool isDone;
        public T source;
        public Stack<T> idleInstantiates;

        public ResourceObject(string name)
        {
            this.type = typeof(T);
            this.name = name;
            events = new Stack<DelegateGetPrefabResources<T>>();
            isDone = false;
            source = null;
            idleInstantiates = new Stack<T>();
        }

        public void AddCallBack(DelegateGetPrefabResources<T> callback)
        {
            events.Push(callback);
        }

        public void FireCallBack()
        {
            while(events.Count > 0)
            {
                DelegateGetPrefabResources<T> callback = events.Pop();
                callback(PopObject());
            }
        }

        public T PopObject()
        {
            T result = null;
            if (idleInstantiates.Count > 0)
            {
                result = idleInstantiates.Pop();
            }
            else
            {
                result = Instantiate<T>(source);
            }
            return result;
        }

        public void PushObject(T obj)
        {
            idleInstantiates.Push(obj);
        }

        public void Clear()
        {
            while (idleInstantiates.Count > 0)
            {
                T obj = idleInstantiates.Pop();
                Destroy(obj);
            }
        }
    }

    private class ResourceFactory<T> where T : UnityEngine.Object
    {
        public ResourceFactory()
        {

        }

        private Dictionary<string, ResourceObject<T>> gameObjectResources = new Dictionary<string, ResourceObject<T>>();
        public void GetResource(string name, DelegateGetPrefabResources<T> callback)
        {
            ResourceObject<T> resourceObject;
            if (!gameObjectResources.TryGetValue(name, out resourceObject))
            {
                resourceObject = new ResourceObject<T>(name);
                gameObjectResources.Add(name, resourceObject);
                IEnumerator fun = gResourcesManagerInstantiate.AsyncLoadPrefab<T>(name, resourceObject);
                gResourcesManagerInstantiate.StartCoroutine(fun);
            }
            if (resourceObject.isDone)
            {
                callback(resourceObject.source);
            }
            else
            {
                resourceObject.AddCallBack(callback);
            }
        }
    }

    private Dictionary<Type, ResourceFactory<UnityEngine.Object>> resourceFactory = new Dictionary<Type, ResourceFactory<UnityEngine.Object>>();
    public void GetResource<T>(string name, DelegateGetPrefabResources<T> callback) where T : UnityEngine.Object
    {
        Type type = typeof(T);
        ResourceFactory<T> factory;
        ResourceFactory<UnityEngine.Object> getValue;
        if (resourceFactory.TryGetValue(type,out getValue))
        {
            factory = getValue as ResourceFactory<T>;
        }
        else
        {
            factory = new ResourceFactory<T>();
            getValue = factory as ResourceFactory<UnityEngine.Object>;
            resourceFactory.Add(type, getValue);
        }
        if (factory != null)
        {
            factory.GetResource(name, callback);
        }
    }


    private IEnumerator AsyncLoadPrefab<T>(string name, ResourceObject<T> resourceObject) where T : UnityEngine.Object
    {
        string path = "Prefab/" + name;
        ResourceRequest r = Resources.LoadAsync(path);
        while (!r.isDone)
        {
            yield return null;
        }
        resourceObject.source = r.asset as T;
        resourceObject.FireCallBack();
    }
}