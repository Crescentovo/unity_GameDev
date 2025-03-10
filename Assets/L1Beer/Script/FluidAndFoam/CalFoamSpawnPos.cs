using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalFoamSpawnPos : MonoBehaviour
{
    public List<Transform> targetRangesList = new List<Transform>();
    void Start()
    {
        targetRangesList = GetAllChildren(transform);
    }


    public Vector3 CalPos()
    {
        int i = Random.Range(0, targetRangesList.Count);

        // ��ȡĿ���������Ⱦ���򣨼���Ŀ��������һ�� SpriteRenderer �� MeshRenderer��
        Renderer targetRenderer = targetRangesList[i].GetComponent<Renderer>();

        if (targetRenderer == null)
        {
            Debug.LogError("Target object does not have a Renderer component.");
            return Vector3.zero;  // ����һ��Ĭ��ֵ
        }

        // ��ȡĿ������ı߽��world space bounds��
        Bounds bounds = targetRenderer.bounds;

        // ��ȡĿ�����������λ�úʹ�С
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;

        // �� X �� Y ����ֱ�����һ�����ƫ��������Χ�ڿ�Ⱥ͸߶ȵ�һ����
        float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomY = Random.Range(center.y - size.y / 2, center.y + size.y / 2);

        // �������ɵ����λ��
        return new Vector3(randomX, randomY, center.z);  // Z ���걣�ֲ���

    }




    List<Transform> GetAllChildren(Transform parent)
    {
        List<Transform> children = new List<Transform>();

        // ���������������������
        foreach (Transform child in parent)
        {
            children.Add(child);
            // ���������Ҳ�������壬��ݹ����
            children.AddRange(GetAllChildren(child));
        }

        return children;
    }
}
