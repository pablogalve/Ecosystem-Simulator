using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float lookSpeedH = 2f;
    [SerializeField] private float lookSpeedV = 2f;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float fastMoveSpeed = 5f;

    [SerializeField] private float rotateAroundEntitySpeed = 5f;
    public GameObject selectedGO = null;
    private bool moving = false;

    private float yaw = 0f;
    private float pitch = 0f;

    public GameObject gameObjectWithUIScript = null;
    private SelectedAnimalStats UIScript = null;

    private void Start()
    {
        // Initialize the correct initial rotation
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        UIScript = gameObjectWithUIScript.GetComponent<SelectedAnimalStats>();
    }

    private void Update()
    {
        SelectGameObjectByClicking();

        if(selectedGO == null)
        {
            NormalCameraMovement();
        }
        else
        {
            RotateAroundSelectedEntity();

            if(moving) MoveToTarget();
        }
    }

    private void NormalCameraMovement()
    {
        //Look around with Left Mouse
        if (Input.GetMouseButton(0))
        {
            yaw += lookSpeedH * Input.GetAxis("Mouse X");
            pitch -= lookSpeedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

        //Zoom in and out with W and S
        if (Input.GetKey(KeyCode.LeftShift))
            transform.Translate(0, 0, Input.GetAxis("Vertical") * fastMoveSpeed, Space.Self);
        else
            transform.Translate(0, 0, Input.GetAxis("Vertical") * moveSpeed, Space.Self);

        //Move left and right with A and D
        if (Input.GetKey(KeyCode.LeftShift)) transform.Translate(Input.GetAxis("Horizontal") * fastMoveSpeed, 0, 0, Space.Self);
        else transform.Translate(Input.GetAxis("Horizontal") * moveSpeed, 0, 0, Space.Self);
    }

    private void RotateAroundSelectedEntity()
    {
        transform.RotateAround(selectedGO.transform.position, transform.right, -Input.GetAxis("Mouse Y") * rotateAroundEntitySpeed * 0.5f);
        transform.RotateAround(selectedGO.transform.position, Vector3.up, -Input.GetAxis("Mouse X") * rotateAroundEntitySpeed);        
    }

    private void SelectGameObjectByClicking()
    {
        // Select GO by clicking with the mouse
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                if (hitInfo.transform.gameObject.tag == "Herbivore" || hitInfo.transform.gameObject.tag == "Carnivore")
                {
                    selectedGO = hitInfo.transform.gameObject;
                    transform.SetParent(selectedGO.transform);

                    moving = true;
                    UIScript.SelectAnimal(selectedGO);
                }
                else // There is a hit with an object that can't be selected
                {
                    DeselectGO();
                }
            }
            else // There is no hit
            {
                DeselectGO();
            }
        }
    }

    private void MoveToTarget()
    {
        float distance = Vector3.Distance(selectedGO.transform.position, transform.position);
        if (distance > 3.0f)
        {
            transform.position = Vector3.Lerp(transform.position, selectedGO.transform.position, 0.1f);
        }
        else
        {
            moving = false;
        }
    }

    private void DeselectGO()
    {
        selectedGO = null;
        gameObject.transform.parent = null;
        UIScript.DeselectAnimal();
    }
}