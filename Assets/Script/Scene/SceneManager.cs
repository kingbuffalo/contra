using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    /// <summary>
    /// 是否正在加在场景
    /// </summary>
    public bool isOnLoad { get {return loadSceneCount > 0; }}

    /// <summary>
    /// 当前使用中的场景
    /// </summary>
    private BaseScene[] nowScenes;
    /// <summary>
    /// 要加载的场景数量
    /// </summary>
    private int loadSceneCount;
    /// <summary>
    /// 加载完成的场景列表
    /// </summary>
    private List<BaseScene> loadedSceneList;

    //单例模式 场景管理
    public static SceneManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("SceneManager instance already exists");
        }
        loadedSceneList = new List<BaseScene>();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// 加载一个场景
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(SceneName sceneName)
    {
        if (isOnLoad)
        {
            Debug.LogWarning("is on loading scene");
            return;
        }
        loadSceneCount = 1;
        string resourcesName = sceneName.ToString();
        ResourcesManager.Instance.PopResource<GameObject>(resourcesName, OnSceneLoaded);
    }
    
    /// <summary>
    /// 加载多场景
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScenes(SceneName[] sceneName)
    {
        print("加在多个场景");
        if (isOnLoad)
        {
            Debug.LogWarning("is on loading scene");
            return;
        }
        loadSceneCount = sceneName.Length;
        for (int i = 0; i < sceneName.Length; i++)
        {
            string resourcesName = sceneName[i].ToString();
            print("加在场景" + resourcesName.ToString());
            ResourcesManager.Instance.PopResource<GameObject>(resourcesName, OnSceneLoaded);
        }
    }

    private void OnSceneLoaded(GameObject go,string name)
    {
        print("UI加载完成" + go.ToString());
        if (loadSceneCount == 0)
        {
            Debug.LogError("load scene error1");
            return;
        }
        if (go == null)
        {
            Debug.LogError("scene name [" + name + "] is not exist");
            loadedSceneList.Add(null);
        }
        else
        {
            BaseScene baseScene = go.GetComponent<BaseScene>();
            loadedSceneList.Add(baseScene);
            baseScene.resourcesName = name;
        }
        print(loadedSceneList.Count.ToString() + " " + loadSceneCount.ToString());
        if(loadedSceneList.Count == loadSceneCount)
        {
            OnAllSceneLoaded();
        }
    }

    private void OnAllSceneLoaded()
    {
        if (nowScenes != null)
        {
            for (int i = 0; i < nowScenes.Length; i++)
            {
                BaseScene baseScene = nowScenes[i];
                ResourcesManager.Instance.PushResource(baseScene.resourcesName, baseScene.gameObject);
            }
        }

        for (int i = 0; i < loadSceneCount; i++)
        {
            BaseScene baseScene = loadedSceneList[i];
            if (baseScene != null)
            {
                baseScene.Init();
            }
        }
        nowScenes = loadedSceneList.ToArray();
        loadSceneCount = 0;
        loadedSceneList.Clear();
    }
}
