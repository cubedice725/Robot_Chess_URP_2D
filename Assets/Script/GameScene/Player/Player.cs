using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    private PlayerMovement playerMovement;
    public int Hp { get; set; } = 1;
    public int MoveCount { get; set; } = 0;
    public int AttackCount { get; set; } = 0;
    // �̵��Ÿ�
    public int MoveDistance { get; set; } = 100;
    // �̵� �ӵ�
    public float PlayerMoveSpeed { get; set; } = 1f;
    public bool Die = false;
    public IState CurrentSkillState { get; private set; }

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
        if(!Die && Hp <= 0)
        {
            UiManager.Instance.SaveToCSV();
            GetComponent<Animator>().SetTrigger("die");
            Die = true;
        }
        if (Die)
        {
            Instance.ButtonLock = true;
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
            if (AttackCount != 1 || Instance.summonedSkill != null)
            {
                playerStateMachine.TransitionTo(playerStateMachine.playerSkillCastingState);
                if (CurrentSkillState != Instance.skillState)
                {
                    if(CurrentSkillState == null)
                    {
                        CurrentSkillState = Instance.skillState;
                    }
                    else
                    {
                        CurrentSkillState.Exit();
                        CurrentSkillState = Instance.skillState;
                        CurrentSkillState.Entry();
                    }
                }
            }
        }
        
        playerStateMachine.PlayerStateMachineUpdate();
        if (!Instance.playerTurn) return;
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            Instance.playerState = State.Move;
        }
        if (Instance.MyHit != null && Instance.MyHit.name.StartsWith("Player"))
        {
            Instance.playerState = State.Move;
            Instance.MyHit.name = "";
        }
    }
}