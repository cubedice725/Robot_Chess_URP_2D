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
        //현재 상태를 통해 동작을 바꿔줌
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
        // 플레이어는 스킬을 사용하기 위해 무언가를 착용했는 의미로 자식 오브젝트가 있는지 확인
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