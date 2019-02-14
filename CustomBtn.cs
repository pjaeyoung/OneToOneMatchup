using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBtn : MonoBehaviour {
    HeroCustomize heroCustomize;

    void Start () {
        heroCustomize = GetComponent<HeroCustomize>();
        StartCoroutine(HideWeapon());
    }
	
    public void MaleBtn()
    {
        heroCustomize.Gender = "Male";
        heroCustomize.UpdateVisual();
    }

    public void FemaleBtn()
    {
        heroCustomize.Gender = "Female";
        heroCustomize.UpdateVisual();
    }

    public void HairBtn()
    {
        heroCustomize.IndexHair.CurrentIndex++;

        if (heroCustomize.IndexHair.CurrentIndex > heroCustomize.IndexHair.MaxIndex)
            heroCustomize.IndexHair.CurrentIndex = 0;

        heroCustomize.UpdateVisual();
    }

    public void HairColorBtn()
    {
        heroCustomize.IndexColorHair.CurrentIndex++;

        if (heroCustomize.IndexColorHair.CurrentIndex > heroCustomize.IndexColorHair.MaxIndex)
            heroCustomize.IndexColorHair.CurrentIndex = 0;

        heroCustomize.UpdateVisual();
    }

    public void FaceBtn()
    {
        heroCustomize.IndexFace.CurrentIndex++;

        if (heroCustomize.IndexFace.CurrentIndex > heroCustomize.IndexFace.MaxIndex)
            heroCustomize.IndexFace.CurrentIndex = 0;

        heroCustomize.UpdateVisual();
    }

    IEnumerator HideWeapon()
    {
        yield return new WaitForSeconds(0.01f);
        heroCustomize.HideWeapon();
    }
}
