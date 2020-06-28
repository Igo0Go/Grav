using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Клас контроллера от первого лица с примером применения пакета настроек.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class MyFPS : MonoBehaviour {

    public InputSettingsManager manager; //передаём InputManager, в котором указан нужный пакет.

    [Space(20), Range(1, 10)]
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

    public InputSettingsMenuScript menuPanel;

    private CharacterController characterController;
    private Vector3 dir;
    private float yAxis;
    private float currentYAxisAngel;
    private float currentXAxisAngel;

    private bool InMenu
    {
        get => _inMenu;
        set
        {
            _inMenu = value;
            Cursor.lockState = _inMenu ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _inMenu;
        }
    }
    private bool _inMenu;

    void Start()
    {
        currentYAxisAngel = transform.eulerAngles.y;
        currentXAxisAngel = transform.eulerAngles.x;
        characterController = GetComponent<CharacterController>();
        menuPanel.Initialize();
        InMenu = false;
    }
    void Update()
    {
        if(!InMenu) PlayerMove();
        PauseInput();
    }
    private void LateUpdate()
    {
        if(!InMenu) PlayerRotate();
    }

    private void PlayerMove()
    {
        float h, v;

        h = manager.GetAxis("Horizontal"); //оси получаем напрямую из менеджера
        v = manager.GetAxis("Vertical");

        Vector3 camForward = transform.forward;
        camForward.y = 0;

        if (h != 0 || v != 0)
        {
            dir = transform.right * h + camForward * v;
            dir *= Sprint();
        }
        if (characterController.isGrounded)
        {
            yAxis = 0;
            if (Input.GetKeyDown(manager.GetKey("Jump"))) //а тут пример обращения к конкретной кнопке
            {
                if (jumpForce < 0)
                {
                    Debug.LogError("Ты чё, больной?! Ты ж вниз прыгаешь...");
                }
                yAxis = jumpForce;
            }
        }
        yAxis -= grav * Time.deltaTime;
        dir = new Vector3(dir.x * speed * Time.deltaTime, yAxis * Time.deltaTime, dir.z * speed * Time.deltaTime);

        if (dir != Vector3.zero)
        {
            characterController.Move(dir);
        }
    }
    private void PlayerRotate()
    {
        float mx, my;
        mx = Input.GetAxis("Mouse X") * manager.inputKit.sensivityMultiplicator; //здесь применяется чувствительность мыши
        my = Input.GetAxis("Mouse Y") * manager.inputKit.sensivityMultiplicator;

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
        if(Input.GetKey(manager.GetKey("Sprint")))  //а кнопки позволяют просто динамически менять KeyCode, который передаём в методы Input
        {
            return sprintMultiplicator;
        }
        return 1;
    }

    public void PauseInput()
    {
        if (Input.GetKeyDown(manager.GetKey("Pause")))
        {
            menuPanel.GetSettingsPanel();
            InMenu = !InMenu;
        }
    }
}
