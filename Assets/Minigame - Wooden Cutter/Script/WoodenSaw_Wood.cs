using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WoodenSaw_Wood : MonoBehaviour
{
    public bool isTrap = false;
    [Space(10)]
    public Transform leftPoint, rightPoint;
    public Image top, bottom, full;
    public TrailRenderer trail;

    public bool isCutting = false;
    public bool alreadyCut = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
