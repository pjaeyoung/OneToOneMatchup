using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class itemCntrl : MonoBehaviour
{
    PlayerScript s_player;
    EnemyScript s_Enemy;
    GameObject highlightBox;
    outline s_outline;
    Scene scene;
    int IsInHighlightBox = (int)eBOOLEAN.FALSE; //아이템이 하이라이트 박스과 충돌했는지 여부 
    public bool isDestroyOK = false;
    AttackMgr atkMgr;

    private void Awake()
    {
        scene = SceneManager.GetActiveScene();
        if(scene.name == "ItemCollectScene")
        {
            highlightBox = GameObject.Find("Player").transform.Find("chkHighlight").gameObject;
        }
        else if (scene.name == "GameScene")
        {
            s_player = SocketServer.SingleTonServ().NowPlayerScript();
            s_Enemy = SocketServer.SingleTonServ().NowEnemyScript();
            atkMgr = s_Enemy.gameObject.GetComponent<AttackMgr>();
            highlightBox = s_player.gameObject.transform.Find("chkHighlight").gameObject;
        }
        highlightBox.SetActive(true);
        s_outline = transform.GetComponentInChildren<outline>();
    }

    private void Update()
    {
        /* 하이라이트박스가 출력 중일 때만 아이템 outline 출력 */
        if (highlightBox.activeSelf == false || IsInHighlightBox == (int)eBOOLEAN.FALSE)
            s_outline.enabled = false;
        else if (highlightBox.activeSelf == true && IsInHighlightBox == (int)eBOOLEAN.TRUE)
            s_outline.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        GameObject obj = other.gameObject;
        if (obj == highlightBox)
            IsInHighlightBox = (int)eBOOLEAN.TRUE;

        if(scene.name == "GameScene")
        {
            /* 아이템을 들어올린 후 던진 뒤에만 floor 충돌 체크 */
            if (isDestroyOK == true)
            {
                //펑 터지는 효과 애니메이션 실행
                if(obj.tag == "Enemy")
                {
                    atkMgr.HitSucc((int)eATKTYPE.em_OBJTHROW);
                }
                if(obj.tag=="Shootable"||obj.tag=="floor"|| obj.tag == "Enemy"|| obj.tag == "Player")
                {
                    Debug.Log("destroy");
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
            IsInHighlightBox = (int)eBOOLEAN.FALSE;
    }
}
