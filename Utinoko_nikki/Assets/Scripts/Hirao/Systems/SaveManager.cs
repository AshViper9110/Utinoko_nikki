using System;
using System.IO;
using AquaGarden.Data;
using UnityEngine;

namespace AquaGarden.Systems
{
    public sealed class SaveManager : MonoBehaviour
    {
        [SerializeField] private PetRegistry petRegistry;
        [SerializeField] private HabitatManager habitatManager;
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private TimeManager timeManager;
        [SerializeField] private GameSettingsData settings = new GameSettingsData();

        private string SavePath => Path.Combine(Application.persistentDataPath, "aquagarden_save.json");
        public GameSettingsData Settings => settings;
        public string LastError { get; private set; }

        public bool SaveNow()
        {
            try
            {
                SaveData data = new SaveData
                {
                    currentHabitatId = habitatManager != null
                        ? habitatManager.CurrentHabitatId : "Room",
                    pets = petRegistry != null
                        ? new System.Collections.Generic.List<PetInstanceData>(petRegistry.Pets)
                        : new System.Collections.Generic.List<PetInstanceData>(),
                    inventory = inventoryManager != null
                        ? inventoryManager.Data : new InventoryData(),
                    settings = settings,
                    totalGameSeconds = timeManager != null ? timeManager.TotalGameSeconds : 0d
                };
                data.Normalize();

                string json = JsonUtility.ToJson(data, true);
                string tempPath = SavePath + ".tmp";
                string backupPath = SavePath + ".bak";
                Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
                File.WriteAllText(tempPath, json);

                // Keep the previous valid save as a backup before replacing it.
                if (File.Exists(SavePath))
                {
                    if (File.Exists(backupPath)) File.Delete(backupPath);
                    File.Copy(SavePath, backupPath);
                    File.Delete(SavePath);
                }
                File.Move(tempPath, SavePath);
                LastError = null;
                Debug.Log("Aqua Garden saved: " + SavePath, this);
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Debug.LogError("Save failed: " + ex, this);
                return false;
            }
        }

        public bool LoadNow()
        {
            string sourcePath = File.Exists(SavePath) ? SavePath : SavePath + ".bak";
            if (!File.Exists(sourcePath))
            {
                LastError = "No save file exists.";
                return false;
            }

            try
            {
                SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(sourcePath));
                if (data == null) throw new InvalidDataException("Save JSON was empty or invalid.");
                if (data.schemaVersion > SaveData.CurrentSchemaVersion)
                    throw new InvalidDataException("Save file was created by a newer version.");

                data.Normalize();
                if (petRegistry != null) petRegistry.ReplaceAll(data.pets);
                if (inventoryManager != null) inventoryManager.Restore(data.inventory);
                if (habitatManager != null) habitatManager.RestoreCurrent(data.currentHabitatId);
                if (timeManager != null) timeManager.Restore(data.totalGameSeconds);
                settings = data.settings;

                LastError = null;
                Debug.Log("Aqua Garden loaded from: " + sourcePath, this);
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Debug.LogError("Load failed: " + ex, this);
                return false;
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) SaveNow();
        }

        private void OnApplicationQuit()
        {
            SaveNow();
        }
    }
}
