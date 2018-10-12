using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicScene : BaseScene
{
    private Image img_logo = null;
    private CanvasGroup canvas_logo = null;
    private CanvasGroup canvas_layout = null;
    private Sprite spriteLogo = null;

    private void Awake()
    {
        // 添加按钮的点击事件
        Transform tfOne = transform.Find("layout/btn_one_player");
        if (tfOne != null)
        {
            Button btnOne = tfOne.GetComponent<Button>();
            if (btnOne != null)
            {
                btnOne.onClick.AddListener(delegate { OnClickStart(1); });
            }
            Text textOne = tfOne.GetComponentInChildren<Text>();
            if (textOne != null)
            {
                if(Main.Instance.spriteType == SpriteType.contra)
                {
                    textOne.text = "一 个 玩 家";
                }
                else
                {
                    textOne.text = "x x x x x 1";
                }
            }
        }

        Transform tfTwo = transform.Find("layout/btn_two_player");
        if (tfTwo != null)
        {
            Button btnTwo = tfTwo.GetComponent<Button>();
            if (btnTwo != null)
            {
                btnTwo.onClick.AddListener(delegate { OnClickStart(2); });
            }
            Text textTwo = tfTwo.GetComponentInChildren<Text>();
            if (Main.Instance.spriteType == SpriteType.contra)
            {
                textTwo.text = "两 个 玩 家";
            }
            else
            {
                textTwo.text = "x x x x x 2";
            }
        }

        Transform img_log = transform.Find("img_log");
        if (img_log != null)
        {
            img_logo = img_log.GetComponent<Image>();
            canvas_logo = img_log.GetComponent<CanvasGroup>();
        }
        Transform btn_layout = transform.Find("layout");
        if (btn_layout != null)
        {
            canvas_layout = btn_layout.GetComponent<CanvasGroup>();
        }
    }

    public override void Init()
    {
        base.Init();
        if (spriteLogo == null)
        {
            if (Main.Instance.spriteType == SpriteType.contra)
            {
                ResourcesManager.Instance.PopResource<Sprite>("png22", OnUiSpriteLoaded);
            }
            else
            {
                ResourcesManager.Instance.PopResource<Sprite>("CaoZhiLiBao-BiaoTiZhuangShi", OnUiSpriteLoaded);
            }
        }
        else
        {
            OnUiSpriteLoaded(spriteLogo, null);
        }
        if (canvas_logo != null)
        {
            canvas_logo.alpha = 0;
        }
        if (canvas_layout != null)
        {
            canvas_layout.alpha = 0;
            canvas_layout.interactable = false;
        }
    }

    void OnUiSpriteLoaded(Sprite sprite, string name)
    {
        if (spriteLogo == null)
        {
            spriteLogo = sprite;
        }
        if (img_logo != null)
        {
            img_logo.sprite = sprite;
        }
    }

    private void Update()
    {
        if (spriteLogo == null)
        {
            return;
        }
        if ((canvas_logo != null) && (canvas_logo.alpha < 1))
        {
            canvas_logo.alpha += Time.deltaTime;
        }
        else if ((canvas_layout != null) && (canvas_layout.alpha < 1))
        {
            canvas_layout.alpha = 1;
            canvas_layout.interactable = true;
        }
    }

    private void OnClickStart(int playerCount)
    {
        Debug.Log(playerCount);
        //canvas_layout.interactable = false;
        GameSetting.playerCount = playerCount;
        SceneManager.Instance.LoadScenes(new SceneName[] { SceneName.blood, SceneName.level1 });
    }
}
