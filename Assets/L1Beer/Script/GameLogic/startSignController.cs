using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class startSignController : MonoBehaviour
{
    public float moveSpeed = 3f; // �½��ٶ�
    public float holdTime = 1f;  // ͣ��ʱ��
    public float exitSpeed = 5f; // �볡�ٶ�
    public Text showRound;

    private Vector3 startPos;
    private Vector3 midPos;
    private Vector3 exitPos;
    private float eclapsedTime;
    private bool flag = true;

    void Start()
    {
        
       
    }

    private void Update()
    {
        eclapsedTime += Time.deltaTime;
        if (eclapsedTime > 0.1 && flag)
        {

            //round info
            if (sceneManager.roundCount % 2 != 0)
            {
                switch (sceneManager.gameCount)
                {
                    case 1: showRound.text = "ROUND 1"; break;
                    case 2: showRound.text = "ROUND 2"; break;
                    case 3: showRound.text = "ROUND 3"; break;

                }
            }
            else if (sceneManager.roundCount % 2 == 0)
            {
                showRound.text = "SWITCH!";
            }

            // ����λ��
            startPos = new Vector3(transform.position.x, Screen.height, transform.position.z);
            midPos = new Vector3(transform.position.x, Screen.height / 2, transform.position.z);
            exitPos = new Vector3(transform.position.x, -Screen.height, transform.position.z);

            transform.position = startPos; // ��ʼλ��
            StartCoroutine(PlayIntroAnimation());
            flag = false;
        }
    }

    IEnumerator PlayIntroAnimation()
    {

        // �ƶ����м�
        yield return MoveToPosition(midPos, moveSpeed);

        // ͣ�� 1 ��
        yield return new WaitForSeconds(holdTime);

        // �����½�ֱ���뿪��Ļ
        yield return MoveToPosition(exitPos, exitSpeed);

        // ��Ϸ��ʼ
        FindObjectOfType<gameController>().gameStart = true;
        Debug.Log("��Ϸ��ʼ��");
    }

    IEnumerator MoveToPosition(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }
}
