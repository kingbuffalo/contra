using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public static Main Instance;
    public SpriteType spriteType;
    private void Awake()
    {
        spriteType = SpriteType.shengjianjiyuan;
        DontDestroyOnLoad(gameObject);
        TextAsset textAsset = Resources.Load("Txt/setting", typeof(TextAsset)) as TextAsset;
        if (textAsset != null)
        {
            string strSetting = textAsset.text;
            GameSetting.SetSetting(strSetting);
        }
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Main instance already exists");
        }
        if (ResourcesManager.Instance == null)
        {
            gameObject.AddComponent<ResourcesManager>();
        }
        if (SceneManager.Instance == null)
        {
            gameObject.AddComponent<SceneManager>();
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        SceneManager.Instance.LoadScene(SceneName.logic);
    }
}