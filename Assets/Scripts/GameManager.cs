using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    private MejaManager mejaManager;

    [Header("Komponen Prefab")]
    [SerializeField]
    private GameObject prefabPelanggan;

    [Header("Komponen Transform")]
    public Transform antrianParent;
    private List<Transform> antrianList = new List<Transform>();

    [Header("List")]
    public Transform poolPelanggan;
    //[SerializeField]
    private List<Pelanggan> pelangganMengantriList = new List<Pelanggan>();
    private List<Pelanggan> pelangganMakanList = new List<Pelanggan>();
    private List<Pelanggan> belumDapatMejaList = new List<Pelanggan>();


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

        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spawnPelanggan();
        }
    }

    private void spawnPelanggan()
    {
        GameObject pelanggan = Instantiate(prefabPelanggan, poolPelanggan.transform.position, poolPelanggan.transform.rotation);

        pelanggan.name = ("pelanggan " + counter);
        pelanggan.transform.SetParent(poolPelanggan);

        counter++;
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
        // buang dari antrian
        if (!pelangganMengantriList.Contains(newPelanggan))
            return;
        pelangganMengantriList.Remove(newPelanggan);        

        if (mejaManager.MejaPenuh())
        {
            Debug.Log("meja Penuh");
            // masukkan ke antrian duduk
            if (belumDapatMejaList.Contains(newPelanggan))
                return;
            belumDapatMejaList.Add(newPelanggan);

            newPelanggan.Berjalan(mejaManager.antrianMeja);
        }
        else
        {
            // masukkan ke antrian Makan
            if (pelangganMakanList.Contains(newPelanggan))
                return;
            pelangganMakanList.Add(newPelanggan);

            pelangganMakan();
        }

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
    public void SelesaiMakan(Meja mejaPelanggan)
    {
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
