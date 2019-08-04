using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Pelanggan : MonoBehaviour
{
    private const string MEJA_TAG = "meja", 
                         ANTRIPESAN_TAG = "antriPesan", 
                         PESAN_TAG = "pesan",
                         ANTRIDUDUK_TAG = "antriDuduk";

    [SerializeField]
    private bool isSpawned;

    [SerializeField]
    private int makananIndex;

    [SerializeField]
    private statePelanggan myState;
    public statePelanggan MyState
    {
        get
        {
            return myState;
        }
    }

    Transform tujuan;
    private float jarak;
    private NavMeshAgent _navAgent;
    private Meja dudukDi;

    GameManager GManager;
    [SerializeField]
    private Canvas canvas;
    private Camera cam;

    private Coroutine mood = null;

    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        GManager = GameManager._instance;

        cam = Camera.main;
        canvas.worldCamera = cam;
        canvas.transform.localScale = new Vector3(-1, 1, 1);

        changeText("");
    }

    private void LateUpdate()
    {
        canvas.transform.parent.LookAt(cam.transform);
    }

    // dipanggil dari GameManager, setelah di Spawn dari pool
    public void SpawnedFromPool()
    {
        GManager.MasukAntrian(this);
        isSpawned = true;
        makananIndex = Random.Range(0, 3);
    }

    private void Update()
    {
        navMeshUpdate();
    }

    private void moodUpdate(bool _state)
    {
        
        if (_state)
        {
            if(mood != null)
                StopCoroutine(mood);

            mood = StartCoroutine(moodEnum());
            
        }
        else
        {
            changeText("");

            if (mood != null)
                StopCoroutine(mood);
        }
        
    }

    private IEnumerator moodEnum()
    {
        changeText("biasa");

        yield return new WaitForSeconds(5f);

        changeText("marah");

        yield return new WaitForSeconds(3f);

        // jika mood habis
        GManager.moodHabis(this);
        resetState();
    }

    private IEnumerator makanEnum()
    {
        yield return new WaitForSeconds(7f);

        myState = statePelanggan.bayar;
        changeText("bayar");
    }

    void changeText(string _text)
    {
        canvas.transform.GetChild(1).GetComponent<Text>().text = _text ;
    }

    // dilanggil dari DragHandler, setelah disentuh
    public void TappedByUser()
    {
        //Debug.Log(myState + "."+ gameObject.name);

        switch (myState)
        {
            case statePelanggan.jalan:


                break;

            case statePelanggan.antriPesan:
                

                break;

            case statePelanggan.pesan:
                GManager.MasukMejaMakan(this, makananIndex);

                break;

            case statePelanggan.makan:
                
                
                break;

            case statePelanggan.bayar:
                if (dudukDi == null)
                    return;

                GManager.SelesaiMakan(this, dudukDi);

                // reset state kembali ke pool
                resetState();

                break;

            case statePelanggan.antriDuduk:
                GManager.MasukMejaMakanDariAntrianDuduk(this);

                break;
        }
    }

    void resetState()
    {
        changeText("");

        moodUpdate(false);
        myState = statePelanggan.jalan;
        dudukDi = null;
        isSpawned = false;
    }

    // dilanggil dari GameManager, MejaManager
    public void Berjalan(Transform _pos)
    {

        tujuan = _pos;
        _navAgent.isStopped = false;

        if (tujuan.tag == MEJA_TAG)
        {
            myState = statePelanggan.makan;
            moodUpdate(false);
        }
        else if (tujuan.tag == ANTRIDUDUK_TAG)
        {
            myState = statePelanggan.antriDuduk;
            moodUpdate(true);
        }
        else if (tujuan.tag == PESAN_TAG)
        {
            // state berubah jika sudah sampai di titik. void berhenti();
            moodUpdate(true);
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
            moodUpdate(true);
        }
        else if (tujuan.tag == PESAN_TAG)
        {
            myState = statePelanggan.pesan;
        }
        else if (tujuan.tag == MEJA_TAG)
        {
            dudukDi = tujuan.GetComponent<Meja>();
            StartCoroutine( makanEnum() );
        }

        _navAgent.isStopped = true;
        jarak = 0;
        tujuan = null;
    }

    void navMeshUpdate()
    {
        if (!isSpawned)
            return;

        if (_navAgent.isStopped || tujuan == null)
            return;

        jarak = Vector3.Distance(transform.position, tujuan.position);

        if (jarak <= 0.2f)
        {
            berhenti();
        }
    }
}

public enum statePelanggan
{
    jalan, antriPesan, pesan, makan, antriDuduk, bayar
    // mood di antriPesan, pesan, antriDuduk
}