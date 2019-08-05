using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MejaManager : MonoBehaviour
{
    
    public Transform mejaParent;
    public Transform antrianMeja;

    private List<Meja> mejaList = new List<Meja>();
    public List<Meja> mejaAvailable = new List<Meja>();
    

    private void Start()
    {

        foreach (Transform child in mejaParent.transform)
        {
            if(child.gameObject.activeSelf)
                mejaList.Add(child.GetComponent<Meja>());

        }

        mejaAvailable.AddRange(mejaList);
    }

    // dipanggin dari GameManager
    public void MejaUpgrade(int _total)
    {

        for (int i = mejaParent.childCount - 1; i > mejaParent.childCount - _total; i--)
        {
            mejaParent.GetChild(i).gameObject.SetActive(false);
        }
        
    }

    // dipanggin dari GameManager
    public void MasukMeja(Pelanggan newPelanggan)
    {
        Meja _mejaIndex = randomMeja();

        _mejaIndex.MasukDiMeja(newPelanggan);
        newPelanggan.Berjalan(_mejaIndex.transform);
        mejaAvailable.Remove(_mejaIndex);
    }


    //dipanggin dari GameManager
    public bool MejaPenuh()
    {
        bool _result = false;

        if (mejaAvailable.Count <= 0)
            _result = true;

        return _result;
    }

    Meja randomMeja()
    {
        int _random = Random.Range(0, mejaAvailable.Count);

        return mejaAvailable[_random];
    }
}
