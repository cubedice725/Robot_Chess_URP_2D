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
        // 플레이어 턴이 아니면 작동안함
        if (!Instance.playerTurn)
            return;

        //현재 상태를 통해 동작을 바꿔줌
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