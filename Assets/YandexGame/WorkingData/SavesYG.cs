using System.Collections.Generic;

namespace YG
{
    public enum Languages
    {
        English,
        Russian,
        Turkish
    }
    
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;

        // Тестовые сохранения для демо сцены
        // Можно удалить этот код, но тогда удалите и демо (папка Example)
        public int money = 1;                       // Можно задать полям значения по умолчанию
        public string newPlayerName = "Hello!";
        public bool[] openLevels = new bool[3];

        // Ваши сохранения
        public GameData gameData;
        public bool isFirstLoad = true;

        // Вы можете выполнить какие то действия при загрузке сохранений
        public SavesYG()
        {
        }
    }
    
    [System.Serializable]
    public class MaxLevelsData
    {
        public List<LevelType> levelType;
        public List<int> maxLevelList;
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
        public List<LevelType> levelType;
        public List<int> playerLevelNumber;

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
    public class GameData
    {
        public LevelsData levelsData;
        public MaxLevelsData maxLevelsData;
        
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

        public void UnlockAllLevels()
        {
            foreach (LevelType el in System.Enum.GetValues(typeof(LevelType)))
            {
                levelsData[el] = 30;
            }
        }
    }
}