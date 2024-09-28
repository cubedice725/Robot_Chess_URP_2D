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
    public override void Entry()
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
    }
    public override void Exit()
    {
        playerMovement.RemoveSkillSelection();
    }
}
