using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using static GameManager;   
public class MyButton : MonoBehaviour
{
    public enum MyButtonOption
    {
        None,
        Check,
        Store,
        Option,
        TurnEnd,
        EndClose
    }
    public enum Store
    {
        None,
        Knife,
        Pistol,
        Sniper,
        MoveDistanceUp,
        Schrotflinte
    }
    public enum Option
    {
        None,
        Rank,
        Again,
        Close
    }
    Button button;

    public MyButtonOption Mybutton;
    public Store store;
    public Option option;
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "GameScene") return;
        
        if (MyButtonOption.TurnEnd == Mybutton)
        {
            if (Instance.ButtonLock)
            {
                button.interactable = false;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    GameManager.Instance.FromPlayerToMonster();
                }
                button.interactable = true;
            }
        }
    }
    public void OnClick()
    {
        switch (Mybutton)
        {
            case MyButtonOption.None:
                {
                    break;
                }
            case MyButtonOption.Store:
                {
                    switch (store)
                    {
                        case Store.None:
                            {
                                break;
                            }
                        case Store.Knife:
                            {
                                //ReadyToMount(Prefabs.Knife, new Vector3(0.082f, -0.02f, 0), 0, 90);
                                break;
                            }
                        case Store.Pistol:
                            {
                                ReadyToMount(PoolManager.Prefabs.Pistol, new Vector3(0.045f, 0.025f, 0), 100);
                                break;
                            }
                        case Store.Sniper:
                            {
                                ReadyToMount(PoolManager.Prefabs.Sniper, new Vector3(-0.013f, 0.019f, 0), 200);
                                break;
                            }
                        case Store.MoveDistanceUp:
                            {
                                ReadyToMount(PoolManager.Prefabs.MoveDistanceUp, new Vector3(0.026f, 0.178f, 0), 200);
                                break;
                            }
                        case Store.Schrotflinte:
                            {
                                ReadyToMount(PoolManager.Prefabs.Schrotflinte, new Vector3(0.0552f, 0, 0), 200);
                                break;
                            }
                        default:
                            break;
                    }
                    break;
                }
            case MyButtonOption.Check:
                {
                    UiManager.Instance.affiliation = GameObject.Find("Affiliation").GetComponent<TMP_InputField>();
                    UiManager.Instance.guestName = GameObject.Find("GuestName").GetComponent<TMP_InputField>();
                    UiManager.Instance.PlayerName = UiManager.Instance.guestName.text;
                    UiManager.Instance.PlayserAffiliation = UiManager.Instance.affiliation.text;
                    if(GameManager.Instance != null)
                    {
                        GameManager.Instance.Reset();
                    }
                    SceneManager.LoadScene("GameScene");
                    break;
                }
            case MyButtonOption.Option:
                {
                    switch (option)
                    {
                        case Option.None:
                            {
                                break;
                            }
                        case Option.Rank:
                            {
                                break;
                            }
                        case Option.Again:
                            {
                                GameManager.Instance.Reset();
                                SceneManager.LoadScene("GameScene");
                                UiManager.Instance.Reset();
                                break;
                            }
                        case Option.Close:
                            {
                                #if UNITY_EDITOR
                                    UnityEditor.EditorApplication.isPlaying = false;
                                #else
                                    Application.Quit();
                                #endif
                                break;
                            }
                        default:
                            break;
                    }
                    break;
                }
            case MyButtonOption.TurnEnd:
                {
                    GameManager.Instance.FromPlayerToMonster();
                    break;
                }
            case MyButtonOption.EndClose:
                {
                    if (GameManager.Instance.player.Die)
                    {
                        GameManager.Instance.Reset();
                        UiManager.Instance.Reset();
                        SceneManager.LoadScene("MainScene");
                    }
                    break;
                }
            default:
                break;
        }

    }
    private void ReadyToMount(PoolManager.Prefabs prefabs, Vector3 skillPosition, int cost = 0, float skillRotation = 0)
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
                    if (skillRotation != 0)
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