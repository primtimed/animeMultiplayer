using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    }

    void OnEnable()
    {
        _ui._weapon.SetActive(false);
    }

    private void Awake()
    {
        _sensSlider.GetComponent<Slider>().value = _movement._sensetivitie;
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
}
