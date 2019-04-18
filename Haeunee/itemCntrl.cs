using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class itemCntrl : MonoBehaviour
{
    PlayerScript s_player;
    GameObject highlightBox;
    GameObject hitEffect;
    outline s_outline;
    outline[] s_outline2;
    Scene scene;
    AttackMgr atkMgr;
    bool IsInHighlightBox = false; //아이템이 하이라이트 박스과 충돌했는지 여부 
    public bool isDestroyOK = false;

    private void Awake()
    {
        scene = SceneManager.GetActiveScene();
        if(scene.name == "ItemCollectScene")
        {
            highlightBox = GameObject.Find("Player").transform.Find("chkHighlight").gameObject;
        }
        else if (scene.name == "GameScene")
        {
            hitEffect = GameObject.Find("HitEffect");
            s_player = SocketServer.SingleTonServ().NowPlayerScript();
            highlightBox = s_player.gameObject.transform.Find("chkHighlight").gameObject;
        }
        highlightBox.SetActive(true);
        if(transform.name == "swordAndShield(Clone)") //swordAndShield 자식 오브젝트 둘 모두에게 적용 
            s_outline2 = transform.GetComponentsInChildren<outline>(); 
        else
            s_outline = transform.GetComponentInChildren<outline>();
    }

    private void Update()
    {
        /* 하이라이트박스가 출력 중이고 하이라이트박스 안에 있을 때만 아이템 outline 출력 */
        if (highlightBox.activeSelf == false || IsInHighlightBox == false)
        {
            if (transform.name == "swordAndShield(Clone)")
            {
                s_outline2[0].enabled = false;
                s_outline2[1].enabled = false;
            }
            else
                s_outline.enabled = false;
        }
        else if (highlightBox.activeSelf == true && IsInHighlightBox == true)
        {
            if (transform.name == "swordAndShield(Clone)")
            {
                s_outline2[0].enabled = true;
                s_outline2[1].enabled = true;
            }
            else
                s_outline.enabled = true;
        }
        if (transform.position.y < -2)
            transform.position += Vector3.up * 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        GameObject obj = other.gameObject;
        if (obj == highlightBox)
            IsInHighlightBox = true;

        if(scene.name == "GameScene")
        {
            /* 아이템을 들어올린 후 던진 뒤에만 floor 충돌 체크 */
            if (isDestroyOK == true)
            {
                if(obj.tag=="Shootable"||obj.tag=="floor"|| obj.tag == "Enemy"|| obj.tag == "Player")
                {
                    hitEffect.transform.position = transform.position;
                    highlightBox.SetActive(true);
                    Destroy(gameObject);
                    isDestroyOK = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj == highlightBox)
            IsInHighlightBox = false; 
    }
}
