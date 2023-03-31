using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;

/// <summary>
/// ConfigManager is used to read config files for the game that do not change at runtime.<br/>
/// If you want to save and load data at runtime, use SaveDataManager instead.
/// </summary>
public class ConfigManager : MonoBehaviour
{
    Dictionary<string, string> loadedConfigs = new Dictionary<string, string>();

    static ConfigManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        //enforce singleton
        if (instance == null) instance = this;
        else Destroy(this.gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private bool LoadConfig<T>(string key) where T : Savable
    {
        string path = Application.streamingAssetsPath + "/Configs/" + key + ".json";

        //check that config file exists
        if (!File.Exists(path))
        {
            Debug.LogError("Error in ConfigManager: Config file not found at " + path);
            return false;
        }

        //load config text into string
        string tempConfigValue = File.ReadAllText(path, System.Text.Encoding.UTF8);


        //check if config matches savable type
        T temp = JsonUtility.FromJson<T>(tempConfigValue);
        if (temp.key != key && !temp.Equals(default(T)))
        {
            Debug.LogError("Error in ConfigManager: Loaded config from " + path + " is not formatted as type " + typeof(T).Name);
            return false;
        }

        //add sanitized config string to loadedConfigs and return
        loadedConfigs.Add(key, tempConfigValue);
        return true;
    }

    /// <summary>
    /// Returns the data entry from the config file in Assets/Configs/{key}.JSON as type T. Note that each config file should contain
    /// exactly one JSON object, and that object should be of type T. <br/><br/>Will throw errors if the config file is not found, the config key
    /// is not set correctly, or the loaded object has entirely default values. A call that throws errors will return a default instance of T.
    /// </summary>
    /// <typeparam name="T">The type of data container returned from this method. T must extend the Savable type</typeparam>
    /// <param name="key">The filename/key of the config containing the requested data</param>
    /// <returns>An object of type T containing all available data from the config file given by key</returns>
    public static T GetData<T> (string key) where T : Savable
    {
        //load config from file if necessary
        if (!instance.loadedConfigs.ContainsKey(key)) {
            Debug.Log("Could not find loaded config with key " + key + ". Attempting to load from file...");
            if (!instance.LoadConfig<T>(key)) return default(T);
        }

        string cfg = instance.loadedConfigs[key];

        return JsonUtility.FromJson<T>(cfg);
    }
}