using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScene : MonoBehaviour {
    public SceneMode sceneMode;
    public SceneName sceneName;
    public string resourcesName { get; set; }

    public virtual void Init()
    {
        GameObject parent = GameObject.FindGameObjectWithTag("UIRoot");
        if (parent != null)
        {
            gameObject.transform.SetParent(parent.transform, false);
        }
    }
}