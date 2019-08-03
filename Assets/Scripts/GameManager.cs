using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    private MejaManager mejaManager;



    [Header("Komponen Transform")]
    public Transform antrianParent;
    private List<Transform> antrianList = new List<Transform>();
    public Transform poolPelangganParent;
    private List<Transform> poolPelangganList = new List<Transform>();
    [Header("List")]
    
    //[SerializeField]
    private List<Pelanggan> pelangganMengantriList = new List<Pelanggan>();
    private List<Pelanggan> pelangganMakanList = new List<Pelanggan>();
    private List<Pelanggan> belumDapatMejaList = new List<Pelanggan>();

    bool isPaused = false;

    int counter;

    private void Awake()
    {
        _instance = this;
        mejaManager = GetComponent<MejaManager>();
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
        StartCoroutine(randomPelangganMasuk());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            StopCoroutine(randomPelangganMasuk());
            StartCoroutine(randomPelangganMasuk());
        }

        // Debug
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spawnPelanggan();
        }
    }

    IEnumerator randomPelangganMasuk()
    {
        yield return new WaitForSeconds(2f);

        while (!isPaused)
        {
            spawnPelanggan();

            Debug.Log("spawn");

            float _random = Random.Range(2, 6);
            yield return new WaitForSeconds(_random);
        }
    }

    private void spawnPelanggan()
    {
        if (poolPelangganList.Count <= 0)
        {
            Debug.Log("Pool Pelanggan Habis");
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
        poolPelangganList.Add(newAdd);

        newAdd.position = poolPelangganParent.position;
        newAdd.rotation = poolPelangganParent.rotation;
    }

    // dipanggil dari pelanggan
    public void MasukAntrian(Pelanggan newPelanggan)
    {
        if (pelangganMengantriList.Contains(newPelanggan))
            return;

        pelangganMengantriList.Add(newPelanggan);
        
        pelangganMengantri();
    }

    // dipanggil dari pelanggan
    public void MasukMejaMakan(Pelanggan newPelanggan)
    {
                

        if (mejaManager.MejaPenuh())
        {
            Debug.Log("meja Penuh");
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
            Debug.Log("meja masih penuh");
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
    }

    private void pelangganMakan()
    {
        for (int i = 0; i < pelangganMakanList.Count; i++)
        {
            Pelanggan newPelanggan = pelangganMakanList[i];

            if (newPelanggan.MyState == statePelanggan.makan)
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
                Debug.Log("antrian penuh");
                continue;
            }

            if (newPelanggan.MyState == statePelanggan.pesan)
                continue;

            newPelanggan.Berjalan(antrianList[i]);
        }
    }
}
