using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pelanggan : MonoBehaviour
{
    private const string MEJA_TAG = "meja", ANTRI_TAG = "antri", PESAN_TAG = "pesan";

    Transform tujuan;
    public float jarak;
    private NavMeshAgent _navAgent;

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

    private void Start()
    {
        getKomponen();

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

    // dilanggil dari DragHandler 
    public void TappedByUser()
    {
        Debug.Log(myState + "."+ gameObject.name);

        switch (myState)
        {
            case statePelanggan.jalan:


                break;

            case statePelanggan.antri:
                

                break;
            case statePelanggan.pesan:
                GManager.MasukMejaMakan(this);

                break;

            case statePelanggan.makan:
                

                break;
            case statePelanggan.bayar:


                break;
        }
    }

    // dilanggil dari GameManager
    public void Berjalan(Transform _pos)
    {
        tujuan = _pos;
        _navAgent.isStopped = false;
        myState = statePelanggan.jalan;

        _navAgent.SetDestination(_pos.position);
    }

    void berhenti()
    {
        transform.rotation = tujuan.rotation;

        if (tujuan.tag == MEJA_TAG)
        {
            myState = statePelanggan.makan;
        }
        else if (tujuan.tag == ANTRI_TAG)
        {
            myState = statePelanggan.antri;
        }
        else if (tujuan.tag == PESAN_TAG)
        {
            myState = statePelanggan.pesan;
        }

        _navAgent.isStopped = true;
        jarak = 0;
        tujuan = null;
    }

    
}

public enum statePelanggan
{
    jalan, antri, pesan, makan, bayar
}