using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField width_Input;
    public TMP_InputField height_Input;
    public TMP_InputField bombsCount_Input;

    public static MainMenu instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
