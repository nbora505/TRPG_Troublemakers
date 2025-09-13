using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public StageData[] curStage = new StageData[3];
    public CSVReader csv;
    public int energy;
    public Sprite[] allCGs;
    public GameObject loadingPanel;

    void Awake()
    {
        if (instance == null)
        {
            Screen.SetResolution(1920, 1080, false);

            instance = this;
            DontDestroyOnLoad(gameObject);  // 첫 생성 때만 파괴 방지
        }
        else if (instance != this)
        {
            Destroy(gameObject);            // 이미 있으면 자기 자신 파괴
            return;
        }

        allCGs = Resources.LoadAll<Sprite>("CG");
        SetDailyPickUp();
    }

    void SetDailyPickUp()
    {
        //23~60
        int rand = Random.Range(23, 61);
        PlayerPrefs.SetInt("DailyPickUp", rand);
    }

    public void GameStartBtn()
    {
        if (PlayerPrefs.HasKey("FirstPlay") == false) RunTutorial();
        else StartCoroutine(AppearLoadingPanel());
    }

    IEnumerator AppearLoadingPanel()
    {
        loadingPanel.SetActive(true);
        UnityEngine.Color color = loadingPanel.GetComponent<Image>().color;
        color.a = 0;

        Image loadImage = loadingPanel.transform.GetChild(0).GetComponent<Image>();
        UnityEngine.Color cgColor = loadImage.color;
        cgColor.a = 0;

        Sprite randomCG = allCGs[Random.Range(0, allCGs.Length)];
        loadImage.sprite = randomCG;
        loadImage.preserveAspect = true;

        while (color.a < 1f)
        {
            color.a += Time.deltaTime / 0.5f;   //1초동안 실행
            loadingPanel.GetComponent<Image>().color = color;

            cgColor.a += Time.deltaTime / 0.5f;
            loadImage.color = cgColor;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        //while (color.a > 0f)
        //{
        //    color.a -= Time.deltaTime / 0.5f;   //1초동안 실행
        //    loadingPanel.GetComponent<Image>().color = color;

        //    cgColor.a -= Time.deltaTime / 0.5f;
        //    loadImage.color = cgColor;

        //}

        //yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainScene");
    }

    void RunTutorial()
    {
        // 플래그 저장
        PlayerPrefs.SetInt("Energy", 200);
        PlayerPrefs.SetString("Standing", "1");
        PlayerPrefs.SetInt("StartReward", 1);
        PlayerPrefs.SetInt("FirstPlay", 1);
        PlayerPrefs.Save();

        StartCoroutine(AppearLoadingPanel());

    }
}
