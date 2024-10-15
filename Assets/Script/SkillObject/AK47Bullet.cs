using UnityEngine;

public class AK47Bullet : MonoBehaviour
{
    MyObject MyObject;
    float speed = 30;
    private void Awake()
    {
        MyObject = GetComponent<MyObject>();
    }
    void Update()
    {
        transform.Translate(speed * Time.deltaTime,0,0);
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
            MyObject.Destroy();
        }
    }
}
