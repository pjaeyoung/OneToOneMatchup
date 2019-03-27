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
    GameObject targetZone;
    AnimationController animationCntrl;
    public GameObject sword;
    public GameObject wand;
    GameObject magicPrefab;
    GameObject point;
    GameObject nowShot;
    GameObject shotMgr;
    Ray shotRay;
    GameObject TextBtn;
    GameObject Enemy;
    GameObject hpPotion;
    GameObject beerBox;
    GameObject loading;

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
    bool idleAni = true;
    bool aniEnd = false;

    float maxDistance = 20; //point 한계치 

    int fill = 0; // loading amount

    public int tuto = -1; //튜토리얼 chapter 번호 (0: 이동키/ 1: 회전 / 2: 아이템 획득/ 3: 아이템, 무기 종류 설명/ 4: 무기공격 / 5: 던지기 공격) 
    bool readOn = false;
    List<string> textList; //튜토리얼 설명 내용 
    int count = 0; /*                              [ 행동 카운트]
                     이동키 각각 1번 이상, 회전 1번, 아이템 클릭-getItem != null / count = 1,
                     아이템 빼기-getItem ==null / count = 1, 칼 휘두르기 - enemy 격파 / count = 3, 마법 공격 - enemy 격파 / count = 5,
                     던지기아이템 클릭-getItem !=null / count = 1, 던지기 아이템 targetZone 생성 및 던지기 실행 - getItem == null / count = 1
                   */
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
        itemBtn = GameObject.Find("Canvas").transform.Find("itemBtn").GetComponent<Button>();
        targetZone = player.transform.Find("targetZone").gameObject;
        animationCntrl = player.GetComponent<AnimationController>();
        magicPrefab = GameObject.Find("MagicPrefab");
        point = GameObject.Find("PointPrefab");
        point.SetActive(false);
        shotMgr = player.transform.GetChild(7).gameObject;
        TextBtn = GameObject.Find("Canvas").transform.Find("TextBtn").gameObject;
        TextBtn.SetActive(false);
        Enemy = GameObject.Find("enemy");
        Enemy.SetActive(false);
        hpPotion = GameObject.Find("hpPotion");
        hpPotion.SetActive(false);
        beerBox = GameObject.Find("BeerBox_Green");
        beerBox.SetActive(false);
        loading = GameObject.Find("Canvas").transform.Find("loading").gameObject;
        loading.SetActive(false);
        StartCoroutine(textBtnActive());
    }

    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if(getItem == null)
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
                count = 1;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "floor")
            IsJump = false;
        if(other.tag == "item")
            other.GetComponent<outline>().enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "item")
            other.GetComponent<outline>().enabled = false;
    }

    void Update()
    {
        if(readOn == true)
        {
            readOn = false;
            Text text = TextBtn.GetComponentInChildren<Text>();
            text.text = textList[tuto];
            text.color = Color.white;
            text.fontSize = 20;
        }
       
        if(tuto == 2)
        {
            if(hpPotion.activeSelf == false && itemBtn.gameObject.activeSelf == false)
            {
                itemBtn.gameObject.SetActive(true);
                hpPotion.SetActive(true);
            }
            clickItem(2);
        }
        else if (tuto == 3)
        {
            if(hpPotion.activeSelf == true || itemBtn.gameObject.activeSelf == true)
            {
                itemBtn.gameObject.SetActive(false);
                hpPotion.SetActive(false);
                getItem = null;
            }
        }
        else if (tuto == 4)
        {
            if (Enemy.activeSelf == false)
                Enemy.SetActive(true);
            setWeapon();
            Attack();
            if(aniEnd == true && weaponIdx == (int)eWEAPON.em_WAND)
            {
                Shot();
            }
        }
        else if (tuto == 5 && count != 1)
        {
            if (Enemy.activeSelf == true)
                Enemy.SetActive(false);
            if(beerBox.activeSelf == false)
                beerBox.SetActive(true);
            clickItem(5);
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
    }

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
    }

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

        }
    }

    void setWeapon()
    {
        if(weaponIdx == (int)eWEAPON.em_GREATESWORD)
        {
            sword.SetActive(true);
        }
        else if(weaponIdx == (int)eWEAPON.em_WAND)
        {
            sword.SetActive(false);
            magicPrefab.SetActive(true);
            wand.SetActive(true);
        }
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
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
                animationCntrl.weaponIndex = 0;
                animationCntrl.PlayAtkDmg(atkName);
            }
            else if (weaponIdx == (int)eWEAPON.em_WAND)
            {
                animationCntrl.weaponIndex = 1;
                animationCntrl.PlayAtkDmg(atkName);
            }
            StartCoroutine(EndAni(animationCntrl.GetAniLength(atkName)));
            atkAni++;
            if (atkAni >= 4)
                atkAni = 0;
        }
    }

    void clickItem(int num)
    { 
        if (Input.GetMouseButtonDown(0) && player.GetComponent<BoxCollider>().enabled == true)
        {
            player.transform.GetComponent<BoxCollider>().enabled = false;
            Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(cameraRay, out rayHit))
            {
                if(rayHit.collider.tag == "item" && rayHit.collider.GetComponent<outline>().enabled)
                {
                    getItem = rayHit.collider.gameObject;
                    if(num == 2)
                    {
                        getItem.SetActive(false);
                        changeItemImg(getItem.name);
                    }
                    else if(num == 5)
                    {
                        getItem.transform.position = player.transform.position + Vector3.up * 2;
                        getItem.GetComponent<Rigidbody>().useGravity = false;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            player.transform.GetComponent<BoxCollider>().enabled = true;
        }
    }

    void changeItemImg(string name)
    {
        Sprite spr = Resources.Load<Sprite>("Sprites/" + name);
        itemBtn.gameObject.GetComponent<Image>().sprite = spr;
    }

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
        }
    }

    void ActiveTargetZone()
    {
        targetZone.SetActive(true);
    }

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
    }

    void TransferItem()
    {
        getItem.transform.GetComponent<Rigidbody>().useGravity = true;
        Vector3 dir = targetZone.transform.position - transform.position;
        getItem.GetComponent<Rigidbody>().velocity = getItem.transform.TransformDirection(dir.x, 0, dir.z);
        targetZone.SetActive(false);
    }

    void drawPoint()
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

    void Shot()
    {
        nowShot = Instantiate(magicPrefab, shotMgr.transform.position, Quaternion.identity);
        nowShot.transform.GetChild(0).transform.eulerAngles = GetComponentInParent<Transform>().eulerAngles;
        nowShot.GetComponentInChildren<ShotController>().rayPoint = shotRay.direction * 10;
    }

    public void ReadOnTrue() //textBtn 누를 때 마다 readOn = true; 
    {
        if(EventSystem.current.currentSelectedGameObject == TextBtn && count != 1)
        {
            Debug.Log("ReadOnTrue");
            readOn = true;
            if (tuto < 5)
                tuto++;
        }
    }

    void ReadData() // 텍스트 파일 읽기
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
    
    public void ChangeScene()
    {
        if(tuto == 5 && count == 1 && EventSystem.current.currentSelectedGameObject == TextBtn)
        {
            loading.SetActive(true);
            StartCoroutine(Loading());
            int len = textList.Count;
            TextBtn.GetComponentInChildren<Text>().text = textList[len - 1];
            SceneManager.LoadScene("WaitScene");
        }
    }

    IEnumerator textBtnActive()
    {
        yield return new WaitForSeconds(2f);
        TextBtn.SetActive(true);
    }

    IEnumerator EndAni(float delay)
    {
        yield return new WaitForSeconds(delay);
        idleAni = true;
        aniEnd = true;
    }

    IEnumerator Loading() // thread로 바꾸기 
    {
        Scene scene = SceneManager.GetActiveScene();
        while (scene.name == "TutorialScene")
        {
            fill++;
            if (fill == 1)
                fill = 0;
            yield return new WaitForSeconds(0.01f);
            loading.GetComponent<Image>().fillAmount = fill;
        }
    }
}
