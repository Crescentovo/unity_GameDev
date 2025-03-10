using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidsController : MonoBehaviour
{
    public GameObject fluidPrefab;//ˮ����Ԥ����

    //spawn
    public bool startSpawn = true;//�Ƿ�ʼ����ˮ

    public Transform parentTrans_fluid;  // ����ˮ�ĸ�����
    public Transform spawnTrans;//����ˮ��λ�õ�
    public int spawNum;//edit һ��ˮ���������ܸ���
    public float spawnRate;    // edit ÿ��ˮ�������ɵ�ʱ����
    private float timeSinceLastSpawn = 0f;

    List<Transform> FluidInBottleList = new List<Transform>();//not use

    void Start()
    {
        
    }


    void Update()
    {
        //spawn
        if (!startSpawn) return;

        //generate
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnRate)
        {
            for (int i = 0; i < spawNum; i++)
            {
                SpawnFluid();
            }
            timeSinceLastSpawn = 0f;
        }
    }

    void SpawnFluid()
    {
        //spawn fluid

        //pos
        Vector3 spawnPos = spawnTrans.position;

        //instancitae
        GameObject foam = Instantiate(fluidPrefab, spawnPos, Quaternion.identity);
        foam.transform.SetParent(parentTrans_fluid);

    }
    public void AddToFluidInBottleList(Transform fluid)
    {
        FluidInBottleList.Add(fluid);
    }

    public void RemoveFromInBottleList(Transform objToRemove)
    {
        if (FluidInBottleList.Contains(objToRemove))
        {
            FluidInBottleList.Remove(objToRemove);
            Debug.Log(objToRemove.name + " has been removed.");
        }
        else
        {
            Debug.Log("Object not found in the list.");
        }
    }
}
