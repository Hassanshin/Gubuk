using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class MainMenu : MonoBehaviour
{
    public static MainMenu _instance;

    [SerializeField]
    private Text v_diamond;

    [SerializeField]
    private Image[] v_Star;

    [SerializeField]
    private Image[] v_Upgrades;

    [SerializeField]
    private string playerName;

    public int[] savedLevelStar = new int[3];
    public int[] upgrades = new int[3];

    public int[,] starRequirement = new int[3, 3] { { 20, 40, 60 }, { 30, 50, 70 }, { 50, 70, 90 } };
    public float[,] spawnRandomDelay = new float[3, 2] { { 3, 6 }, { 2, 5 }, { 1, 3 } };

    public int Diamond = 10;

    public int selectedLevel;

    [Header("BGM")]
    [SerializeField]
    private AudioSource bgmMainMenu;
    [SerializeField]
    private AudioSource bgmGame;

    #region Button 

    public void BtnQuit()
    {
        Application.Quit();
    }

    public void BtnDiamond(int _value)
    {
        Diamond += _value;
        v_diamond.text = Diamond + "";
    }

    public void BtnChangeScene(int _level)
    {
        SceneManager.LoadScene(1);
        MainMenu._instance.bgmPlay(0);

        PlayerPrefs.SetInt("selectedLevel", _level);
        selectedLevel = _level;
    }

    public void BtnResetUpgrades()
    {
       
        for (int i = 0; i < 3; i++)
        {
            WriteShopUpgrades(i, 0);
            upgrades[i] = 0;
        }

        updateUpgradesUI();
    }

    public void BtnUpgradeToko(int _upgradeIndex)
    {
        if(Diamond <= 1)
        {
            Debug.Log("Diamond kurang");

            return;
        }

        if (upgrades[_upgradeIndex] >= 3)
        {
            Debug.Log("Maximum Upgrades");

            return;
        }

        int value = upgrades[_upgradeIndex];

        value++;

        WriteShopUpgrades(_upgradeIndex, value);
        Diamond -= 2;

        v_diamond.text = Diamond + "";

        updateUpgradesUI();
    }

    #endregion

    private void Awake()
    {
        
        if (_instance)
        {
            bgmMainMenu.Stop(); bgmGame.Stop();
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            bgmPlay(1);
        }
    }

    private void Start()
    {
        createFileIfNotExisted();

        LoadPlayerSaveJson();
        LoadShopUpgrades();
        LoadLevelStatsJson();

        v_diamond.text = Diamond + "";

        updateStar();
        updateUpgradesUI();
    }

    public void bgmPlay(int _index)
    {
        Debug.Log("sound playing" + _index);

        if (_instance)
        {

        }

        switch (_index)
        {
            case 0:
                bgmGame.Stop();

                if(!bgmMainMenu.isPlaying)
                    bgmMainMenu.Play();

                break;
            case 1:
                bgmMainMenu.Stop();

                if(!bgmGame.isPlaying)
                    bgmGame.Play();

                break;
        }
        
    }

    private void createFileIfNotExisted()
    {
        if (!File.Exists(Application.persistentDataPath + "/saveFile.json"))
            WritePlayerDataJson(0);

        if (!File.Exists(Application.persistentDataPath + "/levelStats.json"))
            WriteLevelStats();

        if (!File.Exists(Application.persistentDataPath + "/upgradesFile.json"))
            WriteShopUpgrades(0, 0);

        
    }

    private void updateUpgradesUI()
    {
        LoadShopUpgrades();

        for (int i = 0; i < v_Upgrades.Length; i++)
        {
            v_Upgrades[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < upgrades[0]; i++)
        {
            v_Upgrades[i].gameObject.SetActive(true);

        }

        for (int i = 0; i < upgrades[1]; i++)
        {
            v_Upgrades[i + 3].gameObject.SetActive(true);

        }

        for (int i = 0; i < upgrades[2]; i++)
        {
            v_Upgrades[i + 6].gameObject.SetActive(true);

        }
    }

    private void updateStar()
    {
        for (int i = 0; i < savedLevelStar[0]; i++)
        {
            v_Star[i].gameObject.SetActive(true);
            
        }

        for (int i = 0; i < savedLevelStar[1]; i++)
        {
            v_Star[i + 3].gameObject.SetActive(true);

        }

        for (int i = 0; i < savedLevelStar[2]; i++)
        {
            v_Star[i + 6].gameObject.SetActive(true);

        }
    }

    #region Json Method Save and Load

    public class PlayerData
    {
        public string j_name;
        public int[] j_savedLevelStar = new int[3];
        
    }

    public class ShopUpgrades
    {
        public int[] j_upgrades = new int[3];
    }

    public class LevelStats
    {
        public int[] j_levelReq0 = new int[3];
        public int[] j_levelReq1 = new int[3];
        public int[] j_levelReq2 = new int[3];

        public int[] j_spawnDelay0 = new int[2];
        public int[] j_spawnDelay1 = new int[2];
        public int[] j_spawnDelay2 = new int[2];
    }

    private void LoadPlayerSaveJson()
    {
        string _playerData = File.ReadAllText(Application.persistentDataPath + "/saveFile.json");   // load file yang mana

        PlayerData loadedData = new PlayerData();                                           // new class 

        loadedData = JsonUtility.FromJson<PlayerData>(_playerData);                             // ambil class dari json

        // ganti di unity


        playerName = loadedData.j_name;

        // load saved star
        for (int i = 0; i < loadedData.j_savedLevelStar.Length; i++)
        {
            savedLevelStar[i] = loadedData.j_savedLevelStar[i];
        }

        
    }

    private void LoadShopUpgrades()
    {
        string _playerData = File.ReadAllText(Application.persistentDataPath + "/upgradesFile.json");   // load file yang mana

        ShopUpgrades loadedData = new ShopUpgrades();                                           // new class 

        loadedData = JsonUtility.FromJson<ShopUpgrades>(_playerData);                             // ambil class dari json

        // ganti di unity

        for (int i = 0; i < loadedData.j_upgrades.Length; i++)
        {
            upgrades[i] = loadedData.j_upgrades[i];
        }
    }

    private void LoadLevelStatsJson()
    {
        string _playerData = File.ReadAllText(Application.persistentDataPath + " /levelStats.json");   // load file yang mana

        LevelStats loadedData = new LevelStats();                                           // new class 

        loadedData = JsonUtility.FromJson<LevelStats>(_playerData);                             // ambil class dari json

        // ganti di unity

        // load level star requriements
        for (int i = 0; i < loadedData.j_levelReq0.Length; i++)
        {
            starRequirement[0, i] = loadedData.j_levelReq0[i];
        }

        for (int i = 0; i < loadedData.j_levelReq1.Length; i++)
        {
            starRequirement[1, i] = loadedData.j_levelReq1[i];
        }

        for (int i = 0; i < loadedData.j_levelReq2.Length; i++)
        {
            starRequirement[2, i] = loadedData.j_levelReq2[i];
        }

        // load level spawn delay
        for (int i = 0; i < loadedData.j_spawnDelay0.Length; i++)
        {
            spawnRandomDelay[0, i] = loadedData.j_spawnDelay0[i];
        }

        for (int i = 0; i < loadedData.j_spawnDelay1.Length; i++)
        {
            spawnRandomDelay[1, i] = loadedData.j_spawnDelay1[i];
        }

        for (int i = 0; i < loadedData.j_spawnDelay2.Length; i++)
        {
            spawnRandomDelay[2, i] = loadedData.j_spawnDelay2[i];
        }

    }

    // dipanggil dari Win Lose
    public void WritePlayerDataJson(int _star)
    {
        PlayerData loadedData = new PlayerData();                                          // new class 

        loadedData.j_savedLevelStar[selectedLevel] = _star;                                        // rubah datanya unity
        loadedData.j_name = "M. Fahmi Al Kushairi";
        
        string save = JsonUtility.ToJson(loadedData);                                              // masukkan ke json datanya

        File.WriteAllText(Application.persistentDataPath + "/saveFile.json", save);                // tulis file yang mana

        Debug.Log(_star + " on  " + selectedLevel);
    }

    private void WriteShopUpgrades(int _upgradeIndex, int _value)
    {
        ShopUpgrades loadedData = new ShopUpgrades();                                          // new class 

        for (int i = 0; i < 3; i++)
        {
            loadedData.j_upgrades[i] = upgrades[i];
        }

        loadedData.j_upgrades[_upgradeIndex] = _value;                                                 // rubah datanya unity
        
        
        string save = JsonUtility.ToJson(loadedData);                                              // masukkan ke json datanya

        File.WriteAllText(Application.persistentDataPath + "/upgradesFile.json", save);                // tulis file yang mana

        Debug.Log(loadedData.j_upgrades[_upgradeIndex]);

    }

    private void WriteLevelStats()
    {
        LevelStats loadedData = new LevelStats();                                          // new class 

        loadedData.j_levelReq0 = new int[3] { 20, 40, 60 };
        loadedData.j_levelReq1 = new int[3] { 30, 50, 70 };
        loadedData.j_levelReq2 = new int[3] { 50, 70, 90 };

        loadedData.j_spawnDelay0 = new int[2] { 3, 6};
        loadedData.j_spawnDelay1 = new int[2] { 2, 5};
        loadedData.j_spawnDelay2 = new int[2] { 1, 3};

        string save = JsonUtility.ToJson(loadedData);                                              // masukkan ke json datanya

        File.WriteAllText(Application.persistentDataPath + " /levelStats.json", save);                // tulis file yang mana
    }

    #endregion
}
