using UnityEngine;

public class CameraView : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset;
    public float mouseSensitivity = 100f;  // 마우스 민감도
    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // 마우스 커서를 게임 중앙에 고정
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))  // 마우스 left쪽 버튼이 눌려진 경우
        {
            HandleCameraRotation();
        }
        UpdateCameraPosition();
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation -= mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // 카메라가 위아래로 과도하게 회전하는 것을 방지
        yRotation = Mathf.Clamp(-90f, yRotation, 90f);  // 카메라가 위아래로 과도하게 회전하는 것을 방지

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        playerTransform.Rotate(Vector3.up * mouseX);
        playerTransform.Rotate(Vector3.right * mouseY);
    }

    void UpdateCameraPosition()
    {
        transform.position = playerTransform.position + offset;
    }
}
