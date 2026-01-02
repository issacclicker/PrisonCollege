using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어의 물건 집고 던지는 행동 스크립트
public class ProfessorGrab : MonoBehaviour
{
    ////디버그용 임시 이동함수
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float gravity = -9.81f;

    CharacterController controller;
    Camera playerCamera;

    float xRotation = 0f;
    Vector3 velocity;


    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 카메라 회전 (상하)
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // 플레이어 회전 (좌우)
        transform.Rotate(Vector3.up * mouseX);
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 땅에 닿았을 경우 중력 초기화
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }
    ////
    

    //// 함수 본문
    
    RaycastHit hit; //집는 물체

    [Tooltip("집을 수 있는 거리")] [SerializeField]private float maxDistance = 5f; //집을 수 있는 거리
    [Tooltip("집고 있는 거리")] [SerializeField]private float holdDistance = 2f; //집고 있는 거리

    [Tooltip("집는 물건이 따라오는 속도")] [SerializeField]private float holdSmooth = 13f; // 집는 물건이 따라오는 속도

    [Tooltip("물건을 던지는 힘")] [SerializeField]private float throwForce = 10f;

    private bool allowsGrab; //물건을 집을 수 있는지 여부
    private bool isGrapping; //물건을 집는 중인지 여부

    void Start()
    {
        //디버그
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        //

        allowsGrab = true;
    }

    void Update()
    {
        //F를 누르면 물건 집는 검사
        if (!isGrapping & Input.GetKey(KeyCode.F) & allowsGrab)
        {
            // 카메라 기준으로 Raycast
            Transform cam = Camera.main.transform;

            Debug.DrawRay(cam.position, cam.forward * maxDistance, Color.blue, 0.3f);

            allowsGrab = false;

            if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance))
            {
                Debug.Log($"Hit object: {hit.collider.gameObject.name}");
                if(hit.collider.gameObject.tag == "Prop") // 집으려는 물건이 Prop인 경우
                {
                    isGrapping = true;
                    Debug.Log($"isGrapping : {isGrapping}");
                }
                else // 집으려는 물건이 Prop이 아닌경우 
                {
                    //잘못됐다는 VFX 재생
                }
                IEnumerator GrapDelay()
                {
                    yield return new WaitForSeconds(0.5f);
                    allowsGrab = true;
                }
                if(!isGrapping) StartCoroutine(GrapDelay());
            }
        }

        if (isGrapping)
        {
            Rigidbody targetR = hit.collider.gameObject.GetComponent<Rigidbody>(); // 집은 물건

            Vector3 targetPos = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
            // Debug.Log(hit.collider.gameObject.name);
            targetR.useGravity = false;
            targetR.MovePosition(Vector3.Lerp(targetR.position, targetPos, Time.deltaTime * holdSmooth));
            
            //클릭 해 던짐
            if(Input.GetKey(KeyCode.Mouse0))
            {
                isGrapping = false;
                allowsGrab = true;
                targetR.useGravity = true;

                targetR.AddForce(playerCamera.transform.forward * throwForce, ForceMode.VelocityChange);
            }

        }

        // 디버그용
        LookAround();
        MovePlayer();
    }

    
}
