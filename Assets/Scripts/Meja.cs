using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meja : MonoBehaviour
{
    private Pelanggan pelangganDiMeja;

    public bool IsUsed
    {
        get
        {
            if (pelangganDiMeja != null)
                return true;
            else
                return false;
        }
    }

    // dipanggil dari GameManager
    public void MasukDiMeja(Pelanggan newPelanggan)
    {
        pelangganDiMeja = newPelanggan;
    }

    // dipanggil dari GameManager
    public void KeluarDariMeja()
    {

        pelangganDiMeja = null;
    }
}
