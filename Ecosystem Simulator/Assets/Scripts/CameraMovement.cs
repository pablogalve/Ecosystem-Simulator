using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float lookSpeedH = 2f;

    [SerializeField]
    private float lookSpeedV = 2f;

    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private float fastMoveSpeed = 5f;

    private float yaw = 0f;
    private float pitch = 0f;

    private void Start()
    {
        // Initialize the correct initial rotation
        this.yaw = this.transform.eulerAngles.y;
        this.pitch = this.transform.eulerAngles.x;
    }

    private void Update()
    {
        // Only work with the Left Alt pressed
        //if (Input.GetKey(KeyCode.LeftAlt))
        {
            //Look around with Left Mouse
            if (Input.GetMouseButton(0))
            {
                this.yaw += this.lookSpeedH * Input.GetAxis("Mouse X");
                this.pitch -= this.lookSpeedV * Input.GetAxis("Mouse Y");

                this.transform.eulerAngles = new Vector3(this.pitch, this.yaw, 0f);
            }

            //Zoom in and out with W and S
            if (Input.GetKey(KeyCode.LeftShift)) 
                this.transform.Translate(0, 0, Input.GetAxis("Vertical") * this.fastMoveSpeed, Space.Self);
            else 
                this.transform.Translate(0, 0, Input.GetAxis("Vertical") * this.moveSpeed, Space.Self);

            //Move left and right with A and D
            if (Input.GetKey(KeyCode.LeftShift)) this.transform.Translate(Input.GetAxis("Horizontal") * this.fastMoveSpeed, 0, 0, Space.Self);
            else this.transform.Translate(Input.GetAxis("Horizontal") * this.moveSpeed, 0, 0, Space.Self);
        }
    }
}