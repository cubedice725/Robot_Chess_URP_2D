using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    private PlayerMovement playerMovement;
    public int Hp = 1;
    public int MoveCount { get; set; } = 0;
    public int AttackCount { get; set; } = 0;
    // �̵��Ÿ�
    public int MoveDistance { get; set; } = 1;
    // �̵� �ӵ�
    public float PlayerMoveSpeed { get; set; } = 1f;
    private bool start = true;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerStateMachine = new PlayerStateMachine(playerMovement);
        playerStateMachine.Initialize(playerStateMachine.playerIdleState);
    }
    private void Start()
    {
        Instance.Map2D[(int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y)] = (int)MapObject.player;
    }
    private void Update()
    {
        if(start && Hp <= 0)
        {
            GetComponent<Animator>().SetTrigger("die");
            start = false;
        }
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
            
            Instance.playerState = State.Move;
            Instance.hit.name = "";

        }
    }
}