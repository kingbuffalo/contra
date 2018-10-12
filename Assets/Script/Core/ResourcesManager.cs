
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//获取资源时传入的委托
public delegate void DelegateGetPrefabResources<T>(T asste,string name) where T : UnityEngine.Object;

public class ResourcesManager : MonoBehaviour
{
    //单例模式 资源管理
    public static ResourcesManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("ResourcesManager instance already exists");
        }
    }

    private void OnDestroy()
    {
        Instance = null;
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
            while (events.Count > 0)
            {
                DelegateGetPrefabResources<T> callback = events.Pop();
                T obj = PopObject();
                callback(obj, name);
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
            if (typeof(T) == typeof(GameObject))
            {
                GameObject go = result as GameObject;
                go.SetActive(true);
            }
            return result;
        }

        public void PushObject(T obj)
        {
            if (resourceType == ResourceType.Instantiate)
            {
                idleInstantiates.Push(obj);
                if (typeof(T) == typeof(GameObject))
                {
                    GameObject go = obj as GameObject;
                    go.SetActive(false);
                    Transform tf = go.transform;
                    tf.SetParent(null);
                }
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

        private Dictionary<string, ResourceObject<T>> gameObjectResources = new Dictionary<string, ResourceObject<T>>();
        public void PopResource(string name, DelegateGetPrefabResources<T> callback)
        {
            ResourceObject<T> resourceObject;
            if (!gameObjectResources.TryGetValue(name, out resourceObject))
            {
                resourceObject = new ResourceObject<T>(name);
                gameObjectResources.Add(name, resourceObject);
                IEnumerator fun = Instance.AsyncLoadPrefab<T>(name, resourceObject);
                Instance.StartCoroutine(fun);
            }
           
            if (callback != null)
            {
                resourceObject.AddCallBack(callback);
                if (resourceObject.isDone)
                {
                    resourceObject.FireCallBack();
                }
            }
        }

        public void PushResource(string name, T obj)
        {
            ResourceObject<T> resourceObject;
            if (gameObjectResources.TryGetValue(name, out resourceObject))
            {
                resourceObject.PushObject(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
    }

    private Dictionary<Type, System.Object> resourceFactory = new Dictionary<Type, System.Object>();
    public void PopResource<T>(string name, DelegateGetPrefabResources<T> callback) where T : UnityEngine.Object
    {
        Type type = typeof(T);
        ResourceFactory<T> factory;
        System.Object getValue;
        if (resourceFactory.TryGetValue(type, out getValue))
        {
            factory = getValue as ResourceFactory<T>;
        }
        else
        {
            factory = new ResourceFactory<T>();
            getValue = factory as System.Object;
            resourceFactory.Add(type, getValue);
        }
        if (factory != null)
        {
            factory.PopResource(name, callback);
        }
    }

    public void PushResource<T>(string name,T obj) where T : UnityEngine.Object
    {
        Type type = typeof(T);
        ResourceFactory<T> factory;
        System.Object getValue;
        if (resourceFactory.TryGetValue(type, out getValue))
        {
            factory = getValue as ResourceFactory<T>;
            factory.PushResource(name, obj);
        }
        else
        {
            Destroy(obj);
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