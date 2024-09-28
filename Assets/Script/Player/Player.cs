using UnityEngine;
using static GameManager;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    private PlayerMovement playerMovement;
    public PlayerState playerState = PlayerState.Idle;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerStateMachine = new PlayerStateMachine(playerMovement);
        playerStateMachine.Initialize(playerStateMachine.playerIdleState);
    }
    private void LateUpdate()
    {
        // �÷��̾� ���� �ƴϸ� �۵�����
        if (!Instance.playerTurn)
            return;

        //���� ���¸� ���� ������ �ٲ���
        if (Instance.playerState == PlayerState.Idle) 
        {
            playerStateMachine.TransitionTo(playerStateMachine.playerIdleState);
        }
        else if (Instance.playerState == PlayerState.Move)
        {
            playerStateMachine.TransitionTo(playerStateMachine.playerMovingState);
        }
        else if (Instance.playerState == PlayerState.Skill)
        {
            playerStateMachine.TransitionTo(playerStateMachine.playerSkillCastingState);
        }
        playerStateMachine.PlayerStateMachineUpdate();
    }
}