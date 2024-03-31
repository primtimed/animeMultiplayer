using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Movement _movement;

    public Slider _sensSlider;

    public UI _ui = new UI();

    [Serializable]
    public class UI
    {
        [SerializeField] public GameObject _weapon;

        [SerializeField] public TextMeshProUGUI _fovI;
    }

    void OnEnable()
    {
        if (_ui._weapon)
        {
            _ui._weapon.SetActive(false);
        }
    }

    private void Awake()
    {
        if (_sensSlider)
        {
            _sensSlider.GetComponent<Slider>().value = _movement._sensetivitie;
        }
    }

    public void ToMenu()
    {
        NetworkManager.Singleton.Shutdown();

        SceneManager.LoadScene(0);
    }

    public void SetSens(GameObject slider)
    {
        PlayerPrefs.SetFloat("Sens", slider.GetComponent<Slider>().value);
        _movement._sensetivitie = slider.GetComponent<Slider>().value;
        _movement._gameSens = slider.GetComponent<Slider>().value;
    }

    public void SetFOV(GameObject slider)
    {
        PlayerPrefs.SetFloat("FOV", slider.GetComponent<Slider>().value);
        int FOV = (int)slider.GetComponent<Slider>().value;
        _ui._fovI.text = FOV.ToString();
        _movement._back._camera.GetComponent<Camera>().fieldOfView = slider.GetComponent<Slider>().value;
    }

    public void ToGame()
    {
        GameUI gameUI = GetComponentInParent<GameUI>();

        gameUI._settings.SetActive(false);

        gameUI._move.enabled = true;

        gameUI._abil.enabled = true;

        gameUI._gun.enabled = true;

        gameUI._lock.SetLock(true);
    }
}
