using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendModulePoint : MonoBehaviour
{
    public UsingObject usingObject;
    public Transform friendPoint;

    private void OnDrawGizmosSelected()
    {
        if(friendPoint != null)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(friendPoint.position, 0.4f);
        }
    }
}
