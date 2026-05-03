using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private InputAction packageAction;
    [SerializeField] private InputAction EscAction;

    [SerializeField] private Zone_Package zonePackage;
    [SerializeField] private GameObject EscUI;
    [SerializeField] private GameObject GuideUI;
    //인스펙터 할당 필요

    private bool EscUIActive = false;

    private void OnEnable()
    {
        packageAction.Enable();
        EscAction.Enable();
        packageAction.performed += OnPackagePerformed;
        EscAction.performed += EscPopupOn;
    }
    private void OnDisable()
    {
        packageAction.Disable();
        EscAction.Disable();
        packageAction.performed -= OnPackagePerformed;
        EscAction.performed -= EscPopupOn;
    }

    private void OnPackagePerformed(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0) return;
        if (zonePackage != null) zonePackage.TryPackToast(); 
        else Debug.Log("zonePackage=null");
    }

    private void EscPopupOn(InputAction.CallbackContext context)
    {
        if (GuideUI != null && GuideUI.activeSelf) return;

        if (!EscUIActive)
        {
            OpenEscMenu();
        }
        else
        {
            CloseEscMenu();
        }
    }

    private void OpenEscMenu()
    {
        EscUI.SetActive(true);
        EscUIActive = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        /*if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PauseBGM();
        }*/
    }

    public void CloseEscMenu()
    {
        EscUI.SetActive(false);
        EscUIActive = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        /*if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ResumeBGM();
        }*/
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
