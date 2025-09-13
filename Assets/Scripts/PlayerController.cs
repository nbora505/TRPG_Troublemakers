using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Defination
    public enum CharacterState
    {
        Idle,
        Action,
        Play,
        Move,
        Ex,
        Stun,
        Die
    }

    [Header("상태")]
    public CharacterState state;
    public bool isEnemy;
    public bool isArrived;

    [Header("스탯")]
    float speed = 6.0f;
    public float curHP;
    public float curMP;
    public float curAtk;
    public float curMAtk;
    public float curDef;
    public float curMDef;
    public float curCri;
    public float curMiss;

    [Header("연결 오브젝트")]
    public BattleManager bm;
    public SkillManager sm;
    public GameObject token;
    private StatusEffectManager sem;

    [Header("데이터")]
    public GameObject target;
    public CharacterData characterData;
    public List<SkillData> skillData;

    public Slider greenBar;
    float hpBarTime;
    private float prevHP;
    private bool isCoroutineRunning = false;

    #endregion

    void Start()
    {
        if (isEnemy)
        {
            state = CharacterState.Idle;
        }
        else
        {
            state = CharacterState.Move;
        }

        sem = GetComponent<StatusEffectManager>();
        SetStatus();
    }

    void SetStatus()
    {
        curHP = characterData.hp;
        prevHP = curHP;
        curMP = 0;
        curAtk = characterData.atk;
        curMAtk = characterData.mAtk;
        curDef = characterData.def;
        curMDef = characterData.mDef;
        curCri = characterData.cri;
        curMiss = characterData.miss;
}

    void FixedUpdate()
    {
        if (bm.isEnd) return;

        ShootRay();
        UpdateHp();
        UpdateMp();
        CheckIsDead();
        CheckIsDamaged();

        switch (state)
        {
            case CharacterState.Idle:
                CheckIsReady();
                break;
            case CharacterState.Action:
                if (isEnemy) SetNewTarget();
                if (!isCoroutineRunning)
                {
                    isCoroutineRunning = true;
                    StartCoroutine(PlayAction());
                }
                break;
            case CharacterState.Play:
                if (isEnemy) SetNewTarget();
                break;
            case CharacterState.Move:
                MoveCharacter();
                break;
            case CharacterState.Ex:
                break;
            case CharacterState.Stun:
                break;
            case CharacterState.Die:
                break;
            default:
                break;
        }
    }

    void ShootRay()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, characterData.range, LayerMask.GetMask("Enemy"));

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            target = hit.transform.gameObject;

            if (state == CharacterState.Move)
                state = CharacterState.Idle;

            if (!isArrived && !isEnemy)
            {
                isArrived = true;
                bm.isArrived.Add(true);
            } 
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + transform.right * characterData.range, Color.gray);
            state = CharacterState.Move;
        }
    }

    void MoveCharacter()
    {
        transform.Translate(new Vector2(1, 0) * speed * Time.fixedDeltaTime);
    }

    void CheckIsReady()
    {
        if(bm.state == BattleManager.BattleState.playing && bm.isPlaying)
        {
            state = CharacterState.Action;
            Debug.Log(state);
        }
    }

    IEnumerator PlayAction()
    {
        state = CharacterState.Play;
        for (int i = 0; i < 6; i++)
        {
            int index = characterData.pattern[i];
            if (index > 0)
            {
                // target이 null인 경우, 스킬 사용을 중단
                if (target == null || target.active == false)
                {
                    Debug.LogWarning("Target is null, stopping PlayAction coroutine.");
                    break;
                }

                Debug.Log(characterData.name + "의 " + index + "번째 스킬 " + skillData[index-1].name + "!");

                sm.UseSkill(skillData[index-1], this, target.GetComponent<PlayerController>());
            }

            yield return new WaitForSeconds(1f);
        }
        state = CharacterState.Action;
        isCoroutineRunning = false;
    }

    //상태 효과(버프/디버프/스턴 등)를 등록하는 메서드
    public void ApplyEffect(StatusEffect.Type type, float amount, float duration)
    {
        var eff = new StatusEffect(type, amount, duration);
        sem.AddEffect(eff);

        if (type == StatusEffect.Type.Stun)
            StartCoroutine(StunCoroutine(duration));
    }
    // 스턴
    public IEnumerator StunCoroutine(float duration)
    {
        state = CharacterState.Stun;
        yield return new WaitForSeconds(duration);
        state = CharacterState.Idle;  // 이전 상태 복귀 로직 필요 시 구현
    }

    void UpdateHp()
    {
        if (!isEnemy)
        {
            Slider hpBar = token.transform.Find("HpBar").GetComponent<Slider>();
            hpBar.value = curHP / characterData.hp;

            if(hpBar.value <= 0)
            {
                GameObject fillArea = hpBar.gameObject.transform.Find("Fill Area").gameObject;
                GameObject fill = fillArea.transform.Find("Fill").gameObject;
                fill.SetActive(false);
            }
        }
    }
    void UpdateMp()
    {
        if (!isEnemy && bm.isPlaying && !bm.isEnd)
        {
            Slider mpBar = token.transform.Find("MpBar").GetComponent<Slider>();

            curMP += 0.1f;
            if(curMP > 100) 
            {
                curMP = 100f;
                token.GetComponent<Button>().interactable = true;
            }
            mpBar.value = curMP / 100;
        }
    }

    void ActiveGreenBar()
    {
        hpBarTime += Time.fixedDeltaTime;

        greenBar.gameObject.SetActive(true);
        greenBar.value = curHP / characterData.hp;

        if(hpBarTime > 3f)
        {
            greenBar.gameObject.SetActive(false);
        }
    }

    void SetNewTarget()
    {

        // 1) 내가 적이면 playerList, 내가 아군이면 enemyList
        var list = isEnemy
            ? BattleManager.instance.playerList
            : BattleManager.instance.enemyList;

        // 2) 디코이 우선: list 안에서 Decoy 상태인 캐릭터 찾기
        var decoyTarget = list
            .Select(go => go.GetComponent<PlayerController>())
            .FirstOrDefault(pc => pc.HasEffect(StatusEffect.Type.Decoy));
        if (decoyTarget != null)
        {
            target = decoyTarget.gameObject;
            return;
        }

        // 3) 디코이 없으면 기존 포지션 기준으로 타깃 선정
        //    (여기선 x 좌표가 가장 큰 놈, 필요에 따라 가장 작은 쪽으로 바꿔도 OK)
        float bestX = float.MinValue;
        GameObject bestGO = null;
        foreach (var go in list)
        {
            float x = go.transform.position.x;
            if (x > bestX)
            {
                bestX = x;
                bestGO = go;
            }
        }

        // 4) 안전장치: 리스트가 비어있으면 target 유지, 아니면 새로 할당
        if (bestGO != null)
            target = bestGO;
    }

    void CheckIsDamaged()
    {
        if (curHP != prevHP)
        {
            hpBarTime = 0;
            ActiveGreenBar();
            prevHP = curHP;
        }
    }

    void CheckIsDead()
    {
        if (curHP <= 0)
        {
            curHP = 0;
            state = CharacterState.Die;

            if(!isEnemy)
            {
                //플레이어 리스트에서 본인 지우기
                for (int i = 0; i < bm.playerList.Count; i++)
                {
                    if (bm.playerList[i] == this.gameObject)
                    {
                        bm.playerList.Remove(bm.playerList[i]);
                        return;
                    }
                }

                //근데 전멸했으면 패배
                if(bm.playerList.Count == 0)
                {
                    bm.state = BattleManager.BattleState.defeat;
                }

                Debug.LogWarning(characterData.name + " 사망ㅠㅠㅠ");
            }
            else
            {
                //에너미 리스트에서 본인 지우기
                for (int i = 0; i < bm.enemyList.Count; i++)
                {
                    if (bm.enemyList[i] == this.gameObject)
                    {
                        bm.enemyList.Remove(bm.enemyList[i]);
                        return;
                    }
                }

                //근데 전멸했으면 승리
                if (bm.enemyList.Count == 0)
                {
                    bm.state = BattleManager.BattleState.win;
                }

                //적이면 깔끔하게 본인 파괴
                Debug.LogWarning(characterData.name + " 파괴!!!!");
                Destroy(this);
            }
            this.gameObject.SetActive(false);
        }
    }


    //이 캐릭터에 해당 타입의 상태 이상이 있는지
    public bool HasEffect(StatusEffect.Type type)
    {
        return sem != null && sem.HasEffect(type);
    }
}
