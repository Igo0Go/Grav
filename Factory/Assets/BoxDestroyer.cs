using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDestroyer : MyTools
{
    public bool clear = false;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        if(clear)
        {
            BoxCollider col;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (MyGetComponent(transform.GetChild(i).gameObject, out col))
                {
                    Destroy(col);
                }
            }
            clear = false;
        }
    }
}
