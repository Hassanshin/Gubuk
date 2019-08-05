using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private MainMenu mainMenu;
    public static GameManager _instance;
    private MejaManager mejaManager;
    private DapurManager dapurManager;
    private WinLose winLose;

    [Header("Level Stats")]
    private int selectedLevel = 9;
    private float[] spawnRandomDelay = new float[2];
    private int[] starRequirement = new int[3];

    [Header("Komponen Transform")]
    public Transform antrianParent;
    private List<Transform> antrianList = new List<Transform>();
    public Transform poolPelangganParent;
    private List<Transform> poolPelangganList = new List<Transform>();

    [Header("List")]
    private List<Pelanggan> pelangganMengantriList = new List<Pelanggan>();
    private List<Pelanggan> pelangganMakanList = new List<Pelanggan>();
    private List<Pelanggan> belumDapatMejaList = new List<Pelanggan>();

    bool isPaused = false;

    private float money;
    public float Money
    {
        get
        {
            return money;
        }

        set
        {
            money = value;
        }
    }

    int counter;
    private Coroutine pelangganMasukCor;

    private void Awake()
    {
        _instance = this;
        mejaManager = GetComponent<MejaManager>();
        dapurManager = GetComponent<DapurManager>();
        winLose = GetComponent<WinLose>();

        loadLevelStats();

        ApplyUpgrades();

    }

    private void ApplyUpgrades()
    {
        switch (mainMenu.upgrades[0])
        {
            case 0:
                mejaManager.MejaUpgrade(5);
                break;
            case 1:
                mejaManager.MejaUpgrade(4);
                break;
            case 2:
                mejaManager.MejaUpgrade(3);
                break;
            case 3:
                mejaManager.MejaUpgrade(0);
                break;
            default:
                break;
        }

        dapurManager.UpgradeDapur(mainMenu.upgrades[1]);

        winLose.UpgradeTime(mainMenu.upgrades[2]);
    }

    private void loadLevelStats()
    {
        mainMenu = MainMenu._instance;
        if (mainMenu == null)
            return;
        selectedLevel = mainMenu.selectedLevel;

        for (int i = 0; i < starRequirement.Length; i++)
        {
            starRequirement[i] = mainMenu.starRequirement[selectedLevel, i];
            winLose.starReq[i] = starRequirement[i];
        }
        
        spawnRandomDelay[0] = mainMenu.spawnRandomDelay[selectedLevel, 0];
        spawnRandomDelay[1] = mainMenu.spawnRandomDelay[selectedLevel, 1];

        
    }

    private void Start()
    {
        foreach (Transform child in antrianParent.transform)
        {
            antrianList.Add(child);
        }

        foreach (Transform child in poolPelangganParent.transform)
        {
            poolPelangganList.Add(child);
        }

        // Mulai Random Pelanggan
        pelangganMasukCor = StartCoroutine(randomPelangganMasuk());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            StopCorPelanggan();
            pelangganMasukCor = StartCoroutine(randomPelangganMasuk());
        }

        // Debug
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spawnPelanggan();
        }
    }

    IEnumerator randomPelangganMasuk()
    {
        yield return new WaitForSeconds(1f);
        while (!isPaused)
        {
            spawnPelanggan();

            float _random = Random.Range(spawnRandomDelay[0], spawnRandomDelay[1]);
            yield return new WaitForSeconds(_random);
        }
    }

    // dipanggil dari WinLose
    public void StopCorPelanggan()
    {
        StopCoroutine(pelangganMasukCor);
    }

    private void spawnPelanggan()
    {
        if (poolPelangganList.Count <= 0)
        {
            //Debug.Log("Pool Pelanggan Habis");
            return;
        }

        GameObject pelanggan = spawnFromPool(poolPelangganParent.transform.position, poolPelangganParent.transform.rotation);

        pelanggan.name = ("pelanggan " + counter);
        pelanggan.transform.SetParent(poolPelangganParent);

        counter++;
    }

    // Instansiate from pool
    private GameObject spawnFromPool(Vector3 _pos, Quaternion _rot)
    {
        Transform newSpawn = poolPelangganList[Random.Range(0, poolPelangganList.Count)];

        newSpawn.position = _pos;
        newSpawn.rotation = _rot;

        poolPelangganList.Remove(newSpawn);
        newSpawn.gameObject.SetActive(true);
        newSpawn.GetComponent<Pelanggan>().SpawnedFromPool();

        return newSpawn.gameObject;
    }

    // Destroy and add to Pool
    private void deSpawnAddToPool (Transform newAdd)
    {
        newAdd.gameObject.SetActive(false);

        if(!poolPelangganList.Contains(newAdd))
            poolPelangganList.Add(newAdd);

        newAdd.position = poolPelangganParent.position;
        newAdd.rotation = poolPelangganParent.rotation;
    }

    // dipanggil dari pelanggan
    public void moodHabis(Pelanggan newPelanggan)
    {
        deSpawnAddToPool(newPelanggan.transform);

        removeFromAllList(newPelanggan);

        pelangganMengantri();
    }

    private void removeFromAllList(Pelanggan newPelanggan)
    {
        if (pelangganMengantriList.Contains(newPelanggan))
            pelangganMengantriList.Remove(newPelanggan);

        if (pelangganMakanList.Contains(newPelanggan))
            pelangganMakanList.Remove(newPelanggan);

        if (belumDapatMejaList.Contains(newPelanggan))
            belumDapatMejaList.Remove(newPelanggan);
    }

    // dipanggil dari pelanggan
    public void MasukAntrian(Pelanggan newPelanggan)
    {
        if (!pelangganMengantriList.Contains(newPelanggan))
            pelangganMengantriList.Add(newPelanggan);

        pelangganMengantri();
    }

    // dipanggil dari pelanggan
    public void MasukMejaMakan(Pelanggan newPelanggan, int _indexMakanan)
    {
        // Cek stok makanan dengan makanan pelanggan
        if (!dapurManager.DiPesan(_indexMakanan))
        {
            return;
        }

        // Masuk Meja Makan setelah makanan siap
        if (mejaManager.MejaPenuh())
        {
            //Debug.Log("meja Penuh");
            // masukkan ke antrian duduk
            if (!belumDapatMejaList.Contains(newPelanggan))
                belumDapatMejaList.Add(newPelanggan);

            newPelanggan.Berjalan(mejaManager.antrianMeja);
        }
        else
        {
            // masukkan ke antrian Makan
            if (!pelangganMakanList.Contains(newPelanggan))
                pelangganMakanList.Add(newPelanggan);

            pelangganMakan();
        }

        // buang dari antrian
        if (pelangganMengantriList.Contains(newPelanggan))
            pelangganMengantriList.Remove(newPelanggan);

        pelangganMengantri();
    }

    // dipanggil dari pelanggan
    public void MasukMejaMakanDariAntrianDuduk(Pelanggan newPelanggan)
    {
        if (mejaManager.MejaPenuh())
        {
            //Debug.Log("meja masih penuh");
            return;
        }

        // buang dari antrian duduk
        if (!belumDapatMejaList.Contains(newPelanggan))
            return;
        belumDapatMejaList.Remove(newPelanggan);

        // masukkan ke antrian Makan
        if (pelangganMakanList.Contains(newPelanggan))
            return;
        pelangganMakanList.Add(newPelanggan);

        pelangganMakan();
    }

    // dipanggil dari pelanggan
    public void SelesaiMakan(Pelanggan newPelanggan, Meja mejaPelanggan)
    {
        if (pelangganMakanList.Contains(newPelanggan))
            pelangganMakanList.Remove(newPelanggan);

        deSpawnAddToPool(newPelanggan.transform);
        mejaPelanggan.KeluarDariMeja();
        mejaManager.mejaAvailable.Add(mejaPelanggan);

        // menambah uang
        money += 3 + newPelanggan.MemberiTip;
        winLose.UpdateUI();
    }

    private void pelangganMakan()
    {
        for (int i = 0; i < pelangganMakanList.Count; i++)
        {
            Pelanggan newPelanggan = pelangganMakanList[i];

            if (newPelanggan.MyState == statePelanggan.makan ||
                newPelanggan.MyState == statePelanggan.bayar)
                continue;

            mejaManager.MasukMeja(newPelanggan);
        }
    }

    private void pelangganMengantri()
    {
        for (int i = 0; i < pelangganMengantriList.Count; i++)
        {
            Pelanggan newPelanggan = pelangganMengantriList[i];

            if (antrianList.Count < pelangganMengantriList.Count)
            {
                //Debug.Log("antrian penuh");
                continue;
            }

            if (newPelanggan.MyState == statePelanggan.pesan)
                continue;

            newPelanggan.Berjalan(antrianList[i]);
        }
    }
}
