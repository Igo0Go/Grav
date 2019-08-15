using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleReactor : MyTools
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Module"))
        {
            UsingObject usingObject;
            if(MyGetComponent(other.gameObject, out usingObject))
            {
                usingObject.Use();
                if (usingObject is LocationReactor)
                {
                    if (((LocationReactor)usingObject).once)
                    {
                        Destroy(other.gameObject);
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Module"))
        {
            UsingObject usingObject;
            if (MyGetComponent(other.gameObject, out usingObject))
            {
                if(usingObject is LocationReactor)
                {
                    if (!((LocationReactor)usingObject).enterOnly)
                    {
                        usingObject.Use();
                    }
                }
            }
        }
    }
}
