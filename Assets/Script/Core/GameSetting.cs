using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class GameSetting
{
    public static int playerCount { get; set; }

    private static Dictionary<string, string> SettingInfo = new Dictionary<string, string>();
    public static void SetSetting(string setting)
    {
        string[] info = setting.Split(Environment.NewLine.ToCharArray());
        
        for (int i = 0; i < info.Length; i++)
        {
            string[] set = info[i].Split(':');
            if(set.Length > 1)
            {
                string key = set[0];
                if (!SettingInfo.ContainsKey(key))
                {
                    SettingInfo.Add(key, set[1]);
                }
            }
        }
    }

    private static string GetSettingValue(string key)
    {
        string value;
        if (SettingInfo.TryGetValue(key, out value))
        {
            return value;
        }
        return null;
    }

    public static string GetPrefabPath()
    {
        return GetSettingValue("PrefabPath");
    }

    public static string GetSpritePath()
    {
        return GetSettingValue("SpritePath");
    }

    public static int GetDefaultLifeCount()
    {
        int result;
        if (int.TryParse(GetSettingValue("DefaultLifeCount"), out result))
        {
            return result;
        }
        return 1;
    }

    public static int GetDefaultWeapon()
    {
        int result;
        if (int.TryParse(GetSettingValue("DefaultWeapon"), out result))
        {
            return result;
        }
        return 1;
    }

    public static KeyCode GetUpKeyCode(int player_index)
    {
        string key = GetSettingValue("Player" + player_index.ToString() + "Up");
        KeyCode result = (KeyCode)Enum.Parse(typeof(KeyCode), key, true);
        return result;
    }

    public static KeyCode GetLeftKeyCode(int player_index)
    {
        string key = GetSettingValue("Player" + player_index.ToString() + "Left");
        KeyCode result = (KeyCode)Enum.Parse(typeof(KeyCode), key, true);
        return result;
    }

    public static KeyCode GetDownKeyCode(int player_index)
    {
        string key = GetSettingValue("Player" + player_index.ToString() + "Down");
        KeyCode result = (KeyCode)Enum.Parse(typeof(KeyCode), key, true);
        return result;
    }

    public static KeyCode GetRightKeyCode(int player_index)
    {
        string key = GetSettingValue("Player" + player_index.ToString() + "Right");
        KeyCode result = (KeyCode)Enum.Parse(typeof(KeyCode), key, true);
        return result;
    }
}
