using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Manager
{
    #if UNITY_EDITOR

    static class SoundManagerUISettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Managers/Sound", SettingsScope.Project, new []{ "Scriptable", "Settings", "Manager", "Sound" })
            {
                guiHandler = (searchContext) =>
                {
                    // 설정 창에 표시할 UI
                    SoundManagerSettingsProviderHelper.IsDebug = EditorGUILayout.Toggle("Is Debug", SoundManagerSettingsProviderHelper.IsDebug); 
                    EditorGUILayout.LabelField("Sound Manager Data", EditorStyles.boldLabel);
                    var setting = SoundManagerSettingsProviderHelper.setting = (SoundManagerSetting)EditorGUILayout.ObjectField(
                        $"Setting Data",
                        SoundManagerSettingsProviderHelper.setting,
                        typeof(SoundManagerSetting),
                        false
                    );
                    
                    if (setting != null)
                    {
                        Editor.CreateEditor(setting).OnInspectorGUI();
                    }
                    
                    // setting이 변경되었을 경우 Save() 호출
                    if (GUI.changed)
                    {
                        SoundManagerSettingsProviderHelper.Save();
                    }
                },
            };
        
            return provider;
        }
    }
    #endif

    [Serializable]
    public class SoundManagerSettingJson
    {
        public string SettingPath;
    }
    
    public static class SoundManagerSettingsProviderHelper
    {
        private static bool _IsDebug = true;
        public static SoundManagerSetting setting;

        private static readonly string SettingJsonPath = "Resources/Data/Json/Sound Setting.json";
        private static readonly string DefaultKey = "Managers,Sound";
        private static readonly string DebugKey = "IsDebug";
        private static readonly string SettingKey = nameof(SoundManagerSetting);
        
#if UNITY_EDITOR
        public static bool IsDebug
        {
            get => _IsDebug;
            set
            {
                if (_IsDebug != value)
                    EditorPrefs.SetBool(DefaultKey + DebugKey, value);
                _IsDebug = value;
            }
        }

        static SoundManagerSettingsProviderHelper()
        {
            IsDebug = EditorPrefs.GetBool(DefaultKey + DebugKey);
            if (!Directory.Exists("Assets/Resources/Data/Json/Sound"))
                Directory.CreateDirectory("Assets/Resources/Data/Json/Sound");
            AssetDatabase.Refresh();
            Load();
        }

        public static void Save()
        {
            if(setting != null)
            {
                string path = AssetDatabase.GetAssetPath(setting);
                SoundManagerSettingJson data = new();
                data.SettingPath = path;
                string json = JsonUtility.ToJson(data, true);
                
                File.WriteAllText(Path.Combine(Application.dataPath, SettingJsonPath), json);
                AssetDatabase.Refresh();
                
                EditorPrefs.SetString(DefaultKey + SettingKey, path);
            }
        }

        public static void Load()
        {
            if (EditorPrefs.HasKey(DefaultKey + SettingKey))
            {
                string settingPath = EditorPrefs.GetString(DefaultKey + SettingKey, string.Empty);
                setting = AssetDatabase.LoadAssetAtPath<SoundManagerSetting>(settingPath);
                Debug.Assert(setting != null, "해당 경로에 Sound Manager Setting 데이터가 존재하지 않습니다.");
            }
            else
            {
                var path = GetDataPath();
                if (path != string.Empty)
                {
                    setting = Resources.Load<SoundManagerSetting>(path);
                }
            }
        }
#else
        static SoundManagerSettingsProviderHelper()
        {
            Load();
        }

        public static void Load()
        {
            var settingTextFile = Resources.Load<TextAsset>(SettingJsonPath.Replace("Resources/", "").Replace(".json",""));
            if (settingTextFile != null)
            {
                string json = settingTextFile.text;
                var data = JsonUtility.FromJson<SoundManagerSettingJson>(json);
                var path = data.SettingPath;
                path = path.Replace("Assets/", "");
                path = path.Replace("Resources/", "");
                path = path.Replace(".asset", "");
                setting = Resources.Load<SoundManagerSetting>(path);
            }
        }
#endif

        public static string GetDataPath()
        {
            var settingTextFile = Resources.Load<TextAsset>(SettingJsonPath.Replace("Resources/", "").Replace(".json",""));
            if (settingTextFile != null)
            {
                string json = settingTextFile.text;
                var data = JsonUtility.FromJson<SoundManagerSettingJson>(json);
                var path = data.SettingPath;
                path = path.Replace("Assets/", "");
                path = path.Replace("Resources/", "");
                path = path.Replace(".asset", "");
                return path;
            }

            Debug.LogError($"{SettingJsonPath}에 Sound Setting Data가 존재 하지 않습니다.");
            return "";
        }
    }
}