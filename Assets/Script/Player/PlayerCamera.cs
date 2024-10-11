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
        // ȭ�� ���󰡱�
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
    // ī�޶� ������
    private void CameraMove()
    {
        Vector3 mousePosition = Input.mousePosition;

        // ����
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
        //����
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
        //�Ʒ�
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
        // ��
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

    // ī�޶� ����, �ܾƿ�
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

    // ī�޶� ���󰡴°� ����
    private void CamFollowNull()
    {
        if (cam.Follow != null)
        {
            cam.Follow = null;
        }
    }
}
