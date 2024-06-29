using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string playerData;
    public SceneInfo sceneInfo;
    public List<GridData> inventory;

    public SaveData()
    {
        inventory = InventoryManager.instance.grids.list;
    }
} 

public class SaveManager : Singleton<SaveManager>
{
    public static SaveData data;

    [SerializeField]
    private FighterData playerDefaultData;
    [SerializeField]
    private string fileName;
    private static string filePath;

    private static bool isLoad;

    public static bool ExitstsSaveData() => File.Exists(filePath);

    public static void Save()
    {
        data.playerData = GameManager.instance.player.data.ToJson();
        using (var stream = File.Open(filePath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(JsonUtility.ToJson(data));
            }
        }
    }
    public static void Load()
    {
        if (File.Exists(filePath))
        {
            using (var stream = File.Open(filePath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    data = JsonUtility.FromJson<SaveData>(reader.ReadString());
                }
            }
            JsonUtility.FromJsonOverwrite(data.playerData, GameManager.instance.player.data);
            InventoryManager.instance.grids.UpdateList(data.inventory);
            isLoad = true;
        }
        else Save();
    }

    public static bool Reset()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            data = new SaveData();
            InventoryManager.instance.grids.ClearItem();
            GameManager.instance.player.data = Instantiate(instance.playerDefaultData);
            GameManager.TransitionScene(GameManager.instance.startScene);
            return true;
        }
        else return false;
    }

    protected override void Awake()
    {
        base.Awake();
        data = new SaveData();
        filePath = Application.persistentDataPath + "/" + fileName;
        isLoad = false;
    }

    private void Update()
    {
        if (isLoad)
        {
            GameManager.TransitionScene(data.sceneInfo);
            isLoad = false;
        }
    }
}
