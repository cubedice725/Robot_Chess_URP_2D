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
        FindSkillObject();
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
    private void FindSkillObject()
    {
        // �÷��̾�� ��ų�� ����ϱ� ���� ���𰡸� �����ߴ� �ǹ̷� �ڽ� ������Ʈ�� �ִ��� Ȯ��
        if (transform.childCount != 0)
        {
            Instance.skillState = transform.GetChild(0).GetComponent<IState>();
            TransitionTo(Instance.skillState);
        }
    }
    public void TransitionTo(IState nextState)
    {
        if (nextState == Instance.skillState) return;
        Instance.skillState = nextState;
    }
}