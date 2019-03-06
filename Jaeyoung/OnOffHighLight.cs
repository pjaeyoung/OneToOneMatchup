using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnOffHighLight : MonoBehaviour
{
    public int IsHighlight; // 하이라이트박스 내에 있으면 TRUE, 아니면 FALSE

    /* 아이템 필드 상 게임오브젝트 외곽선 하이라이트 표시 : OnTriggerEnter, OnTriggerExit, OnOffHighlight */

    private void OnTriggerEnter(Collider other) //chkHighlightBox에 부딪친 오브젝트 중 touchable layer인 경우에만 하이라이트 표시 
    {
        GameObject obj = other.gameObject;

        if (obj.layer == (int)eLAYER.TOUCHABLE)
        {
            Debug.Log("triggerEnter");
            IsHighlight = (int)eBOOLEAN.TRUE;
            OnOffHighlight(obj, IsHighlight);
        }

    }

    private void OnTriggerExit(Collider other) //chkHighlightBox에서 벗어나면 하이라이트 해제
    {
        GameObject obj = other.gameObject;
        if (obj.layer == (int)eLAYER.TOUCHABLE)
        {
            IsHighlight = (int)eBOOLEAN.FALSE;
            OnOffHighlight(obj, IsHighlight);
        }
    }

    /* 아이템 필드에서 외곽선 하이라이트 표시 여부 결정 */
    void OnOffHighlight(GameObject hitObj, int IsHighlight)
    {
        if (IsHighlight == (int)eBOOLEAN.TRUE)
            hitObj.GetComponentInChildren<outline>().OutlineMode = outline.Mode.OutlineVisible;
        else
            hitObj.GetComponentInChildren<outline>().OutlineMode = outline.Mode.OutlineHidden;
    }
}
