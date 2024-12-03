using UnityEngine;

public class AK47Bullet : MonoBehaviour
{
    float speed = 10;
    public bool isMonster = true;
    void Update()
    {
        transform.Translate(speed * Time.deltaTime, 0,0);
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isMonster)
        {
            try
            {
                if (collision.gameObject.GetComponent<Monster>() != null)
                {
                    collision.gameObject.GetComponent<Monster>().Hp -= 1;
                    GetComponent<MyObject>().Destroy();

                }
            }
            catch { }
        }
        if (isMonster)
        {
            if (collision.gameObject.GetComponent<Player>() != null)
            {
                collision.gameObject.GetComponent<Player>().Hp -= 1;
                GetComponent<MyObject>().Destroy();
            }
        }
    }
}
