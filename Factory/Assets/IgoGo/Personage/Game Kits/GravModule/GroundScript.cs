using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GroundScript : MonoBehaviour
{
    public bool OnGround = false;
    public GameObject player;
    public OnGroundHelper helper;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != player)
        {
            OnGround = true;
            helper?.Invoke();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject != player && !OnGround)
        {
            OnGround = true;
            helper?.Invoke();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject != player)
        {
            OnGround = false;
            helper?.Invoke();
        }
    }
}

public delegate void OnGroundHelper();
