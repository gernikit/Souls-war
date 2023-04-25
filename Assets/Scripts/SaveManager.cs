using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public enum Languages
{
    English,
    Russian
}
public static class SaveManager
{
    [System.Serializable]
    public class MaxLevelsData
    {
        private List<LevelType> levelType;
        private List<int> maxLevelList;
        private const int maxLevel = 30; //for all levels types

        public MaxLevelsData()
        {
            levelType = new List<LevelType>();
            maxLevelList = new List<int>();

            foreach (LevelType el in System.Enum.GetValues(typeof(LevelType)))
            {
                levelType.Add(el);
                maxLevelList.Add(maxLevel);
            }
        }

        public int this[LevelType el]
        {
            get
            {
                return maxLevelList[levelType.IndexOf(el)];
            }
        }
    }

    [System.Serializable]
    public class LevelsData
    {
        private List<LevelType> levelType;
        private List<int> playerLevelNumber;

        public int this[LevelType el]
        {
            get
            {
                return playerLevelNumber[levelType.IndexOf(el)];
            }

            set
            {
                if (levelType == null)
                {
                    levelType = new List<LevelType>();
                    playerLevelNumber = new List<int>();
                }

                if (levelType.Contains(el))
                {
                    //check for max level!!!
                    playerLevelNumber[levelType.IndexOf(el)] = value;
                }
                else
                {
                    levelType.Add(el);
                    playerLevelNumber.Add(value);
                }
            }
        }
    }

    [System.Serializable]
    public class Data
    {
        public LevelsData levelsData;
        public MaxLevelsData maxLevelsData;

        public string strLanguage = Languages.English.ToString();
        public float volume = 1; // 1 = 100%

        public void SetDefaultData()
        {
            levelsData = new LevelsData();
            maxLevelsData = new MaxLevelsData();

            foreach (LevelType el in System.Enum.GetValues(typeof(LevelType)))
            {
                levelsData[el] = 1;
            }
        }
    }
    
    private static bool firstRun = false;
    
    public static Data data = new Data();

    public static void SaveGame()
    {
        if (!firstRun)//from changed value
            return;

        if (!Directory.Exists(Application.persistentDataPath + "/Save"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Save");

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(Application.persistentDataPath + "/Save" + "/SaveGame.sg", FileMode.Create);
        binaryFormatter.Serialize(fileStream, data);
        fileStream.Close();

        Debug.Log("Saving success");
    }

    public static void LoadGame()
    {
        firstRun = true;
        if (File.Exists(Application.persistentDataPath + "/Save" + "/SaveGame.sg"))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(Application.persistentDataPath + "/Save" + "/SaveGame.sg", FileMode.Open);
            data = (Data)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();

            Debug.Log("Loading success");
        }
        else
        {
            Debug.Log("Save didnt find");

            if (!Directory.Exists(Application.persistentDataPath + "/Save"))
                Directory.CreateDirectory(Application.persistentDataPath + "/Save");

            data.SetDefaultData();

            SaveGame();
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(Application.persistentDataPath + "/Save" + "/SaveGave.sg"))
        {
            File.Delete(Application.persistentDataPath + "/Save" + "/SaveGame.sg");
            Debug.Log("Save deleted");
        }
        else
        {
            Debug.Log("Save not deleted, save didnt find");
        }
    }
}

