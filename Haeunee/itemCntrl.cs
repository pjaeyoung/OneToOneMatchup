using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemCntrl : MonoBehaviour
{
    PlayerScript s_player;
    GameObject highlightBox;
    outline s_outline;

    private void Awake()
    {
        s_player = GameObject.Find("Player").GetComponent<PlayerScript>();
        highlightBox = GameObject.Find("Player").transform.Find("chkHighlight").gameObject;
        s_outline = transform.GetComponent<outline>();
    }

    private void Update()
    {
        if (highlightBox.activeSelf == false)
            s_outline.enabled = false;
        else
            s_outline.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.tag == "floor" && s_player.isDestroyOK == (int)eBOOLEAN.TRUE)
        {
            //펑 터지는 효과 애니메이션 실행
            s_player.isDestroyOK = (int)eBOOLEAN.FALSE;
            Debug.Log("destroy");
            Destroy(gameObject);
            highlightBox.SetActive(true);
        }
    }
}
