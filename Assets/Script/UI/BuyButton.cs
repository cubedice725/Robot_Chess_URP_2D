using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static GameManager;
using static PoolManager;
public class BuyButton : MonoBehaviour
{
    public enum Item
    {
        Knife,
        Pistol,
        Sniper
    }
    public Item item;
    public void OnClickBuy()
    {
        
        switch (item)
        {
            case Item.Knife:
                {
                    //ReadyToMount(Prefabs.Knife, new Vector3(0.082f, -0.02f, 0), 0, 90);
                    break;
                }
            case Item.Pistol:
                {
                    ReadyToMount(Prefabs.Pistol, new Vector3(0.045f, 0.025f, 0), 100);
                    break;
                }
            case Item.Sniper:
                {
                    ReadyToMount(Prefabs.Sniper, new Vector3(-0.013f, 0.019f, 0), 200);
                    break;
                }
            default:
                break;
        }
    }
    private void ReadyToMount(Prefabs prefabs, Vector3 skillPosition, int cost = 0, float skillRotation = 0)
    {
        int canBuy = Instance.GameScore - cost;
        if (canBuy >= 0)
        {
            MyObject myObject;

            if (Instance.player.transform.Find("MainSkill").transform.childCount == 0)
            {

                myObject = Instance.poolManager.SelectPool(prefabs).Get();
                myObject.transform.position = new Vector3(100, 100, 0);
                myObject.transform.parent = Instance.player.transform.Find("MainSkill").gameObject.transform;
                if (Mathf.Sign(Instance.player.transform.localScale.x) < 0)
                {
                    myObject.transform.localScale = new Vector3(-myObject.transform.localScale.x, myObject.transform.localScale.y, 0);
                    if(skillRotation != 0)
                    {
                        skillRotation -= 180;
                    }
                }
                myObject.transform.localPosition = skillPosition;
                myObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, skillRotation));
                Instance.GameScore -= cost;
            }
            else if (Instance.player.transform.Find("SideSkill").transform.childCount == 0)
            {
                myObject = Instance.poolManager.SelectPool(prefabs).Get();
                myObject.transform.position = new Vector3(100, 100, 0);
                myObject.transform.parent = Instance.player.transform.Find("SideSkill").gameObject.transform;
                if (Mathf.Sign(Instance.player.transform.localScale.x) < 0)
                {
                    myObject.transform.localScale = new Vector3(-myObject.transform.localScale.x, myObject.transform.localScale.y, 0);
                    if (skillRotation != 0)
                    {
                        skillRotation -= 180;
                    }
                }
                myObject.transform.localPosition = skillPosition;
                myObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, skillRotation));
                Instance.GameScore -= cost;
            }
            else
            {
                print("구매할수없음");
            }
        }
    }
}
