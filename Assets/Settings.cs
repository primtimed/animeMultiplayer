using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public Movement _movement;

    public void ToMenu()
    {
        NetworkManager.Singleton.Shutdown();

        SceneManager.LoadScene(0);
    }

    public void SetSens(GameObject slider)
    {
        _movement._sensetivitie = slider.GetComponent<Slider>().value;
        _movement._gameSens = slider.GetComponent<Slider>().value;
    }
}
