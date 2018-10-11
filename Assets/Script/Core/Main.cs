using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        TextAsset textAsset = Resources.Load("Txt/setting", typeof(TextAsset)) as TextAsset;
        if (textAsset != null)
        {
            string strSetting = textAsset.text;
            GameSetting.SetSetting(strSetting);
        }
    }

    void Start()
    {
        Init();
    }
    private GameObject root_node;
    private CanvasGroup img_canvas;
    private CanvasGroup btn_canvas;
    void Init()
    {
        ResourcesManager.gResourcesManagerInstantiate.GetResource<GameObject>("logic", OnUiLogicLoaded);
    }

    void OnUiLogicLoaded(GameObject go, string name)
    {
        root_node = go;
        if (go != null)
        {
            GameObject parent = GameObject.FindGameObjectWithTag("UI");
            if (parent != null)
            {
                go.transform.SetParent(parent.transform, false);
                ResourcesManager.gResourcesManagerInstantiate.GetResource<Sprite>("png22", OnUiSpriteLoaded);
            }
        }
    }

    void OnUiSpriteLoaded(Sprite sprite, string name)
    {
        if (root_node != null)
        {
            Transform img_log = root_node.transform.Find("img_log");
            if (img_log != null)
            {
                img_log.GetComponent<Image>().sprite = sprite;
                img_canvas = img_log.GetComponent<CanvasGroup>();
                img_canvas.alpha = 0;
            }
            Transform btn_layout = root_node.transform.Find("layout");
            if (btn_layout != null)
            {
                btn_canvas = btn_layout.GetComponent<CanvasGroup>();
                btn_canvas.alpha = 0;
            }
        }
    }

    private void Update()
    {
        if ((img_canvas != null) && (img_canvas.alpha < 1))
        {
            img_canvas.alpha += Time.deltaTime;
        }
        else if ((btn_canvas != null) && (btn_canvas.alpha < 1))
        {
            btn_canvas.alpha = 1;
        }
    }
}