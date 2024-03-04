using Unity.Netcode;

public class PlayerTeam : NetworkBehaviour
{
    public NetworkList<int> _team1 = new NetworkList<int>();
    public NetworkList<int> _team2 = new NetworkList<int>();
    public NetworkList<int> _spectate = new NetworkList<int>();


    [ServerRpc]
    public void TeamPickerServerRpc(int playerID)
    {
        if (_team1.Count > _team2.Count)
        {
            _team2.Add(playerID);
            //playerID.GetComponent<PlayerStats>()._team = Team.Team2;
        }

        else
        {
            _team1.Add(playerID);
            //playerID.GetComponent<PlayerStats>()._team = Team.Team1;
        }
    }
}