using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
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

    void Start () {
        ResourcesManager.gResourcesManagerInstantiate.GetResource<GameObject>("logic", OnUiLogicLoaded);
    }

    public void OnUiLogicLoaded(GameObject go)
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("UI");
        go.transform.SetParent(canvas.transform, false);
    }
	
	void Update () {
		
	}

}
