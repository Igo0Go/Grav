using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeChanger : UsingObject
{
    public List<UsingObject> actors;
    [Range(0.1f,100)]
    public float maxSize = 1;
    public float speed;

    private float currentTurget;
    private float currentSize;
    private float step;
    private bool resize;

    void Start()
    {
        
    }
    void Update()
    {
        if(resize)
        {
            currentSize += Time.deltaTime * speed;
            if(currentSize > currentTurget)
            {
                currentSize = currentTurget;
                resize = false;
            }
            transform.localScale = Vector3.one * currentSize;

        }
    }

    public override void ToStart()
    {
        currentSize = currentTurget = 0.01f;
        step = maxSize / actors.Count;
        transform.localScale = Vector3.one * currentSize;
    }
    public override void Use()
    {
        currentTurget += step;
        resize = true;
    }
}
