using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafePoint : MonoBehaviour
{
    public GameObject sparks;
    public Transform playerPoint;

    private Animator anim;
    private Transform player;
    private GameObject bufer;
    private bool moveSparks;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        SafeUpdate();
    }

    private void SafeUpdate()
    {
        if(moveSparks)
        {
            if (Vector3.Distance(bufer.transform.position, playerPoint.position) < 0.5f)
            {
                moveSparks = false;
                Destroy(bufer, 3f);
            }
            else
            {
                bufer.transform.position = Vector3.Lerp(bufer.transform.position, playerPoint.position, Time.deltaTime);
            }
        }
    }

    public void Safe(Transform target)
    {
        player = target;
        moveSparks = true;
        bufer = Instantiate(sparks, target.position, target.rotation, transform);
    }
    public void OnRestart()
    {
        anim.SetTrigger("Restart");
    }

}
