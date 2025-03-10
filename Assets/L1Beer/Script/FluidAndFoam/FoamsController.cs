using System.Collections.Generic;
using UnityEngine;

public class FoamsController : MonoBehaviour
{
    public GameObject foamPrefab;     // ��ĭС��Ԥ�Ƽ�

    public Transform parentTrans_foam;  // ������ĭ�ĸ�����

    List<Transform> FoamsList = new List<Transform>();//not use
    


    //spawn
    public bool startSpawn = false;// edit ��ʼ������ĭ

    public int spawNum;// edit һ����ĭ�����ܸ���
    public float spawnRate;//edit ÿ����ĭ���ɵ�ʱ����
    private float timeSinceLastSpawn = 0f;

    CalFoamSpawnPos calFoamSpawnPosController;


    //delete
    public Transform parentFoam; // ��Ҫɾ��������ĸ�����


    void Update()
    {
        if(calFoamSpawnPosController == null)
        {
            calFoamSpawnPosController = FindObjectOfType<CalFoamSpawnPos>();
        }


        //spawn
        if (!startSpawn) return;

        //generate
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnRate)
        {
            for (int i = 0; i <spawNum; i++)
            {               
                SpawnFoam();
            }           
            timeSinceLastSpawn = 0f;
        }

    }

    void SpawnFoam()
    {
        //spawn foam

        //pos
        Vector3 spawnPos = calFoamSpawnPosController.CalPos();

        //instancitaye
        GameObject foam = Instantiate(foamPrefab, spawnPos, Quaternion.identity);
        foam.transform.SetParent(parentTrans_foam);
       
    }

    public void AddToFoamsList(Transform foam)
    {
        FoamsList.Add(foam);
    }

    public void DeleteFoams(int foamDeleteCount)
    {
        List<Transform> childList = new List<Transform>();

        // ��ȡ����������
        foreach (Transform child in parentFoam)
        {
            if(child.GetComponent<Foam>().isFloating==false)
                childList.Add(child);
        }

        // ȷ������ɾ������ʵ������������
        int actualDeleteCount = Mathf.Min(foamDeleteCount, childList.Count);

        for (int i = 0; i < actualDeleteCount; i++)
        {
            // ���ѡ��һ������������
            int randomIndex = Random.Range(0, childList.Count);

            // ɾ��������
            Destroy(childList[randomIndex].gameObject);

            // ���б����Ƴ���ɾ���������壬��ֹ�ظ�ɾ��
            childList.RemoveAt(randomIndex);
        }
    }
}
