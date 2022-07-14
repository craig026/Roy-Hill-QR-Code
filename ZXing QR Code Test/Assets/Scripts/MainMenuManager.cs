using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button _qrCodeDetection;
    public Button _manualPlacement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadQRCodeDetection()
    {
        SceneManager.LoadScene("QRCodeDetection");
    }

    public void LoadManualPlacement()
    {
        SceneManager.LoadScene("ManualPlacement");
    }
}
