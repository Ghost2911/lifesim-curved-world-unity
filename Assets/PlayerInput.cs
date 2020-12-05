using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class PlayerInput : MonoBehaviour
{
    Transform cam;
    CharacterController cc;

    public Inventory inventory;
    public Vector3 CameraOffset = new Vector3(-5f, 2f, 0f);
    public WorldChanger wc;
    public HandTool handTool;
 
    public delegate void MainAction();
    public MainAction btnAction;
    public delegate void AdditiveAction();
    public AdditiveAction btnAdditiveAction;

    [Header("Player Settings")]
    public float playerSpeed = 4.0f;
    public float rotateSpeed = 10f;

    private Vector3 playerVelocity;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    [Header("Other Settings")]
    public FixedJoystick joystick;
    private Vector3 lastPosition;
    private Animator anim;
    private bool isMove;

    public Button btnMain;
    public Button btnAdditive;

    void Start()
    {
        cam = Camera.main.transform;
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 move = new Vector3(joystick.Horizontal * playerSpeed, 0f, joystick.Vertical * playerSpeed);
        //Vector3 move = new Vector3(Input.GetAxis("Horizontal") * playerSpeed, 0f, Input.GetAxis("Vertical") * playerSpeed);

        if (move != Vector3.zero)
        {
            if (!isMove)
            {
                isMove = true;
                anim.SetTrigger("isMove");
            }
            move = Quaternion.AngleAxis(cam.eulerAngles.y, Vector3.up) * move;
            lastPosition = transform.position;
            cc.Move(move * Time.deltaTime);
            anim.SetFloat("MoveSpeed", move.magnitude);
            cc.transform.forward = Vector3.Lerp(cc.transform.forward, move, Time.deltaTime * rotateSpeed);
            cam.position = cc.transform.position + CameraOffset;
        }
        else
        {
            if (isMove)
            {
                isMove = false;
                anim.SetTrigger("isIdle");
            }
        }
        
    }

    public void MainActionUse()
    {
        if (btnAction != null)
            btnAction.Invoke();
        else
            anim.SetTrigger("isAttack");
    }

    public void AdditiveActionUse()
    {
        if (btnAdditiveAction != null)
            btnAdditiveAction.Invoke();
    }

    public void TakeObject()
    {
        Vector3 placePos = cc.transform.position + 1.5f * cc.transform.forward;
        wc.TakeObject(handTool.type, Mathf.RoundToInt(placePos.x), Mathf.RoundToInt(placePos.z));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            HandTool ht = other.GetComponent<HandTool>();
            handTool.type = ht.type;
            btnMain.GetComponent<Image>().sprite = ht.icon;
            handTool.GetComponent<MeshFilter>().sharedMesh = (Mesh)other.GetComponent<MeshFilter>().sharedMesh;
            anim.SetFloat("HandTool",Convert.ToInt32(handTool.type));
        }
    }

}
