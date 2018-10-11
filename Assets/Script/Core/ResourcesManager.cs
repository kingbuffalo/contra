
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//获取资源时传入的委托
public delegate void DelegateGetPrefabResources<T>(T asste,string name) where T : UnityEngine.Object;

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

    private enum ResourceType
    {
        Instantiate,
        Source,
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
        public ResourceType resourceType;

        public ResourceObject(string name)
        {
            this.type = typeof(T);
            this.name = name;
            events = new Stack<DelegateGetPrefabResources<T>>();
            isDone = false;
            source = null;
            idleInstantiates = new Stack<T>();
            resourceType = ResourceType.Instantiate;
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
                callback(PopObject(),name);
            }
        }

        public T PopObject()
        {
            if (resourceType == ResourceType.Source)
            {
                return source;
            }
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
            if (resourceType == ResourceType.Instantiate)
            {
                idleInstantiates.Push(obj);
            }
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
                callback(resourceObject.source, resourceObject.name);
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
        Type type = typeof(T);
        string path;
        ResourceType resourceTyp;
        GetResourcePathAndType(type, name, out path, out resourceTyp);
        resourceObject.resourceType = resourceTyp;
        ResourceRequest r = Resources.LoadAsync<T>(path);
        while (!r.isDone)
        {
            yield return null;
        }
        if((r!=null)&&(r.asset!=null))
        {
            resourceObject.source = r.asset as T;
            resourceObject.FireCallBack();
        }
    }

    private void GetResourcePathAndType(Type type,string name,out string path,out ResourceType resourceType)
    {
        if (type == typeof(GameObject))
        {
            path = "Prefab/" + name;
            resourceType = ResourceType.Instantiate;
        }
        else if (type == typeof(Sprite))
        {
            path = "Sprite/" + name;
            resourceType = ResourceType.Source;
        }
        else
        {
            path = "Prefab/" + name;
            resourceType = ResourceType.Instantiate;
        }
    }


}