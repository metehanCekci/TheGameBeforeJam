using UnityEngine;
using UnityEngine.SceneManagement;
public class DeathMenu : MonoBehaviour
{

    [Header("GUI Elements")]
    [SerializeField] private GameObject guiElements;
    [Header("Control Elements")]
    [SerializeField] private GameObject ctrlElements;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        guiElements.gameObject.SetActive(false);
        ctrlElements.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu(){
        SceneManager.LoadScene(0);
    }
    public void QuitGame(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
