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

        // 풀 초기화
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

    // 이펙트 요청
    public GameObject SpawnEffect(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"풀에 해당 태그가 없음: {tag}");
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
            // 큐 비었으면 prefab으로 새로 생성
            var prefab = pools.Find(p => p.tag == tag)?.prefab;
            if (prefab != null)
            {
                effect = Instantiate(prefab);
            }
            else
            {
                Debug.LogWarning($"프리팹 정보도 못 찾음: {tag}");
                return null;
            }
        }

        effect.SetActive(true);
        effect.transform.SetPositionAndRotation(position, rotation);

        // 이펙트 종료 후 자동 반환
        float duration = 1f; // 이펙트 재생 길이에 맞춰 설정
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
        // 1) 기본 풀에서 꺼내오기
        GameObject go = SpawnEffect(tag, pos, rot);

        // 2) PooledText 컴포넌트를 찾아서 텍스트 설정
        var pooled = go.GetComponent<PooledText>();
        if (pooled != null)
        {
            pooled.SetText(message);
        }
        else
        {
            // 혹은 바로 TMP_Text 찾아서 바꿀 수도 있다
            var tmp = go.GetComponentInChildren<TextMesh>();
            if (tmp != null) tmp.text = message;
        }

        return go;
    }
}
