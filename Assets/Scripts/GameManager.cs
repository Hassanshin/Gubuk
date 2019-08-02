using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform antrianParent;
    public List<Transform> antrianList;

    private void Start()
    {
        foreach (Transform child in antrianParent.transform)
        {
            antrianList.Add(child);
            print("Foreach loop: " + child.name);
        }
            
        
    }
}
