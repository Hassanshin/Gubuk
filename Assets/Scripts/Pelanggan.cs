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

    private bool isSpawned;
    private int makananIndex;
    private int memberiTip = 2;

    public int MemberiTip
    {
        get
        {
            return memberiTip;
        }
    }

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
    private Animator _anim;
    private Meja dudukDi;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private Sprite[] v_makanan;
    private Image v_makananImage;
    
    [SerializeField]
    private Sprite[] v_emot;
    private Image v_emotImage;

    GameManager GManager;
    private Camera cam;
    private Coroutine mood = null;
    Coroutine hidePopupCor = null;

    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        GManager = GameManager._instance;

        cam = Camera.main;
        canvas.worldCamera = cam;
        canvas.transform.localScale = new Vector3(-1, 1, 1);

        

        
    }

    private void Start()
    {
        v_makananImage = canvas.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        v_emotImage = canvas.transform.GetChild(2).GetChild(0).GetComponent<Image>();

        changeEmot("");

        hideFoodPopup();
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
        //Debug.Log(isSpawned + ":" + gameObject.name);
        if (!isSpawned)
            return;

        if (_state)
        {
            if(mood != null)
                StopCoroutine(mood);

            mood = StartCoroutine(moodEnum());
            
        }
        else
        {
            changeEmot("");

            if (mood != null)
                StopCoroutine(mood);
        }
        
    }

    private IEnumerator moodEnum()
    {
        changeEmot("puas");

        yield return new WaitForSeconds(1f);

        memberiTip = 1;
        changeEmot("biasa");

        yield return new WaitForSeconds(5f);

        memberiTip = 0;
        changeEmot("marah");

        yield return new WaitForSeconds(3f);

        // jika mood habis
        GManager.moodHabis(this);
        resetState();
    }

    private IEnumerator makanEnum()
    {
        yield return new WaitForSeconds(7f);

        myState = statePelanggan.bayar;
        changeEmot("bayar");

        _anim.SetBool("isEating", false);
    }

    void changeEmot(string _text)
    {
        canvas.transform.GetChild(0).GetComponent<Text>().text = _text ;

        if(hidePopupCor != null)
            StopCoroutine(hidePopupCor);

        v_emotImage.transform.parent.gameObject.SetActive(true);

        switch (_text)
        {
            case "biasa":
                v_emotImage.sprite = v_emot[0];
                break;
            case "puas":
                v_emotImage.sprite = v_emot[1];
                break;
            case "marah":
                v_emotImage.sprite = v_emot[2];
                break;
            default:
                v_emotImage.transform.parent.gameObject.SetActive(false);
                break;
        }

        if(v_emotImage.transform.parent.gameObject.activeSelf)
            hidePopupCor = StartCoroutine(hideEmotPopup());
    }

    IEnumerator hideEmotPopup()
    {
        yield return new WaitForSeconds(3f);

        v_emotImage.transform.parent.gameObject.SetActive(false);
    }

    void showFoodPopup()
    {
        v_makananImage.sprite = v_makanan[makananIndex];
        v_makananImage.transform.parent.gameObject.SetActive(true);
        
    }

    void hideFoodPopup()
    {
        if (!v_makananImage.transform.parent.gameObject.activeSelf)
            return;

        v_makananImage.transform.parent.gameObject.SetActive(!true);

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
        changeEmot("");

        memberiTip = 2;
        moodUpdate(false);
        myState = statePelanggan.jalan;
        dudukDi = null;
        isSpawned = false;
    }

    // dilanggil dari GameManager, MejaManager
    public void Berjalan(Transform _pos)
    {
        _anim.SetBool("isWalking", true);
        tujuan = _pos;
        _navAgent.isStopped = false;

        if (tujuan.tag == MEJA_TAG)
        {
            myState = statePelanggan.makan;
            moodUpdate(false);

            hideFoodPopup();
        }
        else if (tujuan.tag == ANTRIDUDUK_TAG)
        {
            myState = statePelanggan.antriDuduk;
            moodUpdate(true);

            hideFoodPopup();
        }
        else if (tujuan.tag == PESAN_TAG)
        {
            // state berubah jika sudah sampai di titik. void berhenti();
            
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

        _anim.SetBool("isWalking", false);

        if (tujuan.tag == ANTRIPESAN_TAG)
        {
            myState = statePelanggan.antriPesan;
            moodUpdate(true);
        }
        else if (tujuan.tag == PESAN_TAG)
        {
            myState = statePelanggan.pesan;
            moodUpdate(true);

            showFoodPopup();
        }
        else if (tujuan.tag == MEJA_TAG)
        {
            dudukDi = tujuan.GetComponent<Meja>();
            StartCoroutine( makanEnum() );

            _anim.SetBool("isEating", true);
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