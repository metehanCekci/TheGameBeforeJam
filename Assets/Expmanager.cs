using UnityEngine;

public class Expmanager : MonoBehaviour
{
    public static Expmanager Instance;

    public delegate void ExpChange(int amount);
    public event ExpChange OnExperienceChange;

    private int currentXp = 0;
    private int maxXp = 100;
    private ProgressBar progressBar;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        
        progressBar = GameObject.Find("XP Bar").GetComponent<ProgressBar>();
    }

    public void AddExperience(int amount)
    {
        currentXp += amount;

      
        if (currentXp >= maxXp)
        {
            currentXp -= maxXp;
            maxXp *= 2;
        }

        
        progressBar.SetProgress(currentXp, maxXp);

        
        OnExperienceChange?.Invoke(currentXp);
    }
}
