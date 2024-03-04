using UnityEngine;

public class SetAbbilities : MonoBehaviour
{
    public GameObject _player;

    public void SetAbbilitie(BaseAbillitie abbilitie)
    {
        _player.GetComponent<AbilitieManager>()._abbilitie = abbilitie;
    }

    public void SetPassive(BasePassive passive)
    {
        _player.GetComponent<AbilitieManager>()._passive = passive;
    }
}
