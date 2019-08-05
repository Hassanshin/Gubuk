using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinLose : MonoBehaviour
{

    private float stageTimer = 60f;
    public int[] starReq = new int[3] { 1, 1, 1 };

    private Coroutine timer;
    float money;

    [Header("UI")]
    [SerializeField]
    private Text v_WinLoseBanner;

    [SerializeField]
    private Image[] v_Star;

    [SerializeField]
    private Text v_MoneyInGame;

    [SerializeField]
    private Text v_TimerInGame;

    private GameManager GManager;

    private void Start()
    {
        GManager = GetComponent<GameManager>();
        timer = StartCoroutine( timerNum() );
        UpdateUI();
    }

    // dipanggil dari game manager
    public void UpdateUI()
    {
        money = GManager.Money;
        v_MoneyInGame.text = money + "";
    }

    public void BtnBackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    IEnumerator timerNum()
    {
        float _value = stageTimer;
        v_TimerInGame.gameObject.SetActive(true);
        v_TimerInGame.text = _value + "";

        while (_value > 0)
        {
            yield return new WaitForSeconds(1f);

            _value--;
            v_TimerInGame.text = _value + "";
        }

        gameOver();
    }

    private void gameOver()
    {
        GManager.StopCorPelanggan();

        money = GManager.Money;

        v_WinLoseBanner.transform.parent.gameObject.SetActive(true);

        int starThisMatch = star();

        MainMenu._instance.WriteLevelStarJson(starThisMatch);

        if (starThisMatch >= 1)
        {
            v_WinLoseBanner.text = "Menang";

            
        }
        else
        {
            v_WinLoseBanner.text = "Kalah";
        }

        for (int i = 0; i < starThisMatch; i++)
        {
            v_Star[i].gameObject.SetActive(true);
        }

    }

    int star()
    {
        int _result = 0;

        if(money >= starReq[2])
        {
            _result = 3;
        }
        else if(money >= starReq[1])
        {
            _result = 2;
        }
        else if(money >= starReq[0])
        {
            _result = 1;
        } 

        return _result;
    }
}
