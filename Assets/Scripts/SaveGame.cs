using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public interface ISaveable
{
    string SaveID {get;}
    JsonData SavedData{get;}
    void LoadFromData(JsonData data);
}

public static class Saving{
    private const string ACTIVE_SCENE_KEY = "activeScene";
    private const string SCENES_KEY = "scenes";
    private const string OBJECTS_KEY = "objects";
    private const string SAVEID_KEY = "$saveID";

    public static void SaveGame(string fileName) {
        var result = new JsonData();
        var allSaveableObjects = Object.FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        if (allSaveableObjects.Count() > 0) {
            var savedObjects = new JsonData();
            foreach (var saveableObject in allSaveableObjects) {
                var data = saveableObject.SavedData;
                if (data.IsObject) {
                    data[SAVEID_KEY] = saveableObject.SaveID;
                    savedObjects.Add(data);
                } else {
                    var behaviour = saveableObject as MonoBehaviour;
                    Debug.LogWarningFormat(behaviour, "{0}'s save data is not a dictionary. The object was not saved.", behaviour.name );
                }
            }
        result[OBJECTS_KEY] = savedObjects;
        } else 
        { 
            Debug.LogWarningFormat("The scene did not include any saveable objects."); 
        }
    }
    public static bool LoadGame(string fileName) {
        if (data.ContainsKey(OBJECTS_KEY)) {
            var objects = data[OBJECTS_KEY];
            LoadObjectsAfterSceneLoad = (scene, loadSceneMode) => {
                var allLoadableObjects = Object.FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>().ToDictionary(o => o.SaveID, o => o);
                var objectsCount = objects.Count;
                for (int i = 0; i < objectsCount; i++) {
                    var objectData = objects[i];
                    var saveID = (string)objectData[SAVEID_KEY];
                    if (allLoadableObjects.ContainsKey(saveID)) {
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
public class TransformSaver : MonoBehaviour, Isaveable {
    private const string LOCAL_POSITION_KEY = "localPosition";
    private const string LOCAL_ROTATION_KEY = "localRotation";
    private const string LOCAL_SCALE_KEY = "localScale";
    private JsonData SerializeValue(object obj) { return JsonMapper.ToObject(JsonUtility.ToJson(obj)); }
    public override JsonData SavedData {
        get {
            var result = new JsonData();
            result[LOCAL_POSITION_KEY] = SerializeValue(transform.localPosition);
            result[LOCAL_ROTATION_KEY] = SerializeValue(transform.localRotation);
            result[LOCAL_SCALE_KEY] = SerializeValue(transform.localScale);
            return result;
        }
    }
    public override void LoadFromData(JsonData data) {
        if (data.ContainsKey(LOCAL_POSITION_KEY)) { transform.localPosition = DeserializeValue<Vector3>(data[LOCAL_POSITION_KEY]); }
        if (data.ContainsKey(LOCAL_ROTATION_KEY)) { transform.localRotation = DeserializeValue<Quaternion>(data[LOCAL_ROTATION_KEY]); }
        if (data.ContainsKey(LOCAL_SCALE_KEY)) { transform.localScale = DeserializeValue<Vector3>(data[LOCAL_SCALE_KEY]); }
    }
}