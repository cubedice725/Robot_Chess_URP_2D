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
            // 메인 카메라를 통해 마우스 클릭한 곳의 ray 정보를 가져옴
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
