using UnityEngine;

public class PlayerMovingState : IState
{
    private PlayerMovement _playerMovement;
    private RaycastHit hit;

    public PlayerMovingState(PlayerMovement playerMovement)
    {
        _playerMovement = playerMovement;
    }
    public void Enter()
    {
        _playerMovement.SetPlayerPlane();
    }
    public void IStateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ���� ī�޶� ���� ���콺 Ŭ���� ���� ray ������ ������
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                if (hit.transform.name.StartsWith("PlayerMovePlane"))
                {
                    _playerMovement.Hit = hit;
                    _playerMovement.RemovePlayerPlane();
                    _playerMovement.Move();
                    GameManager.Instance.playerState = GameManager.PlayerState.Idle;
                    GameManager.Instance.FromPlayerToMonster();
                }
            }
        }
    }
    public void Exit()
    {
        _playerMovement.RemovePlayerPlane();
    }
}
