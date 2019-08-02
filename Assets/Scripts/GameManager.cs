using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    [Header("Komponen Prefab")]
    [SerializeField]
    private GameObject prefabPelanggan;

    [Header("Komponen Transform")]
    public Transform antrianParent;
    private List<Transform> antrianList = new List<Transform>();

    public Transform mejaParent;
    private List<Transform> mejaList = new List<Transform>();

    [Header("List")]
    public Transform poolPelanggan;
    //[SerializeField]
    private List<Pelanggan> pelangganMengantriList = new List<Pelanggan>();
    private List<Pelanggan> pelangganMakanList = new List<Pelanggan>();

    int counter;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        foreach (Transform child in antrianParent.transform)
        {
            antrianList.Add(child);

        }

        foreach (Transform child in mejaParent.transform)
        {
            mejaList.Add(child);

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
        if (!pelangganMengantriList.Contains(newPelanggan))
            return;
        pelangganMengantriList.Remove(newPelanggan);

        if (pelangganMakanList.Contains(newPelanggan))
            return;
        pelangganMakanList.Add(newPelanggan);

        pelangganMakan();
        pelangganMengantri();
    }

    private void pelangganMakan()
    {
        for (int i = 0; i < pelangganMakanList.Count; i++)
        {
            Pelanggan newPelanggan = pelangganMakanList[i];

            if (newPelanggan.MyState == statePelanggan.makan)
                continue;

            newPelanggan.Berjalan(mejaList [Random.Range(0, mejaList.Count - 1)] );
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
