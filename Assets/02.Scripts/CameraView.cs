using UnityEngine;

public class CameraView : MonoBehaviour
{
    public Transform playerTransform;
    
    public float distanceFromPlayer = 5f;
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerTransform = GameObject.Find("Eagle").GetComponent<Transform>();
    }

    private void Update()
    {
        
        if (!DialogueManager.isInDialogue && !InventoryUI.isInventory)
        {
            if(Input.GetMouseButton(0))
                HandleCameraRotation();

            UpdateCameraPosition();
        }
            
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void UpdateCameraPosition()
    {

        Vector3 direction = transform.forward;
        direction.y = 0;
        direction.Normalize();
        transform.position = playerTransform.position - direction * distanceFromPlayer + Vector3.up * 5f;
        
    }

    
}
