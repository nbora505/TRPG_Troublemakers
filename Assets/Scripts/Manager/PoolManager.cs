using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static PoolManager Instance;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    public List<Pool> pools;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Ǯ �ʱ�ȭ
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // ����Ʈ ��û
    public GameObject SpawnEffect(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Ǯ�� �ش� �±װ� ����: {tag}");
            return null;
        }

        var queue = poolDictionary[tag];
        GameObject effect;
        if (queue.Count > 0)
        {
            effect = queue.Dequeue();
        }
        else
        {
            // ť ������� prefab���� ���� ����
            var prefab = pools.Find(p => p.tag == tag)?.prefab;
            if (prefab != null)
            {
                effect = Instantiate(prefab);
            }
            else
            {
                Debug.LogWarning($"������ ������ �� ã��: {tag}");
                return null;
            }
        }

        effect.SetActive(true);
        effect.transform.SetPositionAndRotation(position, rotation);

        // ����Ʈ ���� �� �ڵ� ��ȯ
        float duration = 1f; // ����Ʈ ��� ���̿� ���� ����
        StartCoroutine(DespawnAfter(effect, tag, duration));

        return effect;
    }

    private IEnumerator DespawnAfter(GameObject obj, string tag, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }

    public GameObject SpawnEffectWithText(string tag, Vector3 pos, Quaternion rot, string message)
    {
        // 1) �⺻ Ǯ���� ��������
        GameObject go = SpawnEffect(tag, pos, rot);

        // 2) PooledText ������Ʈ�� ã�Ƽ� �ؽ�Ʈ ����
        var pooled = go.GetComponent<PooledText>();
        if (pooled != null)
        {
            pooled.SetText(message);
        }
        else
        {
            // Ȥ�� �ٷ� TMP_Text ã�Ƽ� �ٲ� ���� �ִ�
            var tmp = go.GetComponentInChildren<TextMesh>();
            if (tmp != null) tmp.text = message;
        }

        return go;
    }
}
