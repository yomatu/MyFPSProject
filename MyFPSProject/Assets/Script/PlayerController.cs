using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GUNTYPE
{
    SINGLESHOT,
    AUTO,
    SNIPING
}


public class PlayerController : MonoBehaviour
{

    //ctrl +k +f 对齐代码
    //public Transform playerTransform;

    public float moveSpeed; //移动速度

    public float rotateSpeed; //转头速度

    private float angleY;//左右看
    private float angleX;//上下看

    public Animator animator;

    public Rigidbody rigid;

    public float jumpForce;

    public Vector3 pos;

    public Transform gunPointTrans;

    public GameObject bloodEffectGo;

    public GameObject GroundEffectGo;

    public float attackCD;

    public float attackTimer;

    public Transform attackEffectTrans;

    public GameObject singleAttackEffectGo;

    public GUNTYPE gunType;//武器类型

    public GameObject autoAttackEffectGo;

    public GameObject snipingAttackEffectGo;


    //字典 子弹背包   
    private Dictionary<GUNTYPE, int> bulletsBag = new Dictionary<GUNTYPE, int>();

    //字典 子弹数量
    private Dictionary<GUNTYPE, int> bulletsClip = new Dictionary<GUNTYPE, int>();


    public int maxSingleShotBullets;

    public int maxAutoShotBullets;

    public int maxSnipingShotBullets;

    //正在装填子弹
    public bool isReloading;

    public int HP;

    //  private Cat cat;

    // public Test test;


    public GameObject[] gunGo;


    //shift +F12 可以快速定位方法位置信息
    public GameObject scope; // 倍镜

    //子弹威力字典
    private Dictionary<GUNTYPE, int> gunWeaponDamage = new Dictionary<GUNTYPE, int>();


    public AudioSource audioSource;

    //音频
    public AudioClip singleShootAudio;
    public AudioClip autoShootAudio;
    public AudioClip snipingShootAudio;
    public AudioClip reloadAudio;
    public AudioClip hitGroundAudio;

    public AudioClip junmpAudio;

    public AudioSource moveAudioSource;

    public Text playerHPText;

    public GameObject[] gunUIGos;

    public Text bulletText;


    public GameObject bloodUIGo;

    public GameObject scopeUIGo;


    public GameObject gameOverPanel;

    //private Image bloodUIImg;


    void Start()
    {
        //不显示鼠标,能用在之前的项目!
        Cursor.visible = false;
        //锁定鼠标在项目内
        Cursor.lockState = CursorLockMode.Locked;


        //Vector3 v = playerTransform.position;
        //v.z = 2;
        //playerTransform.position = v;

        // Debug.Log(playerTransform.position.x);

        // playerTransform.position.x = 0;

        // test = new Test();

        //  cat = new Cat();//实例化 不实例化会报错

        //cat.name = gameObject.name;
        //test.StartGame(0.2f);
        //Test.StartGame(0.2f);
        //Debug.Log(test.x);

        bulletsBag.Add(GUNTYPE.SINGLESHOT, 30);
        bulletsBag.Add(GUNTYPE.AUTO, 50);
        bulletsBag.Add(GUNTYPE.SNIPING, 5);

        bulletsClip.Add(GUNTYPE.SINGLESHOT, maxSingleShotBullets);
        bulletsClip.Add(GUNTYPE.AUTO, maxAutoShotBullets);
        bulletsClip.Add(GUNTYPE.SNIPING, maxSnipingShotBullets);


        gunWeaponDamage.Add(GUNTYPE.SINGLESHOT, 2);
        gunWeaponDamage.Add(GUNTYPE.AUTO, 1);
        gunWeaponDamage.Add(GUNTYPE.SNIPING, 5);

        //bloodUIImg = bloodUIGo.GetComponent<Image>();

        //StartCoroutine(FadeInAndOut(bloodUIImg,1f));


    }

    // Update is called onsce per frame
    void Update()
    {

        PlayerMove();

        LookAround();

        Attack();

        Jump();

        //切枪 
        ChangeGun();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        OpenOrColseScope();

        //Vector3.forward 世界坐标正前方

        //transform.forward//游戏物体正前方


        //Debug.Log(Input.GetAxis("Vertical"));


        //Input.GetAxis("Vertical");

        //playerTransform.position = new Vector3(transform.position.x,transform.position.y,2);
        // playerTransform.position += new Vector3(0,0,1)*Time.deltaTime;
        //  transform.position += new Vector3(0, 0, 1) * Time.deltaTime;


        // Vector3 v = playerTransform.position;
        //v.z = v.z + 1 * Time.deltaTime; //v.z+=1;
        //playerTransform.position = v;
    }

    private void PlayerMove()
    {
        float verticalInput = Input.GetAxis("Vertical");//玩家ws方向输入

        float horizontalInput = Input.GetAxis("Horizontal");//玩家ad


        Vector3 movementv = transform.forward * verticalInput
            * moveSpeed * Time.deltaTime;


        Vector3 movementh = transform.right * horizontalInput
            * moveSpeed * Time.deltaTime;

        //把每一秒的位移加到位置上
        transform.position += movementv + movementh; //实现玩家移动

        //获取x移动
        animator.SetFloat("MoveX", horizontalInput);
        //获取y移动
        animator.SetFloat("MoveY", verticalInput);
        if (verticalInput > 0||horizontalInput>0)
        {
            if (!moveAudioSource.isPlaying)
            {
                moveAudioSource.Play();
            }
        }
        else
        {
            if (moveAudioSource.isPlaying)
            {
                moveAudioSource.Stop();
            }
        }

       
    }


    private void LookAround()
    {


        // Debug.Log("Y:"+Input.GetAxis("Mouse Y")+" X:"+ Input.GetAxis("Mouse X"));
        //鼠标ad的输入


        //左右看,改变y的值(欧拉角)
        // transform.eulerAngles

        //左右看
        float mouseX = Input.GetAxis("Mouse X");
        //左右看,改变y的值(玩家输入值)
        float lookHAngleY = mouseX * rotateSpeed;

        angleY = angleY + lookHAngleY;


        //上下看,改变x的值
        //鼠标ws的输入
        float mouseY = -Input.GetAxis("Mouse Y");

        //上下看,改变x的值(玩家输入值)
        float lookVAngleX = mouseY * rotateSpeed;

        //限制改变的最大值和最小值
        angleX = Mathf.Clamp(angleX + lookVAngleX, -60, 60);


        //Mathf.Clamp(lookVAngleX, -60, 60);
        //      限制角度,目标     ,最小值,最大值

        transform.eulerAngles = new Vector3(angleX, angleY, transform.eulerAngles.z);

    }
    private void Attack()
    {

        if (!isReloading)
        {

            switch (gunType)
            {
                case GUNTYPE.SINGLESHOT:
                    SingleShotAttack();
                    break;

                case GUNTYPE.AUTO:
                    AutoAttack();
                    break;

                case GUNTYPE.SNIPING:
                    SnipingAttack();

                    break;
                default:
                    break;
            }
        }





    }


    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigid.AddForce(jumpForce * Vector3.up);
            audioSource.PlayOneShot(junmpAudio);
        }
    }

    private void ChangeGun()
    {
        if (Input.GetKeyDown(KeyCode.C)&&!isReloading)
        {
            gunType++;
            //当索引大于最后一个的时候将索引变为0
            if (gunType > GUNTYPE.SNIPING)
            {
                gunType = 0;
            }

            switch (gunType)
            {
                case GUNTYPE.SINGLESHOT:
                    attackCD = 0.2f;
                    ChangeGunGameObject(0);
                    break;
                case GUNTYPE.AUTO:
                    attackCD = 0.1f;
                    ChangeGunGameObject(1);

                    break;
                case GUNTYPE.SNIPING:
                    attackCD = 1;
                    ChangeGunGameObject(2);

                    break;
                default:
                    break;
            }
        }
    }


    private void ChangeGunGameObject(int gunLevel)
    {
        for (int i = 0; i < gunGo.Length; i++)
        {
            gunGo[i].SetActive(false);
            gunUIGos[i].SetActive(false);
        }
        gunGo[gunLevel].SetActive(true);
        gunUIGos[gunLevel].SetActive(true);

        bulletText.text ="X"+ bulletsClip[gunType].ToString();

    }

    //点射枪
    private void SingleShotAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - attackTimer >= attackCD)
        {
            PlaySound(singleShootAudio);
            //访问字典
            //取得弹夹子弹数
            if (bulletsClip[gunType] > 0)
            {
                bulletsClip[gunType]--;
                bulletText.text = "X" + bulletsClip[gunType].ToString();

                animator.SetTrigger("SingleAttack");


                GameObject go = Instantiate(singleAttackEffectGo, attackEffectTrans);
                //重置位置
                go.transform.localPosition = Vector3.zero;
                //重置角度


                go.transform.localEulerAngles = Vector3.zero;

                Invoke("GunAttack", 0.15f);
                // GunAttack();    

            }
            else //子弹用完了,从背包添加子弹 
            {
                Reload();
            }

        }
    }
    //自动步枪
    private void AutoAttack()
    {
        //这里是getButton 不是GetButtonDown所以只调用一次
        if (Input.GetMouseButton(0) && Time.time - attackTimer >= attackCD)
        {
            //取到当前使用枪对应的弹夹的子弹数量
            if (bulletsClip[gunType] > 0)//弹夹还有子弹，可以攻击
            {
                PlaySound(autoShootAudio);
                bulletsClip[gunType]--;
                bulletText.text = "X" + bulletsClip[gunType].ToString();
                animator.SetBool("AutoAttack", true);
                GameObject go = Instantiate(autoAttackEffectGo, attackEffectTrans);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                GunAttack();
            }
            else//弹夹里边子弹用完了，需要从背包里拿子弹填充到弹夹里
            {
                Reload();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("AutoAttack", false);
        }
    }

    //狙击枪
    private void SnipingAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - attackTimer >= attackCD)
        {

            //取得弹夹子弹数
            if (bulletsClip[gunType] > 0)
            {

                PlaySound(snipingShootAudio);

                bulletsClip[gunType]--;
                bulletText.text = "X" + bulletsClip[gunType].ToString();

                animator.SetTrigger("SnipingAttack");


                GameObject go = Instantiate(snipingAttackEffectGo, attackEffectTrans);
                //重置位置
                go.transform.localPosition = Vector3.zero;
                //重置角度


                go.transform.localEulerAngles = Vector3.zero;

                Invoke("GunAttack", 0.25f);
                // GunAttack();    
            }
            else //子弹用完了,从背包添加子弹 
            {
                Reload();
            }

        }
    }
    //具体的射击方法
    private void GunAttack()
    {


        RaycastHit hit;

        attackTimer = Time.time;
        //发射射线 (从哪里发射,发射方向,碰到了谁,发射距离(m))

        if (Physics.Raycast(gunPointTrans.position, gunPointTrans.forward, out hit, 5))
        {
            //if (hit.collider.CompareTag("Enemy"))
            //{
            //    //实例化物体,实例化的位置,旋转角度
            //    Instantiate(bloodEffectGo,hit.point,Quaternion.identity);
            //    Debug.Log("IS HITingEnemy" );

            //}
            //Debug.Log("IS HITED"+hit.collider.name); 
            switch (hit.collider.tag)
            {
                case "Enemy":
                    Instantiate(bloodEffectGo, hit.point, Quaternion.identity);
                    //查询字典并将值附加
                    hit.transform.GetComponent<Enemy>().TakeDamage(gunWeaponDamage[gunType]);
                    PlaySound(hitGroundAudio);
                    break;


                default:
                    Instantiate(GroundEffectGo, hit.point, Quaternion.identity);
                    PlaySound(hitGroundAudio);

                    break;  
            }

        }
    }


    private void Reload()
    {
        bool canReload = false;//是否可以装子弹
        switch (gunType)
        {
            case GUNTYPE.SINGLESHOT:
                if (bulletsClip[gunType] < maxSingleShotBullets)
                {   
                    canReload = true;
                }
                break;
            case GUNTYPE.AUTO:
                if (bulletsClip[gunType] < maxAutoShotBullets)
                {
                    canReload = true;
                }
                break;
            case GUNTYPE.SNIPING:
                if (bulletsClip[gunType] < maxSnipingShotBullets)
                {
                    canReload = true;
                }
                break;
            default:
                break;
        }
        if (canReload)//弹夹不满可以填充
        {
            PlaySound(reloadAudio);
            if (bulletsBag[gunType] > 0)//背包里没子弹的时候
            {
                isReloading = true;
                Invoke("RecoverAttackState", 3f);
                animator.SetTrigger("Reload");
                switch (gunType)
                {
                    case GUNTYPE.SINGLESHOT:
                        if (bulletsBag[gunType] >= maxSingleShotBullets)
                        //背包里的剩余子弹数是足够填充满弹夹的
                        {
                            if (bulletsClip[gunType] > 0) //如果弹夹里有剩余，补满
                            {
                                //补充数量
                                int bulletNum = maxSingleShotBullets - bulletsClip[gunType];
                                bulletsBag[gunType] -= bulletNum;
                                bulletsClip[gunType] += bulletNum;
                            }
                            else  //没剩余，则需要装入最大数量
                            {
                                bulletsBag[gunType] -= maxSingleShotBullets;
                                bulletsClip[gunType] += maxSingleShotBullets;
                            }
                        }
                        else
                        {
                            //不够加满的时候就把剩余的都填充到弹夹里
                            bulletsClip[gunType] += bulletsBag[gunType];
                            bulletsBag[gunType] = 0;
                        }
                        break;
                    case GUNTYPE.AUTO:
                        if (bulletsBag[gunType] >= maxAutoShotBullets)
                        {
                            if (bulletsClip[gunType] > 0) //如果弹夹里有剩余，补满
                            {
                                //补充数量
                                int bulletNum = maxAutoShotBullets - bulletsClip[gunType];
                                bulletsBag[gunType] -= bulletNum;
                                bulletsClip[gunType] += bulletNum;
                            }
                            else  //没剩余，则需要装入最大数量
                            {
                                bulletsBag[gunType] -= maxAutoShotBullets;
                                bulletsClip[gunType] += maxAutoShotBullets;
                            }
                        }
                        else
                        {
                            bulletsClip[gunType] += bulletsBag[gunType];
                            bulletsBag[gunType] = 0;
                        }
                        break;
                    case GUNTYPE.SNIPING:
                        if (bulletsBag[gunType] >= maxSnipingShotBullets)
                        {
                            if (bulletsClip[gunType] > 0) //如果弹夹里有剩余，补满
                            {
                                //补充数量
                                int bulletNum = maxSnipingShotBullets - bulletsClip[gunType];
                                bulletsBag[gunType] -= bulletNum;
                                bulletsClip[gunType] += bulletNum;
                            }
                            else  //没剩余，则需要装入最大数量
                            {
                                bulletsBag[gunType] -= maxSnipingShotBullets;
                                bulletsClip[gunType] += maxSnipingShotBullets;
                            }
                        }
                        else
                        {
                            bulletsClip[gunType] += bulletsBag[gunType];
                            bulletsBag[gunType] = 0;
                        }
                        break;
                    default:
                        break;
                }
            }
              bulletText.text = "X" + bulletsClip[gunType].ToString();
        }
        animator.SetBool("AutoAttack", false);
    }

    //装填完毕,可以攻击
    private void RecoverAttackState()
    {
        isReloading = false;
    }

    private void OpenOrColseScope()
    {
        //右键开镜.否则不开
        if (Input.GetMouseButton(1)&&gunType==GUNTYPE.SNIPING)
        {
            scope.SetActive(true);
            scopeUIGo.SetActive(true);
        }
        else
        { 
            scope.SetActive(false);
            scopeUIGo.SetActive(false);

        }

    }


    public void TakeDamage(int value)
    {
        bloodUIGo.SetActive(true);
        HP-=value;
        Invoke("hideBloodUIGo", 1f);
        if (HP < 0)
        {
            HP = 0; 
            gameOverPanel.SetActive(true);
        }
        playerHPText.text = HP.ToString();


        //取消鼠标锁定功能
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void PlaySound(AudioClip ac)
    {
        audioSource.PlayOneShot(ac);
    }

    private void hideBloodUIGo()
    {
        bloodUIGo.SetActive(false);
    }    

    public void Replay()
    {
        //重载场景.第一个场景编号为0
        SceneManager.LoadScene(0);  
    }

    //决定单独封装到一个方法内方便调用
    //IEnumerator FadeInAndOut(Image image ,float durtion)
    //{
    //    yield return null;
    //}


}

//public class Cat
//{
//    public string name;

//    public void jump()
//    {
//        Debug.Log("maozaitiao");

//    }
//}
