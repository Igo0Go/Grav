using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public GameObject sparks;
    public Transform playerPoint;
    public ModuleController moduleController;

    private Animator anim;
    private Transform player;
    private GameObject bufer;
    private bool moveSparks;
    private SphereGravModule planet;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        SaveUpdate();
    }

    public void Save(Transform target)
    {
        moduleController.Save();
        player = target;
        moveSparks = true;
        bufer = Instantiate(sparks, target.position, target.rotation, transform);
    }
    public void OnRestart()
    {
        anim.SetTrigger("Restart");
        moduleController.Load();
    }
    
    private void SaveUpdate()
    {
        if (moveSparks)
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
}
