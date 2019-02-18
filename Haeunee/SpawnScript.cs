using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//적, 캐릭터를 스폰시키는 스크립트
public class SpawnScript : MonoBehaviour {
    public GameObject player;
    public GameObject enemy;
    string playerPname;
    string enemyPname;
    bool spawnPoss = false;
    public GameObject nowEnemy;
    GameObject nowPlayer;
    sCharInfo enemyInfo;
    GameEnterScript enter;

    void Start () {
        enter = GetComponent<GameEnterScript>();
    }

    private void Update()
    {
        if(spawnPoss==true) //스폰될 정보를 받았을 때
        {
            StartCoroutine(ItemDelay()); //두 유저가 모두 정보를 주고 받을 때까지 기다리는 코루틴
            spawnPoss = false;
        }
    }

    void Spawn()
    {//적과 내 캐릭터 스폰시키기
        Quaternion q = new Quaternion(0, 0, 0, 0);
        GameObject playerP = GameObject.Find(playerPname);
        GameObject enemyP = GameObject.Find(enemyPname);

        nowPlayer = Instantiate(player, playerP.transform.position, q, playerP.transform);
        nowEnemy = Instantiate(enemy, enemyP.transform.position, q, enemyP.transform);
        
        Custom(nowPlayer, enter.savCharInfo);
        Custom(nowEnemy, enemyInfo);

        Camera mainCam = Camera.main; //내 캐릭터가 가진 카메라를 화면에 띄우기
        mainCam.enabled = false;
        Camera playerCam = nowPlayer.GetComponentInChildren<Camera>();
        playerCam.enabled = true;
    }

    public void SpawnInfo(string playerParent, string enemyParent, sCharInfo charInfo)
    { //스폰될 정보를 받아오는 함수
        playerPname = playerParent;
        enemyPname = enemyParent;
        enemyInfo = charInfo;
        spawnPoss = true;
    }

    void Custom(GameObject nowObj, sCharInfo charInfo) //정보에 따라 캐릭터를 커스텀하는 함수
    {
        HeroCustomize custom = nowObj.GetComponent<HeroCustomize>();
        AnimationController aniMgr = nowObj.GetComponent<AnimationController>();
        string genderStr = "";
        if (charInfo.gender == 1)
            genderStr = "Male";
        else if (charInfo.gender == 2)
            genderStr = "Female";
        custom.Gender = genderStr;
        custom.IndexWeapon.CurrentType = charInfo.weapon;
        custom.IndexWeapon.CurrentIndex = 3;
        custom.IndexSuit.CurrentIndex = charInfo.cloth;
        custom.IndexHair.CurrentIndex = charInfo.hair;
        custom.IndexColorHair.CurrentIndex = charInfo.hairColor;
        custom.IndexFace.CurrentIndex = charInfo.face;
        custom.UpdateVisual();
        custom.UpdateWeapon();
        aniMgr.weaponIndex = charInfo.weapon; //무기에 따른 애니메이션도 설정
    }

    IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Spawn();
    }

    IEnumerator ItemDelay() 
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("GameScene");
        StartCoroutine(SpawnCoroutine()); //게임 씬이 모두 로드될 때까지 기다리는 코루틴
    }
}
