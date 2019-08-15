using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MyFPS : MonoBehaviour {

    public InputSettingsManager inputSettingsManager;

    [Space(20)]
    [Range(1, 10)]
    public float speed;
    [Range(1, 360)]
    public float camRotateSpeed;
    [Range(0, 90)]
    public float maxYAngel;
    [Range(-90, 0)]
    public float minYAngel;
    [Range(1, 5)]
    public float sprintMultiplicator;

    [Space(10)]
    [Header("Насройки гравитации")]
    [Range(0, 100)]
    public float grav;
    [Range(-100, 100)]
    public float jumpForce;
    
    private CharacterController characterController;
    private Vector3 dir;
    private float yAxis;
    private float currentYAxisAngel;
    private float currentXAxisAngel;

    void Start()
    {
        currentYAxisAngel = transform.eulerAngles.y;
        currentXAxisAngel = transform.eulerAngles.x;
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        PlayerMove();
    }
    private void LateUpdate()
    {
        PlayerRotate();
    }

    private void PlayerMove()
    {
        float h, v;

        v = inputSettingsManager.GetAxis("Vertical");
        h = inputSettingsManager.GetAxis("Horizontal");

        Vector3 camForward = transform.forward;
        camForward.y = 0;

        if (h != 0 || v != 0)
        {
            dir = transform.right * h + camForward * v;
            dir *= Sprint();
        }
        if (characterController.isGrounded)
        {
            if (Input.GetKeyDown(inputSettingsManager.GetKey("Jump")))
            {
                if (jumpForce < 0)
                {
                    Debug.LogError("Ты чё, больной?! Ты ж вниз прыгаешь...");
                }
                yAxis = jumpForce;
                
            }
        }
        else
        {
            yAxis -= grav * Time.deltaTime;
        }
        dir = new Vector3(dir.x * speed * Time.deltaTime, yAxis * Time.deltaTime, dir.z * speed * Time.deltaTime);
        if (dir != Vector3.zero)
        {
            characterController.Move(dir);
        }
    }
    private void PlayerRotate()
    {
        float mx, my;
        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");

        if (mx != 0 || my != 0)
        {
            currentYAxisAngel += mx * camRotateSpeed * Time.deltaTime;
            currentXAxisAngel -= my * camRotateSpeed * Time.deltaTime;
            currentXAxisAngel = Mathf.Clamp(currentXAxisAngel, minYAngel, maxYAngel);
            transform.rotation = Quaternion.Euler(currentXAxisAngel, currentYAxisAngel, 0);
        }
    }
    private float Sprint()
    {
        if(Input.GetKey(inputSettingsManager.GetKey("Sprint")))
        {
            return sprintMultiplicator;
        }
        return 1;
    }
}
