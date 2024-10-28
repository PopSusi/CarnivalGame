using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using LitJson;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using System.Linq;

public interface ISaveable
{
    string SaveID {get;}
    JsonData SavedData {get;}
    void LoadFromData(JsonData data);
}

public static class SavingService
{
    private const string ACTIVE_SCENE_KEY = "activeScene";
    private const string SCENES_KEY = "scenes";
    private const string OBJECTS_KEY = "objects";
    private const string SAVEID_KEY = "$saveID";

    public static void SaveGame(string fileName)
    {
        var result = new JsonData();
        var allSaveableObjects = Object.FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        if (allSaveableObjects.Count() > 0)
        {
            var savedObjects = new JsonData();
            foreach (var saveableObject in allSaveableObjects)
            {
                var data = saveableObject.SavedData;
                if (data.IsObject)
                {
                    data[SAVEID_KEY] = saveableObject.SaveID;
                    savedObjects.Add(data);
                }
                else
                {
                    var behaviour = saveableObject as MonoBehaviour;
                    Debug.LogWarningFormat(behaviour, "{0}'s save data is not a dictionary. The object was not saved.", behaviour.name);
                }
            }
            result[OBJECTS_KEY] = savedObjects;
        }
        else { Debug.LogWarningFormat("The scene did not include any saveable objects."); }
        var openScenes = new JsonData();
        var sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            openScenes.Add(scene.name);
        }
        result[SCENES_KEY] = openScenes;
        result[ACTIVE_SCENE_KEY] = SceneManager.GetActiveScene().name;
        var outputPath = Path.Combine(Application.persistentDataPath, fileName);
        var writer = new JsonWriter();
        writer.PrettyPrint = true;
        result.ToJson(writer);
        File.WriteAllText(outputPath, writer.ToString());
        Debug.LogFormat("Wrote saved game to {0}", outputPath);
        result = null;
        System.GC.Collect(); // Garbage Collector
    }
    public static bool LoadGame(string fileName)
    {
        var dataPath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(dataPath) == false) { Debug.LogErrorFormat("No file exists at {0}", dataPath); return false; }
        var text = File.ReadAllText(dataPath);
        var data = JsonMapper.ToObject(text);
        if (data == null || data.IsObject == false)
        {
            Debug.LogErrorFormat("Data at {0} is not a JSON object", dataPath); return false;
        }
        if (!data.ContainsKey("scenes"))
        {
            Debug.LogWarningFormat("Data at {0} does not contain any scenes; notloading any!", dataPath); return false;
        }
        var scenes = data[SCENES_KEY];
        int sceneCount = scenes.Count;
        if (sceneCount == 0)
        {
            Debug.LogWarningFormat("Data at {0} doesn't specify any scenes to load.", dataPath); return false;
        }
        for (int i = 0; i < sceneCount; i++)
        {
            var scene = (string)scenes[i];
            if (i == 0) { SceneManager.LoadScene(scene, LoadSceneMode.Single); }
            else { SceneManager.LoadScene(scene, LoadSceneMode.Additive); }
        }
        if (data.ContainsKey(ACTIVE_SCENE_KEY))
        {
            var activeSceneName = (string)data[ACTIVE_SCENE_KEY];
            var activeScene = SceneManager.GetSceneByName(activeSceneName);
            if (activeScene.IsValid() == false)
            {
                Debug.LogErrorFormat("Data at {0} specifies an active scene that doesn't exist. Stopping loading here.",
                dataPath); return false;
            }
            SceneManager.SetActiveScene(activeScene);
        }
        else
        {
            Debug.LogWarningFormat("Data at {0} does not specify an active scene.", dataPath);
        }
        if (data.ContainsKey(OBJECTS_KEY))
        {
            var objects = data[OBJECTS_KEY];
            LoadObjectsAfterSceneLoad = (scene, loadSceneMode) =>
            {
                var allLoadableObjects = Object.FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>().ToDictionary(o => o.SaveID, o => o);
                var objectsCount = objects.Count;
                for (int i = 0; i < objectsCount; i++)
                {
                    var objectData = objects[i];
                    var saveID = (string)objectData[SAVEID_KEY];
                    if (allLoadableObjects.ContainsKey(saveID))
                    {
                        var loadableObject = allLoadableObjects[saveID];
                        loadableObject.LoadFromData(objectData);
                    }
                }
                SceneManager.sceneLoaded -= LoadObjectsAfterSceneLoad;
                LoadObjectsAfterSceneLoad = null;
                System.GC.Collect();
            };
            SceneManager.sceneLoaded += LoadObjectsAfterSceneLoad;
        }
        return true;
    }
}
public abstract class SaveableBehaviour : MonoBehaviour, ISaveable, ISerializationCallbackReceiver
{
    public string SaveID
    {
        get { return _saveID; }
        set { _saveID = value; }
    }
    private string _saveID;
    public void OnBeforeSerialize()
    {
        if (_saveID == null)
        {
            _saveID = System.Guid.NewGuid().ToString();
        }
    }
    public void OnAfterDeserialize()
    {
    }
    public JsonData SavedData { get; }
    public abstract void LoadFromData(JsonData data);
}
public class TransformSaver : SaveableBehaviour
{
    private const string LOCAL_POSITION_KEY = "localPosition";
    private const string LOCAL_ROTATION_KEY = "localRotation";
    private const string LOCAL_SCALE_KEY = "localScale";
    private JsonData SerializeValue(object obj) { return JsonMapper.ToObject(JsonUtility.ToJson(obj)); }
    new public JsonData SavedData
    {
        get
        {
            var result = new JsonData();
            result[LOCAL_POSITION_KEY] = SerializeValue(transform.localPosition);
            result[LOCAL_ROTATION_KEY] = SerializeValue(transform.localRotation);
            result[LOCAL_SCALE_KEY] = SerializeValue(transform.localScale);
            return result;
        }
    }
    public override void LoadFromData(JsonData data)
    {
        if (data.ContainsKey(LOCAL_POSITION_KEY)) { transform.localPosition = DeserializeValue<Vector3>(data[LOCAL_POSITION_KEY]); }
        if (data.ContainsKey(LOCAL_ROTATION_KEY)) { transform.localRotation = DeserializeValue<Quaternion>(data[LOCAL_ROTATION_KEY]); }
        if (data.ContainsKey(LOCAL_SCALE_KEY)) { transform.localScale = DeserializeValue<Vector3>(data[LOCAL_SCALE_KEY]); }
    }
}