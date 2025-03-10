using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CupController : MonoBehaviour
{
    public float tiltSpeed = 50f; // ��б�ٶ�
    public float moveDistance = 1f; // �̰����ƾ���
    public float moveSpeed = 5f; // �����ٶ�
    public float returnSpeed = 5f; // �ָ��ٶ�
    public float maxTiltAngle = 30f;
    public float defaultTiltAngle = 15f;
    public float minTiltAngle = 0f;
    public float combineTiltAngle = 10f;
    public float holdThreshold = 0.3f; // ��������ʱ��
    [HideInInspector]public float fluidChangeRate = 0;
    public float maxInterval = 1f;
    //��
    public float foamRateOfFluid = 10f;
    public float addFoamRate = 20;
    public float minusFoamRate = 15;
    public int deleteFoamNum = 20;
    //����
    public Button strengthF;
    public Button strengthJ;
    public float strengthIncreaceRate = 15;
    public float strengthDecreaseRate = 10;
    public float strengthDecreaseClick = 5;

    private Quaternion originalRotation;
    private float originalRotation_y;
    private Vector3 originalPosition;
    private bool isTilting = false;
    private bool isMoving = false;
    private float keyHoldTime = 0f;
    private bool isKeyHeldLongEnough = false;
    private bool isReturning = false;
    //��������ˮ
    
    private float minInterval;
    private float targetTiltAngle = 0f; // Ŀ����б�Ƕ�
    private float currentTiltAngle = 0f; // ��ǰ��б�Ƕ�
    private float currentMoveDistance = 0;
    //��
    private float timer = 0f;
    private bool hasStartFoam = false;
    private float currentFoamRate = 0;
    private float defaultFoamRate = 0;
    //����
    private bool playerFPressing = false;
    private bool playerJPressing = false;
    private bool hasFDecreased = false;
    private bool hasJDecreased = false;
    private bool FneedRelax = false;
    private bool JneedRelax = false;

    //��Ұ�������
    private KeyCode keyAction1 = KeyCode.J;
    private KeyCode keyAction2 = KeyCode.F;
    private bool hasSwitchedKey = false;


    void Start()
    {
        //Ӱ��Һ��
        originalRotation = transform.rotation;
        originalRotation_y = transform.rotation.y;
        originalPosition = transform.position;
        minInterval = maxInterval / 3;
        currentTiltAngle = defaultTiltAngle;
        //Ӱ�����
        defaultFoamRate = FindObjectOfType<FoamsController>().spawnRate;
        currentFoamRate = FindObjectOfType<FoamsController>().spawnRate;
        strengthF.image.fillAmount = 1;
        
    }

    void Update()
    {

        if (FindObjectOfType<gameController>().gameStart)
        {
            FindObjectOfType<FluidsController>().startSpawn = true;
            timer += Time.deltaTime; // ��ʱ
            //��������
            if (timer >= 0.1f && !hasSwitchedKey) 
            {
                Debug.Log("keySwitched");
                //��������
                if (sceneManager.roundCount % 2 != 0) //r1
                {

                }
                else if (sceneManager.roundCount % 2 == 0) //r2
                {
                    (keyAction1, keyAction2) = (keyAction2, keyAction1); // ��������ӳ��
                }
                hasSwitchedKey = true; // ȷ��ִֻ��һ��
            }
            //������ĭ
            if (timer >= 1.5f && !hasStartFoam) // 3 ���ִ��
            {
                FindObjectOfType<FoamsController>().startSpawn = true;
                Debug.Log("startFoam");
                hasStartFoam = true; // ȷ��ִֻ��һ��
            }

            //F
            if (strengthF.image.fillAmount < 1 && !playerFPressing)
            {
                strengthF.image.fillAmount += strengthIncreaceRate* 0.01f*Time.deltaTime;
            }
            else if (FneedRelax)
            {
                strengthF.image.fillAmount += strengthIncreaceRate * 0.01f * Time.deltaTime;
            }
            else if (strengthF.image.fillAmount > 0 && playerFPressing)
            {
                strengthF.image.fillAmount -= strengthDecreaseRate * 0.01f * Time.deltaTime;
            }
            //J
            if (strengthJ.image.fillAmount < 1 && !playerJPressing)
            {
                strengthJ.image.fillAmount += strengthIncreaceRate * 0.01f * Time.deltaTime;
            }
            else if (FneedRelax)
            {
                strengthJ.image.fillAmount += strengthIncreaceRate * 0.01f * Time.deltaTime;
            }
            else if (strengthJ.image.fillAmount > 0 && playerJPressing)
            {
                strengthJ.image.fillAmount -= strengthDecreaseRate * 0.01f * Time.deltaTime;
            }

        }
        else
        {
            FindObjectOfType<FluidsController>().startSpawn = false;
            FindObjectOfType<FoamsController>().startSpawn = false;
        }


        if (FindObjectOfType<gameController>().gameStart)
        {
            bool isJPressed = Input.GetKey(keyAction1);
            bool isFPressed = Input.GetKey(keyAction2);
            if (isJPressed || isFPressed)
            {
                keyHoldTime += Time.deltaTime;
                if (keyHoldTime >= holdThreshold)
                {
                    isKeyHeldLongEnough = true;
                }
            }

            //������
            if (Input.GetKeyDown(keyAction2))
            {
                playerFPressing = true;
                hasFDecreased = false;
            }
            else if (Input.GetKeyUp(keyAction2))
            {
                playerFPressing = false;
                Debug.Log("unpressing");
            }
            if (Input.GetKeyDown(keyAction1))
            {
                playerJPressing = true;
                hasJDecreased = false;
            }
            else if (Input.GetKeyUp(keyAction1))
            {
                playerJPressing = false;
                Debug.Log("unpressing");
            }

            if (strengthF.image.fillAmount < 0.01 * strengthDecreaseRate)
            {
                FneedRelax = true;
            }
            else if (strengthF.image.fillAmount > 0.34)
            {
                FneedRelax = false;
            }
            if (strengthJ.image.fillAmount < 0.01 * strengthDecreaseRate)
            {
                JneedRelax = true;
            }
            else if (strengthJ.image.fillAmount > 0.34)
            {
                JneedRelax = false;
            }

            //����
            if (isJPressed && !isFPressed && strengthJ.image.fillAmount > 0)
            {
                if (isKeyHeldLongEnough)
                    if (!JneedRelax)
                        targetTiltAngle = maxTiltAngle;
                    else
                    {
                        targetTiltAngle = defaultTiltAngle;
                    }
            }
            else if (isFPressed && !isJPressed && strengthF.image.fillAmount > 0)
            {
                if (isKeyHeldLongEnough)
                {
                    if (!FneedRelax)
                        targetTiltAngle = minTiltAngle;
                    else
                    {
                        targetTiltAngle = defaultTiltAngle;
                    }
                }
            }
            else if (isJPressed && isFPressed && strengthF.image.fillAmount > 0 && strengthJ.image.fillAmount > 0)
            {
                if (isKeyHeldLongEnough)
                {
                    if (!FneedRelax)
                    {
                        targetTiltAngle = combineTiltAngle;
                    }
                    else
                    {
                        targetTiltAngle = maxTiltAngle;
                    }
                }
            }
            else if (isJPressed && isFPressed && strengthF.image.fillAmount > 0 && strengthJ.image.fillAmount < 0)
            {
                if (isKeyHeldLongEnough)
                {
                    if (!FneedRelax)
                        targetTiltAngle = minTiltAngle;
                    else
                    {
                        targetTiltAngle = defaultTiltAngle;
                    }
                }
            }
            else if (isJPressed && isFPressed && strengthF.image.fillAmount < 0 && strengthJ.image.fillAmount > 0)
            {
                if (isKeyHeldLongEnough)
                    if (!JneedRelax)
                        targetTiltAngle = maxTiltAngle;
                    else
                    {
                        targetTiltAngle = defaultTiltAngle;
                    }
            }
            else
            {
                targetTiltAngle = defaultTiltAngle;
            }



            if (Input.GetKeyUp(keyAction1) || Input.GetKeyUp(keyAction2))
            {
                //if (Input.GetKeyUp(keyAction1))
                //{
                //    currentFoamRate = defaultFoamRate * minusFoamRate;
                //    currentMoveDistance = moveDistance;
                //    FindObjectOfType<FoamsController>().DeleteFoams(deleteFoamNum);
                //    if (!isKeyHeldLongEnough)
                //    {
                //        isMoving = true;
                //    }
                //}
                if (Input.GetKeyUp(keyAction1) && !JneedRelax)
                {
                    currentFoamRate = defaultFoamRate * addFoamRate;
                    currentMoveDistance = moveDistance;
                    FindObjectOfType<FoamsController>().DeleteFoams(deleteFoamNum);
                    if (!isKeyHeldLongEnough)
                    {
                        isMoving = true;
                        if (!hasJDecreased)
                        {
                            //-strengthF
                            strengthJ.image.fillAmount -= 0.01f * strengthDecreaseClick;
                            hasJDecreased = true;
                        }
                    }
                }
                else if (Input.GetKeyUp(keyAction2) && !FneedRelax)
                {
                    currentFoamRate = defaultFoamRate / addFoamRate;
                    currentMoveDistance = -moveDistance;
                    if (!isKeyHeldLongEnough)
                    {
                        isMoving = true;
                        if (!hasFDecreased)
                        {
                            //-strengthF
                            strengthF.image.fillAmount -= 0.01f * strengthDecreaseClick;
                            hasFDecreased = true;
                        }
                    }
                }

                keyHoldTime = 0f;
                isKeyHeldLongEnough = false;
            }

            //��������
            //Debug.Log("rotate angle"+transform.rotation);
            //0.1736
            float tilt = Mathf.Clamp(transform.eulerAngles.z, minTiltAngle, currentTiltAngle); // ��ȡ��ǰ��б��
                                                                                               //Debug.Log("tilt" + tilt);
                                                                                               //Debug.Log("transform.rotation.z" + transform.rotation.z);
                                                                                               //Debug.Log("transform.eulerAngles.z" + transform.eulerAngles.z);
                                                                                               //Debug.Log(maxInterval * (defaultTiltAngle / transform.eulerAngles.z));
            FindObjectOfType<FluidsController>().spawnRate = maxInterval * (defaultTiltAngle / transform.eulerAngles.z);
        }

    }

    void FixedUpdate()
    {
        if (FindObjectOfType<gameController>().gameStart)
        {
            currentTiltAngle = Mathf.Lerp(currentTiltAngle, targetTiltAngle, tiltSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, currentTiltAngle);

            if (isMoving)
            {
                Vector3 targetPosition = originalPosition + Vector3.down * currentMoveDistance;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                FindObjectOfType<FoamsController>().spawnRate = currentFoamRate;
                if (transform.position == targetPosition)
                {
                    isMoving = false;
                    isReturning = true;
                }
            }
            else if (isReturning)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, returnSpeed * Time.fixedDeltaTime);
                if (transform.position == originalPosition)
                {
                    isReturning = false;
                }
            }
            else
            {
                FindObjectOfType<FoamsController>().spawnRate = foamRateOfFluid * FindObjectOfType<FluidsController>().spawnRate;
            }
        }
    }
}
