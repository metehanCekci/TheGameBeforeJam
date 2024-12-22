using System;
using UnityEngine;

public class Expmanager : MonoBehaviour
{
    public static Expmanager Instance;

    public delegate void ExpChange(int amount);
    public event ExpChange OnExperienceChange;

    private float currentXp = 0;
    private float maxXp = 100;
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
            maxXp *= 1.25f;
        }

        
        progressBar.SetProgress(Convert.ToInt32(Mathf.Round(currentXp)), Convert.ToInt32(Mathf.Round(maxXp)));

        
        OnExperienceChange?.Invoke(Convert.ToInt32(Mathf.Round(currentXp)));
    }
}
