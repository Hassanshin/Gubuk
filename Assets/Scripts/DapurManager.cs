using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DapurManager : MonoBehaviour
{
    private int[] stockMakanan = new int[3];

    private float[] durasiPembuatan = new float[3] { 5, 5, 5 };

    [SerializeField]
    private Transform[] v_StockView;

    private bool[] isProducing = new bool[3];

    // dipanggil dari GameManager
    public void UpgradeDapur(int _level)
    {
        float a = 5;

        switch (_level)
        {
            case 1:
                a = 3.5f;
                break;
            case 2:
                a = 2.5f;
                break;
            case 3:
                a = 1f;
                break;
            default:
                break;
        }

        Debug.Log(_level);

        durasiPembuatan = new float[3] { a, a, a };
    }

    // dipanggil dari GameManager, return true bila stok ada
    public bool DiPesan(int _index)
    {
        if(stockMakanan[_index] <= 0)
        {
            Debug.Log("Stock makanan " + _index + " habis");
            return false;
        }

        stockMakanan[_index]--;
        updateUI();
        return true;
    }

    // dipanggil dari Button UI
    public void BtnProduce(int _index)
    {
        //stockMakanan[_index]++;

        if (!isProducing[_index])
        {
            StartCoroutine(producingProg(_index));
        }
        else
        {
            Debug.Log("sedang memproduksi");
        }
        
    }

    IEnumerator producingProg(int _index)
    {
        float waitTime = durasiPembuatan[_index];
        float counter = 0f;
        isProducing[_index] = true;
        progressBarActive(_index, isProducing[_index]);

        while (counter < waitTime)
        {
            counter += Time.deltaTime;
            progressBarUI(_index, counter);

            yield return null;
        }

        stockMakanan[_index]++;
        updateUI();
        isProducing[_index] = false;
        progressBarActive(_index, isProducing[_index]);
    }

    private void Start()
    {
        updateUI();
    }

    private void updateUI()
    {
        for (int i = 0; i < v_StockView.Length; i++)
        {
            v_StockView[i].GetChild(0).GetComponent<Text>().text = stockMakanan[i] + "x";
        }
    }

    private void progressBarUI(int _index, float _prog)
    {
        v_StockView[_index].GetChild(1).GetComponent<Image>().fillAmount = _prog / durasiPembuatan[_index];
    }

    private void progressBarActive(int _index, bool _stat)
    {
        v_StockView[_index].GetChild(1).gameObject.SetActive(_stat);
    }
}
