using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager MenuManagerInstance;
    public bool GameState;
    public GameObject[] menuElement = new GameObject[4];


    void Start()
    {
        MenuManagerInstance = this;
        GameState = false;
        menuElement[3].GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("score").ToString();
    }


    void Update()
    {

    }
    public void StartGame()
    {
        GameState = true;
        menuElement[0].SetActive(false);
        GameObject.FindGameObjectWithTag("AirEffect").GetComponent<ParticleSystem>().Play();
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
