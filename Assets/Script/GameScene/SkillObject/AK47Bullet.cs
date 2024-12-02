using UnityEngine;

public class AK47Bullet : MonoBehaviour
{
    float speed = 15;
    public bool isMonster = true;
    void FixedUpdate()
    {
        transform.Translate(speed * Time.fixedDeltaTime, 0,0);
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (isMonster)
        {
            try
            {
                if (collision.gameObject.GetComponent<Player>() != null)
                {
                    collision.gameObject.GetComponent<Player>().Hp -= 1;
                    GetComponent<MyObject>().Destroy();
                }
            }
            catch { }
        }
        else if (!isMonster)
        {
            if (collision != null && collision.transform.name != "Player")
            {
                if (collision.gameObject.GetComponent<Monster>() != null)
                {
                    collision.gameObject.GetComponent<Monster>().Hp -= 1;
                }
                GetComponent<MyObject>().Destroy();
            }
        }
    }
}
