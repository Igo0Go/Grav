using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDestroyer : MyTools
{
    public bool clear = false;

    private void OnDrawGizmosSelected()
    {
        if(clear)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (MyGetComponent(transform.GetChild(i).gameObject, out BoxCollider col))
                {
                    DestroyImmediate(col);
                }
            }
            clear = false;
        }
    }
}
