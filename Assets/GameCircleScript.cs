using UnityEngine;
using System.Collections;

public class GameCircleScript : MonoBehaviour {
    bool isServiceReady;
	string[] leaderboards;
	public Texture gameCircle;
	
	public GUIStyle buttonStyle;

	// Use this for initialization
	void Start () {
		AGSClient.ServiceReadyEvent += serviceReadyHandler;
		AGSClient.ServiceNotReadyEvent += serviceNotReadyHandler;
		bool usesLeaderboards = true;
		bool usesAchievements = false;
		bool usesWhispersync = false;
		
		AGSClient.Init (usesLeaderboards, usesAchievements, usesWhispersync);
		isServiceReady = AGSClient.IsServiceReady();


		// Hook up feedback functions
//		AGSLeaderboardsClient.SubmitScoreCompleted += submitScoreSucceeded;
//		AGSLeaderboardsClient.SubmitScoreFailed += submitScoreFailed;
//
//		AGSAchievementsClient.UpdateAchievementSucceeded(updateAchievementSucceeded);
//		AGSAchievementsClient.UpdateAchievementFailed(updateAchievementFailed);

        leaderboards = new string[]{"classic_50", "time_30"};
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.JoystickButton19) || Input.GetKeyDown(KeyCode.JoystickButton11)){
			AGSClient.ShowGameCircleOverlay();
		}
	}

	private void ReportScore(float s){
        isServiceReady = AGSClient.IsServiceReady();
		if(isServiceReady){
            long score = (long)(s * 1000.0F);
            string board = "score";

//            if (gameFSM.FsmVariables.GetFsmBool("timedGame").Value){
//                board = leaderboards[1]; // timed
//                score = (long)gameFSM.FsmVariables.GetFsmInt("score").Value;
//            } else {
//                board = leaderboards[0]; // classic
//                score = (long)Mathf.RoundToInt(gameFSM.FsmVariables.GetFsmFloat("timer").Value * 100);
//            }
            
            Debug.Log ("Reporting score " + score + " on leaderboard " + board);

			//Debug.Log ("Reporting Score");
			if(score > 0) AGSLeaderboardsClient.SubmitScore(board,score);
		}else{
			Debug.Log ("Score - Service is not ready");
		}
	}

	void OnGUI(){
		if(GameController.betweenRoundGUI){
			if(GUI.Button(new Rect(Screen.width * 0.25F, Screen.height * 0.80F, Screen.width * 0.10F, Screen.width* 0.10F), gameCircle, buttonStyle)){
				AGSClient.ShowGameCircleOverlay();
			}
		}
	}

    void ShowLeaderboard(){
        AGSClient.ShowGameCircleOverlay();
    }

	// Game Circle Functions
	private void serviceNotReadyHandler (string error)    {
//		Debug.Log("Service is not ready");
		isServiceReady = false;
	}
	
	private void serviceReadyHandler ()    {
//		Debug.Log("Service is ready");
		isServiceReady = true;
	}

	private void submitScoreSucceeded(string leaderboardId){
		Debug.Log ("submitScoreSucceeded");
	}
	
	private void submitScoreFailed(string leaderboardId, string error){
		Debug.Log ("submitScoreFailed: " + error);
	}

	private void updateAchievementSucceeded(string achievementId) {
		Debug.Log ("updateAchievementSucceeded");
	}
	
	private void updateAchievementFailed(string achievementId, string error) {
		Debug.Log ("updateAchievementFailed: " + error);
	}
}
