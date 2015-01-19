/// <summary>
/// this script is used by PlayerDataBase script in building the playerlist
/// </summary>

public class PlayerDataClass{
	/*Variables start*/
	public int networkPlayer;
	public string playerName;
	public int playerScore;
	public string playerTeam;
	/*Variables end**/

	public PlayerDataClass Constructor (){
		PlayerDataClass capture = new PlayerDataClass ();
		capture.networkPlayer = networkPlayer;
		capture.playerName = playerName;
		capture.playerScore = playerScore;
		capture.playerTeam = playerTeam;
		return capture;
	}
}