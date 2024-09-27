using UnityEngine;

public class SkillBasicObject : SkillObject
{
    private void Update()
    {
        transform.Translate(Direction * 7f * Time.deltaTime);
    }
    public void OnCollisionEnter(Collision collision)
    {
        print(collision.transform.name);
        Destroy();
    }
}
