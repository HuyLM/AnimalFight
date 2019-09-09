using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class IPlayerPrefs {

    public static readonly string DATA_GAME_FILE = "DataGame";

    private static Data gameData;
    private static Data GameData
    {
        get
        {
            if (gameData == null)
            {
                LoadDisk();
            }
            return gameData;
        }
    }

    private static void _FirstSaveData()
    {
        gameData = new Data();
        gameData.data = new Dictionary<string, object>();
        // save to here
        Save();
    }

    public static void LoadDisk()
    {
        try
        {
            gameData = FileData.LoadFile<Data>(DATA_GAME_FILE);
        }
        catch
        {
            _FirstSaveData();
            Debug.LogError("[Data] Player Data got problem :( sorry to create new one.");
        }

        if (gameData == null)
        {
            _FirstSaveData();
        }
    }

    public static void Save()
    {
        try
        {
            if (gameData != null)
                FileData.SaveFile(gameData, DATA_GAME_FILE);
        }
        catch (Exception ex)
        {
            if (gameData != null)
                FileData.SaveFile(gameData, DATA_GAME_FILE);
            Debug.LogError("[Data] Player Data got problem :( sorry to create new one.");
        }
    }

    public static T Get<T>(string key, T defaultValue = default(T))
    {
        if (GameData == null)
            return defaultValue;

        if (HasKey(key))
        {
            return (T)GameData.data[key];
        }

        return defaultValue;
    }

    public static void SetFloat(string key, float value)
    {
        Set(key, value);
    }

    public static float GetFloat(string key, float defaultValue = 0)
    {
        return Get(key, defaultValue);
    }

    public static void SetInt(string key, int value)
    {
        Set(key, value);
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return Get(key, defaultValue);
    }

    public static void SetString(string key, string value)
    {
        Set(key, value);
    }

    public static string GetString(string key, string defaultValue = "")
    {
        return Get(key, defaultValue);
    }

    public static void SetBool(string key, bool value)
    {
        Set(key, value);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        return Get(key, defaultValue);
    }

    public static void DeleteKey(string key)
    {
        if (GameData == null)
            return;

        if (HasKey(key))
        {
            GameData.data.Remove(key);
        }
    }

    public static void DeleteAll()
    {
        GameData.data.Clear();
        Save();
    }

    public static bool HasKey(string key)
    {
        Dictionary<string, object> dictionary = GameData.data;

        if (dictionary == null || key == null)
        {
            return false;
        }

        if (dictionary != null)
        {
            if (dictionary.ContainsKey(key))
            {
                return true;
            }
        }

        return false;
    }

    public static void Set(string key, object data)
    {
        if (key == null) return;

        if (GameData.data != null)
        {
            GameData.data[key] = data;
        }
        else
        {
            GameData.data = new Dictionary<string, object>();
            GameData.data[key] = data;
        }
    }

    [System.Serializable]
    public class Data
    {
        public Dictionary<string, object> data;
    }

    public class FileData
    {

        public static void ConfigSettings()
        {
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        }

        /// <summary>
        /// Return path data in local
        /// </summary>
        public static string DeviceDataPath
        {
            get
            {
                return Application.persistentDataPath + "/";
            }
        }

        // Trung: To remove all save files.
        public static void ClearSaveFile()
        {
            var path = DeviceDataPath;
            var files = Directory.GetFiles(path);
            Debug.Log(DeviceDataPath);
            foreach (var file in files)
            {
                File.Delete(file);
                Debug.Log("deleted " + file);
            }
        }

        /// <summary>
        /// Save data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="playerInfo"></param>
        /// <param name="filename"></param>
        public static void SaveFile<T>(T playerInfo, string filename)
        {
            var path = DeviceDataPath + filename;
            ConfigSettings();
            FileStream file = File.Create(path);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, playerInfo);
            file.Close();
        }
        /// <summary>
        /// Load data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T LoadFile<T>(string filename)
        {
            var path = DeviceDataPath + filename;
            if (File.Exists(path))
            {
                ConfigSettings();
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                T data = (T)bf.Deserialize(file);
                file.Close();
                return data;
            }
            return default(T);
        }

        public static void DeleteFile(string filename)
        {
            var path = DeviceDataPath + filename;
            File.Delete(path);
        }

        public static bool Exists(string fileName)
        {
            var path = DeviceDataPath + fileName;
            return File.Exists(path);
        }

        public static List<string> GetNamesFile()
        {
            var path = DeviceDataPath;
            var files = Directory.GetFiles(path);

            List<string> names = new List<string>();

            foreach (var file in files)
            {
                int start = path.Length;
                int end = file.Length;
                string name = file.Substring(start, end - start);
                Debug.Log("File Name: " + name);
                names.Add(name);
            }

            return names;
        }
    }
}

