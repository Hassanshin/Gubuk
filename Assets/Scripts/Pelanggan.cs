using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pelanggan : MonoBehaviour
{
    private NavMeshAgent _navAgent;

    void getKomponen()
    {
        _navAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        getKomponen();


    }

    
}