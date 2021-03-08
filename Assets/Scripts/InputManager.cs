using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public Transform PlayerCamera;
    public float MoveSpeed;
    public KeyCode UseKey;
    public KeyCode SprintKey;
    public KeyCode InventoryOpenCloseKey;

    [HideInInspector]
    public bool CanPickUpAnItem;
    [HideInInspector]
    public bool CanMove;

    private float currentMoveSpeed;
    private Rigidbody myRigidbody;
    private MouseLookAt mouseLookAt;

    void Start()
    {
        Instance = this;
        CanMove = true;
        myRigidbody = gameObject.GetComponent<Rigidbody>();
        mouseLookAt = gameObject.GetComponentInChildren<MouseLookAt>();
    }

    private void LateUpdate()
    {
        if (CanPickUpAnItem)
        {
            if (Input.GetKeyDown(UseKey))
            {
                InventoryManager.Instance.TryPickUpCurrentItem();
            }
        }
        if (Input.GetKeyDown(InventoryOpenCloseKey))
        {
            if (UIManager.Instance.InventoryUI.activeSelf)
            {
                UIManager.Instance.CloseMainInventory();
                CanMove = true;
                mouseLookAt.CanLook = true;
            }
            else
            {
                if (UIManager.Instance.inventoryIsReadyToOpenAgain)
                {
                    UIManager.Instance.OpenMainInventory();
                    CanMove = false;
                    mouseLookAt.CanLook = false;
                }
            }
        }

    }

    void FixedUpdate()
    {

        if (CanMove)
        {

            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentMoveSpeed = MoveSpeed * 2;
            }
            else
            {
                currentMoveSpeed = MoveSpeed;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    //diagonal force correction
                    currentMoveSpeed = currentMoveSpeed / 2;
                }
            }


            Vector3 horizontalAxis = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            Vector3 cameradirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 actualDirection = PlayerCamera.TransformDirection(cameradirection);
            Vector3 actualHorizontalAxis = PlayerCamera.TransformDirection(horizontalAxis);
            actualDirection.y = Mathf.Clamp(actualDirection.y, 0, 0);

            if (Input.GetKey(KeyCode.W))
            {
                myRigidbody.AddForce(actualDirection * currentMoveSpeed, ForceMode.VelocityChange);
            }
            if (Input.GetKey(KeyCode.S))
            {
                myRigidbody.AddForce(-actualDirection * -currentMoveSpeed, ForceMode.VelocityChange);
            }
            if (Input.GetKey(KeyCode.A))
            {
                myRigidbody.AddForce(actualHorizontalAxis * currentMoveSpeed, ForceMode.VelocityChange);
            }
            if (Input.GetKey(KeyCode.D))
            {
                myRigidbody.AddForce(actualHorizontalAxis * currentMoveSpeed, ForceMode.VelocityChange);
            }
        }



    }
}
