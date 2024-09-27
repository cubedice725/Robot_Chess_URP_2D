using UnityEngine;

public class SkillBasic : Skill
{
    RaycastHit hit;
    bool start;
    protected override void Awake()
    {
        prefabObject = "Prefab/Skill/SkillBasicObject";
        base.Awake();
    }
    public override void Enter()
    {
        playerMovement.SetSkillSelection();
    }
    public override void IStateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ���� ī�޶� ���� ���콺 Ŭ���� ���� ray ������ ������
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                if (hit.transform.name.StartsWith("SkillSelection"))
                {
                    playerMovement.RemoveSkillSelection();
                    start = true;
                }
            }
        }
        if (start)
        {
            Skillcasting();
        }
    }
    public void Skillcasting()
    {
        if (!playerMovement.UpdateLooking(hit.transform.position))
        {
            start = false;
            var skillObject = skillObjectPool.Get();
            skillObject.transform.position = transform.TransformDirection(Vector3.forward) + transform.position;
            skillObject.Direction = transform.TransformDirection(Vector3.forward);
            skillObject.gameObject.SetActive(true);
            GameManager.Instance.playerState = GameManager.PlayerState.Idle;
            GameManager.Instance.FromPlayerToMonster();
        }
    }
    public override void Exit()
    {
        playerMovement.RemoveSkillSelection();
    }
}
