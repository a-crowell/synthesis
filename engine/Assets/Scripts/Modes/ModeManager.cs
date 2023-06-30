using Synthesis.Runtime;
using Synthesis.UI.Dynamic;
using Synthesis.UI.Dynamic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeManager {
    
    private static IMode _currentMode;
    public static IMode CurrentMode
    {
        get => _currentMode;
        set
        {
            if (_currentMode != null)
                _currentMode.End();
            _currentMode = value;
            
            if (SceneManager.GetActiveScene().name == "MainScene") _currentMode.Start();
        }
    }

    public static void Start()
    {
        if (CurrentMode == null)
        {
            DynamicUIManager.CreateModal<ChooseModeModal>();
        }
        else 
            CurrentMode.Start();
    }
    
    public static void Update()
    {
        CurrentMode?.Update();
    }

    public static void ModalClosed()
    {
        if (CurrentMode != null) 
            CurrentMode.CloseMenu();
    }
}