using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;

public class tutorial : MonoBehaviour
{
    GameObject player;
    Rigidbody playerRigid;
    Camera camera;
    Quaternion playerRot;
    Quaternion CameraInitRot; //카메라 처음 회전 각도 
    Quaternion CameraPredictRot; //카메라 예측 회전 각도
    GameObject getItem; // 클릭한 아이템
    Button itemBtn;
    GameObject itemInfo; // tuto = 3 일 때 생성되는 아이템 정보 설명하는 창 
    GameObject targetZone;
    AnimationController animationCntrl;
    Slider hpBar;
    public GameObject sword;
    public GameObject wand;
    GameObject magicPrefab;
    GameObject point; // 사정거리 표시 
    GameObject nowShot;
    GameObject shotMgr;
    Ray shotRay;
    GameObject TextBtn;
    GameObject Enemy;
    GameObject hpPotion;
    GameObject beerBox;

    float moveSpeed = 5.0f;
    float sensibilityX = 2.0f;
    float sensibilityY = 2.0f;
    bool IsJump = false;

    float maxX = 100.0f; //targetZone 이동 범위 한계치
    float minX = -100.0f;
    float maxZ = 180.0f;
    float minZ = 0.0f;

    int weaponIdx = 0;
    int atkAni = 0;
    bool aniEnd = false;
    bool set = false;

    float maxDistance = 20; //point 한계치 
    Vector3 rayPoint;

    public int tuto = -1; //튜토리얼 chapter 번호 (0: 이동키/ 1: 회전 / 2: 아이템 획득/ 3: 아이템, 무기 종류 설명/ 4: 무기공격 / 5: 던지기 공격) 
    int readLine = -1;
    bool readOn = false;
    List<string> textList; //튜토리얼 설명 내용 
    int ActCount = 0; //튜토리얼에 따라 행동 수행 여부 측정 

    private void Awake()
    {
        getItem = null;
        textList = new List<string>();
        ReadData();
        player = GameObject.Find("Player");
        playerRigid = player.GetComponent<Rigidbody>();
        playerRot = player.transform.rotation;
        camera = player.transform.GetChild(6).GetComponent<Camera>();
        CameraInitRot = camera.transform.localRotation;
        CameraPredictRot = camera.transform.localRotation;
        itemBtn = GameObject.Find("Canvas/itemBtn").GetComponent<Button>();
        itemBtn.gameObject.SetActive(false);
        itemInfo = GameObject.Find("Canvas/itemInfo").gameObject;
        itemInfo.gameObject.SetActive(false);
        targetZone = player.transform.Find("targetZone").gameObject;
        animationCntrl = player.GetComponent<AnimationController>();
        magicPrefab = GameObject.Find("MagicPrefab");
        magicPrefab.SetActive(false);
        point = GameObject.Find("PointPrefab");
        point.SetActive(false);
        shotMgr = player.transform.GetChild(7).gameObject;
        TextBtn = GameObject.Find("Canvas").transform.Find("TextBtn").gameObject;
        TextBtn.SetActive(false);
        Enemy = GameObject.Find("enemy");
        hpBar = Enemy.transform.Find("enemyHp/Slider").GetComponent<Slider>();
        Enemy.SetActive(false);
        hpPotion = GameObject.Find("hpPotion");
        hpPotion.SetActive(false);
        beerBox = GameObject.Find("BeerBox_Green");
        beerBox.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(textBtnActive());
    }

    private void FixedUpdate()
    {
        animationCntrl.PlayAnimation("Idle");
        if (getItem == null && tuto != 6)
        {
            move();
            jump();
        }
        Rot();

        if (getItem != null && tuto == 5)
        {
            if (Input.GetMouseButtonDown(1) && getItem != null)
                ActiveTargetZone();
            else if (Input.GetMouseButton(1) && getItem != null)
                drawTargetZone();
            else if (Input.GetMouseButtonUp(1) && getItem != null)
            {
                TransferItem();
                getItem = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "floor")
            IsJump = false;
        if (other.tag == "item")
            other.GetComponent<outline>().enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "item")
            other.GetComponent<outline>().enabled = false;
    }

    void Update()
    {
        if (readOn == true)
        {
            readOn = false;
            int cpy = tuto;
            readLine++;
            showText(readLine);
            if (cpy != tuto)
            {
                ActCount = 0;
                if(tuto == 5)
                    set = false;
            }
        }

        if (tuto == 2)
        {
            if (hpPotion.activeSelf == false && itemBtn.gameObject.activeSelf == false)
            {
                itemBtn.gameObject.SetActive(true);
                hpPotion.SetActive(true);
            }
            if (Input.GetMouseButtonDown(0) && player.GetComponent<BoxCollider>().enabled == true)
            {
                player.transform.GetComponent<BoxCollider>().enabled = false;
                clickItem(2);
            }
            else if (Input.GetMouseButtonUp(0))
                player.GetComponent<BoxCollider>().enabled = true;
        }
        else if (tuto == 3)
        {
            if (hpPotion.activeSelf == true || itemBtn.gameObject.activeSelf == true)
            {
                itemBtn.gameObject.SetActive(false);
                hpPotion.SetActive(false);
                getItem = null;
            }
            if (itemInfo.activeSelf == false)
                itemInfo.SetActive(true);
        }
        else if (tuto == 4)
        {
            if (itemInfo.activeSelf == true)
                itemInfo.SetActive(false);
            if(set == false)
            {
                setWeapon();
                set = true;
            }
                Attack();
        }
        else if (tuto == 5)
        {
            if (set == false)
            {
                setWeapon();
                set = true;
            }
            Attack();
            drawPoint();
            if (aniEnd == true)
                Shot();
        }
        else if (tuto == 6)
        {
            if (Enemy.activeSelf == true)
                Enemy.SetActive(false);
            if (beerBox.activeSelf == false)
                beerBox.SetActive(true);
            if (Input.GetMouseButtonDown(1) && player.GetComponent<BoxCollider>().enabled == true)
            {
                player.transform.GetComponent<BoxCollider>().enabled = false;
                clickItem(5);
            }
            else if (Input.GetMouseButtonUp(1))
                player.GetComponent<BoxCollider>().enabled = true;
        }
    }

    void move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if(tuto == 0)
                ActCount = 1;
            animationCntrl.PlayAnimation("Move");
        }
    } // 전후좌우 움직임 

    void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsJump == false && transform.position.y < 2)
            {
                IsJump = true;
                playerRigid.AddForce(0, 300, 0, ForceMode.Acceleration);
            }
        }
    } // 점프

    void Rot()
    {
        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X") * sensibilityX;
            float mouseY = Input.GetAxis("Mouse Y") * sensibilityY;

            Quaternion rotY = Quaternion.Euler(Vector3.up * mouseX);
            playerRot *= rotY;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, playerRot, 6 * Time.deltaTime);

            Quaternion rotX = Quaternion.Euler(Vector3.left * mouseY);
            CameraPredictRot *= rotX;
            float angle = Quaternion.Angle(CameraInitRot, CameraPredictRot);
            if (angle >= 0 && angle <= 10)
                camera.transform.localRotation = Quaternion.Slerp(CameraInitRot, CameraPredictRot, 40);
            if (tuto == 1)
                ActCount = 1;
        }
    } // 상하좌우 회전 

    void setWeapon()
    {
        Enemy.SetActive(true);
        if (weaponIdx == (int)eWEAPON.em_GREATESWORD)
        {
            sword.SetActive(true);
            hpBar.maxValue = 3;
            hpBar.value = 3;
        }
        else if (weaponIdx == (int)eWEAPON.em_WAND)
        {
           
            sword.SetActive(false);
            magicPrefab.SetActive(true);
            wand.SetActive(true);
            hpBar.maxValue = 4;
            hpBar.value = 4;
        }
    } // tuto = 4 : sword,  tuto = 5 : wand

    void Attack() //공격
    {
        float distance = 0;
        if (tuto == 4)
            distance = Vector3.Distance(player.transform.position, Enemy.transform.position);
        else if (tuto == 5)
            distance = Vector3.Distance(magicPrefab.transform.position, Enemy.transform.position);

        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject != TextBtn)
        {
            string atkName = "";
            if (atkAni == 0)
                atkName = "Attack01";
            else if (atkAni == 1)
                atkName = "Attack02";
            else if (atkAni == 2)
                atkName = "Critical01";
            else if (atkAni == 3)
                atkName = "Critical02";

            if (weaponIdx == (int)eWEAPON.em_GREATESWORD)
            {
                animationCntrl.weaponIndex = (int)eWEAPON.em_GREATESWORD;
                animationCntrl.PlayAtkDmg(atkName);
            }
            else if (weaponIdx == (int)eWEAPON.em_WAND)
            {
                animationCntrl.weaponIndex = (int)eWEAPON.em_WAND;
                animationCntrl.PlayAtkDmg(atkName);
            }
            StartCoroutine(EndAni(animationCntrl.GetAniLength(atkName)));

           if(distance <= 0.8f)
            {
                hpBar.value -= 1;
                if (hpBar.value == 0)
                {
                    ActCount = 1;
                    Enemy.SetActive(false);
                }
            }

            atkAni++;
            if (atkAni >= 4)
                atkAni = 0;
        }
    }

    void clickItem(int _tuto)
    {
        Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(cameraRay, out rayHit))
        {
            if (rayHit.collider.tag == "item" && rayHit.collider.GetComponent<outline>().enabled)
            {
                getItem = rayHit.collider.gameObject;
                if (_tuto == 2)
                {
                    getItem.SetActive(false);
                    changeItemImg(getItem.name);
                }
                else if (_tuto == 5)
                {
                    getItem.transform.position = player.transform.position + Vector3.up * 2;
                    getItem.GetComponent<Rigidbody>().useGravity = false;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            player.transform.GetComponent<BoxCollider>().enabled = true;
        }
    } //아이템 outline활성화시 클릭 가능 

    void changeItemImg(string name)
    {
        Sprite spr = Resources.Load<Sprite>("Sprites/" + name);
        itemBtn.gameObject.GetComponent<Image>().sprite = spr;
    } // itemBtn 이미지 변경 

    public void ItemDragDrop()
    {
        changeItemImg("img_emty");
        if (getItem != null)
        {
            Vector3 pos = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2));
            if (pos.y < 0.4f)
                pos.y = 0.4f;
            getItem.transform.position = pos;
            getItem.SetActive(true);
            getItem = null;
            ActCount = 1;
        }
    } // itemBtn에 들어간 아이템 다시 화면 밖으로 내보내기 

    void ActiveTargetZone()
    {
        targetZone.SetActive(true);
    } //targetZone 활성화
     
    void drawTargetZone()
    {
        float x = Input.GetAxisRaw("Mouse X");
        float y = Input.GetAxisRaw("Mouse Y");
        Vector3 newPos = new Vector3(x, 0, y);
        Vector3 nowPos = targetZone.transform.localPosition;
        nowPos += newPos * 15;
        if (nowPos.x > maxX)
            nowPos.x = maxX;
        else if (nowPos.x < minX)
            nowPos.x = minX;
        if (nowPos.z > maxZ)
            nowPos.z = maxZ;
        else if (nowPos.z < minZ)
            nowPos.z = minZ;
        targetZone.transform.localPosition = nowPos;
    } //targetZone 마우스 움직임에 따라 이동

    void TransferItem()
    {
        getItem.transform.GetComponent<Rigidbody>().useGravity = true;
        Vector3 dir = targetZone.transform.position - transform.position;
        getItem.GetComponent<Rigidbody>().velocity = getItem.transform.TransformDirection(dir.x, 0, dir.z);
        targetZone.SetActive(false);
    } //targetZone이 마지막으로 그려진 위치에 아이템 던지기 

    void drawPoint() //point 보이기 
    {
        shotRay = new Ray();
        shotRay.origin = shotMgr.transform.position;
        shotRay.direction = -shotMgr.transform.forward;
        RaycastHit rayHit;
        if(Physics.Raycast(shotRay, out rayHit, maxDistance))
        {
            if(point !=null && (rayHit.collider.tag == "Shootable"))
            {
                point.SetActive(true);
                point.transform.position = rayHit.point;
                point.transform.localRotation = Quaternion.FromToRotation(Vector3.up, rayHit.normal);
            }
        }
        else if (point != null)
        {
            point.SetActive(false);
        }
    }

    void Shot() // MagicPrefab 쏘기 
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject != TextBtn)
        {
            nowShot = Instantiate(magicPrefab, shotMgr.transform.position, Quaternion.identity);
            nowShot.transform.GetChild(0).transform.eulerAngles = GetComponentInParent<Transform>().eulerAngles;
            rayPoint = shotRay.direction * 10;
        }
    }

    public void ReadOnTrue() //텍스트 넘길 수 있는 조건
    {
        if(EventSystem.current.currentSelectedGameObject == TextBtn)
        {
            Debug.Log("ReadOnTrue");
            if (tuto == -1)
                readOn = true;
            else
            {
                int pass = int.Parse(textList[readLine].Substring(1, 1));
                if (pass == 1 || ActCount >= 1)
                {
                    readOn = true;
                    pass = 0;
                }
                else
                    readOn = false;
            }
        }
    }

    void ReadData() // 텍스트 외부 파일 읽기
    {
        TextAsset data = Resources.Load<TextAsset>("Data");
        StringReader sr = new StringReader(data.text);
        while(sr.Peek() > -1)
        {
            string line = sr.ReadLine();
            textList.Add(line);
        }
        sr.Close();
    }
    
    void showText(int idx) // 리스트에 저장된 텍스트 출력 
    {
        Text text = TextBtn.GetComponentInChildren<Text>();
        text.text = subtractTutoNum(textList[idx]);
        text.color = Color.white;
        text.fontSize = 20;
    }

    string subtractTutoNum(string str) //텍스트 파일에 있는 숫자 제외하고 출력하기 
    {
        tuto = int.Parse(str.Substring(0, 1));
        if (tuto == 4 || tuto == 5)
        {
            weaponIdx = int.Parse(str.Substring(2, 1));
            return str.Substring(3);
        }
        else
            return str.Substring(2);

    }

    public void ChangeScene() //튜토리얼 종료 알림 및 WaitScene으로 돌아가기 
    {
        if(tuto == 7 && EventSystem.current.currentSelectedGameObject == TextBtn ) //튜토5에서 
        {
            showText(readLine);
            StartCoroutine(OnDelay(1f));
        }
    }

    IEnumerator textBtnActive()
    {
        yield return new WaitForSeconds(2f);
        TextBtn.SetActive(true);
    } //튜토리얼 씬에 진입 직후 텍스트 출력 시간 제어

    IEnumerator EndAni(float delay)
    {
        yield return new WaitForSeconds(delay);
        aniEnd = true;
    } //animation 끝난 후 delay

    IEnumerator OnDelay(float delay)  
    {
        yield return new WaitForSeconds(delay);
        showText(readLine);
        yield return new WaitForSeconds(delay);
        loading.LoadScene("WaitScene");
    } //씬 전환 할 때 delay
}
