using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    

    public void LoadLevel(){
        Debug.Log("click");
        int loadVal;
        if((loadVal=SceneManager.GetActiveScene().buildIndex)==0) SceneManager.LoadScene(1);
        else SceneManager.LoadScene(loadVal+1); 
         
    }
    public void QuitGame(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
