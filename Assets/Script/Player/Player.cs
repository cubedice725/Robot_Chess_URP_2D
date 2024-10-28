using UnityEngine;
using static GameManager;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerStateMachine = new PlayerStateMachine(playerMovement);
        playerStateMachine.Initialize(playerStateMachine.playerIdleState);
    }
    private void Update()
    {
        //���� ���¸� ���� ������ �ٲ���
        if (Instance.playerState == State.Idle) 
        {
            playerStateMachine.TransitionTo(playerStateMachine.playerIdleState);
        }
        else if (Instance.playerState == State.Move)
        {
            playerStateMachine.TransitionTo(playerStateMachine.playerMovingState);
        }
        else if (Instance.playerState == State.Skill)
        {
            playerStateMachine.TransitionTo(playerStateMachine.playerSkillCastingState);
        }
        playerStateMachine.PlayerStateMachineUpdate();
    }
}