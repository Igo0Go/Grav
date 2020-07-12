using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravTest : MonoBehaviour
{
    [Tooltip("Transform камеры игрока")]
    public Transform cam;
    [Tooltip("Чтобы работал прыжок нужно выделить игрока в отдельный слой и указать его здесь")]
    public LayerMask jumpMask;
    [Range(1, 360)]
    [Tooltip("Скорость вращения камеры")]
    public float camRotateSpeed;
    [Range(1,5)]
    public float speed;
    [Range(0, 90)]
    [Tooltip("Ограничение камеры по вертикальному углу сверху")]
    public float maxYAngle;
    [Range(-90, 0)]
    [Tooltip("Ограничение камеры по вертикальному углу снизу")]
    public float minYAngle;

    private Rigidbody rb;
    private Collider currentCol;
    private Vector3 gravVector;
    [SerializeField]private Transform gravObj;
    private Rigidbody gravRb;

    private float currentCamAngle;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravVector = -transform.up;
        Cursor.lockState = CursorLockMode.Locked;
        if(gravObj != null)
        {
            gravRb = gravObj.GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && OnGround())
        {
            rb.AddForce(transform.up * 10, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (gravObj == null)
        {
            gravVector = Vector3.Slerp(-transform.up, Physics.gravity, Time.fixedDeltaTime);
            Quaternion rotBufer = Quaternion.FromToRotation(-transform.up, gravVector);
            transform.rotation = rotBufer * transform.rotation;
            if (!OnGround())
            {
                rb.AddForce(gravVector.normalized);
            }
        }
        else
        {
            gravVector = (gravObj.position - transform.position);

            float distance = gravVector.magnitude;
            float strength = 10 * rb.mass * gravRb.mass / (distance * distance);
            rb.AddForce(gravVector.normalized * strength);

            Quaternion rotBufer = Quaternion.FromToRotation(-transform.up, gravVector.normalized);
            transform.rotation = rotBufer * transform.rotation;
        }

        Vector3 down = Vector3.Project(rb.velocity, transform.up);
        Vector3 forward = transform.forward * Input.GetAxis("Vertical") * 4;
        Vector3 right = transform.right * Input.GetAxis("Horizontal") * 4;

        rb.velocity = down + right + forward;
    }

    private void LateUpdate()
    {
        PlayerRotate();
    }

    private void PlayerRotate()
    {
        float mx, my;
        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");

        if (mx != 0 || my != 0)
        {
            transform.Rotate(Vector3.up, mx * camRotateSpeed * Time.deltaTime);
            currentCamAngle -= my * camRotateSpeed * Time.deltaTime;
            currentCamAngle = Mathf.Clamp(currentCamAngle, minYAngle, maxYAngle);
            cam.localRotation = Quaternion.Euler(currentCamAngle, cam.localRotation.eulerAngles.y, 0);

        }
    }

    public bool OnGround()
    {
        if (Physics.Raycast(transform.position, gravVector, out RaycastHit hit, 2, ~jumpMask))
        {
    //        Vector3 bufer = hit.point - transform.position;
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Grav"))
        {
            currentCol = other;
            gravObj = other.transform;
            gravRb = gravObj.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == currentCol)
        {
            gravObj = null;
            gravRb = null;
        }
    }
}
