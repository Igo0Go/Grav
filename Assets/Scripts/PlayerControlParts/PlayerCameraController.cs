using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : PlayerControllerBlueprint
{
    [Tooltip("Transform камеры игрока")]
    public Transform cam;

    [Space(10)]
    [Range(1, 360)]
    [Tooltip("Скорость вращения камеры")]
    public float camRotateSpeed = 180;
    [Range(0, 89)]
    [Tooltip("Ограничение камеры по вертикальному углу сверху")]
    public float maxYAngle = 89;
    [Range(-89, 0)]
    [Tooltip("Ограничение камеры по вертикальному углу снизу")]
    public float minYAngle = -89;

    private float currentCamAngle;
    private InputKit inputKit;

    private bool InMenu => PlayerStateController.InMenu;
    private PlayerState State => PlayerStateController.Status;
    private bool Alive => PlayerStateController.Alive;

    protected override void SetReferences(PlayerStateController playerState)
    {
        currentCamAngle = cam.localRotation.eulerAngles.x;
        inputKit = playerState.playerInputController.inputSettingsManager.inputKit;

        playerState.playerInputController.RotateInputEvent += PlayerRotate;
    }

    private void PlayerRotate(float mx, float my)
    {
        if (State > 0)
        {
            if (Alive)
            {
                if (InMenu)
                    return;

                if (mx != 0 || my != 0)
                {
                    transform.Rotate(Vector3.up, mx * camRotateSpeed * inputKit.sensivityMultiplicator * Time.deltaTime);
                    currentCamAngle -= my * camRotateSpeed * inputKit.sensivityMultiplicator * Time.deltaTime;
                    currentCamAngle = Mathf.Clamp(currentCamAngle, minYAngle, maxYAngle);
                    cam.localRotation = Quaternion.Euler(currentCamAngle, cam.localRotation.eulerAngles.y, 0);
                }
            }
        }
    }
}
