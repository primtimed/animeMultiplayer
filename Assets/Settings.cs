using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public Movement _movement;

    public Slider _sensSlider;

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
