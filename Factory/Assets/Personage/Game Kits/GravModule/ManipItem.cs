using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipItem : MyTools
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Module"))
        {
            if(MyGetComponent(other.gameObject, out ManipReactor manip))
            {
                if(manip.manip == this)
                {
                    manip.Use();
                }
            }
        }
    }
}
