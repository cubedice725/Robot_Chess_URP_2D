using UnityEngine;
using static GameManager;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    private PlayerMovement playerMovement;

    public int MoveCount { get; set; } = 0;
    public int AttackCount { get; set; } = 0;
    // �̵��Ÿ�
    public int MoveDistance { get; set; } = 1;
    // �̵� �ӵ�
    public float PlayerMoveSpeed { get; set; } = 1f;
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
            if(MoveCount != 1)
            {
                playerStateMachine.TransitionTo(playerStateMachine.playerMovingState);
            }
        }
        else if (Instance.playerState == State.Skill)
        {
            if (AttackCount != 1)
            {
                playerStateMachine.TransitionTo(playerStateMachine.playerSkillCastingState);
            }
        }
        

        playerStateMachine.PlayerStateMachineUpdate();
        if (Instance.hit != null && Instance.hit.name == "Player")
        {
            if (!Instance.playerTurn) return;
            Instance.hit.name = "";
            Instance.playerState = State.Move;
        }
    }
}