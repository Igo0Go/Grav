using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleReactor : MyTools
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Module"))
        {
            if (MyGetComponent(other.gameObject, out UsingObject usingObject))
            {
                usingObject.Use();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Module"))
        {
            if (MyGetComponent(other.gameObject, out UsingObject usingObject))
            {
                if (usingObject is LocationReactor reactor)
                {
                    if (!reactor.enterOnly)
                    {
                        usingObject.Use();
                    }
                }
            }
        }
    }
}
