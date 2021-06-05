using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossParts : MonoBehaviour
{
    [HideInInspector]
    public BossTarget Manager;

    // Start is called before the first frame update
    void Start()
    {
        Manager = transform.root.GetComponentInChildren<BossTarget>();
    }
}
