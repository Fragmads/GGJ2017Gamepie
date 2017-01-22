using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLevel : MonoBehaviour {

	#region Properties

	public List<GamePlayer> players = new List<GamePlayer>();

	public Scene NextLevel;

	#endregion

}
