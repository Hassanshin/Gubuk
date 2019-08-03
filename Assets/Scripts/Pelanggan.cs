using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pelanggan : MonoBehaviour
{
    private const string MEJA_TAG = "meja", 
                         ANTRIPESAN_TAG = "antriPesan", 
                         PESAN_TAG = "pesan",
                         ANTRIDUDUK_TAG = "antriDuduk";

    Transform tujuan;
    public float jarak;
    private NavMeshAgent _navAgent;
    private Meja dudukDi;

    [SerializeField]
    private statePelanggan myState;
    public statePelanggan MyState
    {
        get
        {
            return myState;
        }
    }

    GameManager GManager;

    void getKomponen()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        GManager = GameManager._instance;
    }

    private void Awake()
    {
        getKomponen();

        
    }

    // dipanggil dari GameManager, setelah di Spawn dari pool
    public void SpawnedFromPool()
    {
        GManager.MasukAntrian(this);
    }

    private void Update()
    {
        if (_navAgent.isStopped || tujuan == null)
            return;

        jarak = Vector3.Distance(transform.position, tujuan.position);

        if(jarak <= 0.2f)
        {
            berhenti();
        }
    }

    // dilanggil dari DragHandler, setelah disentuh
    public void TappedByUser()
    {
        Debug.Log(myState + "."+ gameObject.name);

        switch (myState)
        {
            case statePelanggan.jalan:


                break;

            case statePelanggan.antriPesan:
                

                break;

            case statePelanggan.pesan:
                GManager.MasukMejaMakan(this);

                break;

            case statePelanggan.makan:
                if (dudukDi == null)
                    return;

                GManager.SelesaiMakan(this, dudukDi);

                // reset state
                myState = statePelanggan.jalan;
                dudukDi = null;
                break;

            case statePelanggan.bayar:


                break;

            case statePelanggan.antriDuduk:
                GManager.MasukMejaMakanDariAntrianDuduk(this);

                break;
        }
    }

    // dilanggil dari GameManager, MejaManager
    public void Berjalan(Transform _pos)
    {
        tujuan = _pos;
        _navAgent.isStopped = false;

        if (tujuan.tag == MEJA_TAG)
        {
            myState = statePelanggan.makan;
        }
        else if (tujuan.tag == ANTRIDUDUK_TAG)
        {
            myState = statePelanggan.antriDuduk;
        }
        else
        {
            myState = statePelanggan.jalan;
        }

        _navAgent.SetDestination(_pos.position);
    }

    void berhenti()
    {
        transform.rotation = tujuan.rotation;

        if (tujuan.tag == ANTRIPESAN_TAG)
        {
            myState = statePelanggan.antriPesan;
        }
        else if (tujuan.tag == PESAN_TAG)
        {
            myState = statePelanggan.pesan;
        }
        else if (tujuan.tag == MEJA_TAG)
        {
            dudukDi = tujuan.GetComponent<Meja>();
        }

        _navAgent.isStopped = true;
        jarak = 0;
        tujuan = null;
    }

    
}

public enum statePelanggan
{
    jalan, antriPesan, pesan, makan, antriDuduk, bayar
}