using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    private Player player;

    private float scrollSpeed = 2000.0f;
    private float mouseSpeed = 100f;
    private float scrollWheel;
    private void Awake()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        player = FindObjectOfType<Player>();
    }
    private void Update()
    {
        // 화면 따라가기
        if (Input.GetKeyDown("space"))
        {
            cam.Follow = player.transform;
        }
    }
    private void LateUpdate()
    {
        CameraMove();

        if (0f != Input.GetAxis("Mouse ScrollWheel"))
        {
            ZoomInOut();
        }
    }
    // 카메라 움직임
    private void CameraMove()
    {
        Vector3 mousePosition = Input.mousePosition;

        // 좌측
        if (mousePosition.x <= 0)
        {
            CamFollowNull();
            cam.transform.Translate(-Time.deltaTime * mouseSpeed, 0, 0);
        }
        else if (mousePosition.x <= 50)
        {
            CamFollowNull();
            cam.transform.Translate(-Time.deltaTime * mouseSpeed / 10, 0, 0);
        }
        //우측
        else if (mousePosition.x >= Screen.width - 1)
        {
            CamFollowNull();
            cam.transform.Translate(Time.deltaTime * mouseSpeed, 0, 0);
        }
        else if (mousePosition.x >= Screen.width - 50)
        {
            CamFollowNull();
            cam.transform.Translate(Time.deltaTime * mouseSpeed / 10, 0, 0);
        }
        //아래
        if (mousePosition.y <= 0)
        {
            CamFollowNull();
            cam.transform.Translate(0, -Time.deltaTime * mouseSpeed, 0);
        }
        else if (mousePosition.y <= 50)
        {
            CamFollowNull();
            cam.transform.Translate(0, -Time.deltaTime * mouseSpeed/10, 0);
        }
        // 위
        else if (mousePosition.y >= Screen.height - 1)
        {
            CamFollowNull();
            cam.transform.Translate(0, Time.deltaTime * mouseSpeed, 0);
        }
        else if (mousePosition.y >= Screen.height - 50)
        {
            CamFollowNull();
            cam.transform.Translate(0, Time.deltaTime * mouseSpeed/10, 0);
        }
    }

    // 카메라 줌인, 줌아웃
    private void ZoomInOut()
    {
        CamFollowNull();
        scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        
        
        cam.m_Lens.OrthographicSize += -scrollWheel * Time.deltaTime * scrollSpeed;
        
        if (cam.m_Lens.OrthographicSize < 3)
        {
            cam.m_Lens.OrthographicSize = 3;
        }
        if (cam.m_Lens.OrthographicSize > 7)
        {
            cam.m_Lens.OrthographicSize = 7;
        }
    }

    // 카메라 따라가는거 중지
    private void CamFollowNull()
    {
        if (cam.Follow != null)
        {
            cam.Follow = null;
        }
    }
}
