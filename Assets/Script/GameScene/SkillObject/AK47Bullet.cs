using UnityEngine;

public class AK47Bullet : MonoBehaviour
{
    float speed = 20;
    void FixedUpdate()
    {
        transform.Translate(speed * Time.fixedDeltaTime, 0,0);
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.transform.name);
        if (collision != null && collision.transform.name != "Player")
        {
            if(collision.gameObject.GetComponent<Monster>() != null)
            {
                collision.gameObject.GetComponent<Monster>().HP -= 1;
            }
            GameManager.Instance.MyObjectActivate = false;
            GetComponent<MyObject>().Destroy();
        }
    }
}
