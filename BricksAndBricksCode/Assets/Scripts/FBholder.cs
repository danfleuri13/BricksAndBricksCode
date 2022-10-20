using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using Facebook.MiniJSON;


//Code: http://greyzoned.com/
//Modifications to this app;

public class FBholder : MonoBehaviour
{
	public GameObject screenIsLoggedIn;
	public GameObject screenNotLoggedIn;
	public GameObject faceAvatar;
	public GameObject faceUserName;
    public GameObject scoreEntryPanel;
    public GameObject scoreScrollList;

    private List<object> scoresList = null;
	private Dictionary<string, string> profile = null;

	void Awake()
	{
		FB.Init (SetInit, OnHideUnity);
	}

	private void SetInit()
	{
		if(FB.IsLoggedIn)
		{
			ScreenOrder(true);
			Debug.Log ("FB Logged In");
		}else{
			ScreenOrder(false);
		}
	}

	private void OnHideUnity(bool isGameShown)
	{
		//if(!isgameshown)
		//{
		//	time.timescale = 0;
		//}else{
		//	time.timescale = 1;
		//}
	}

	public void FBlogin()
	{
		FB.LogInWithPublishPermissions(new List<string>() { "email", "publish_actions", "user_friends" }, AuthCallback);
	}

	void AuthCallback(IResult result)
	{
		if(FB.IsLoggedIn){
			Debug.Log ("FB login worked!");
            GetMyScore();
			ScreenOrder(true);
		}else{
			Debug.Log ("FB Login fail");
			ScreenOrder(false);
		}
	}

	void ScreenOrder(bool isLoggedIn)
	{
		if(isLoggedIn){
			screenIsLoggedIn.SetActive (true);
			screenNotLoggedIn.SetActive(false);

           // if (CooldownController.myScore == 0)
           // {
            //    GetMyScore();
          //  }

			FB.API (Util.GetPictureURL("me", 128, 128), HttpMethod.GET, ProfilePictureCallback);
			FB.API ("/me?fields=id,first_name", HttpMethod.GET, faceUserNameCallback);
            GetMyScore();

		}else{
			screenIsLoggedIn.SetActive (false);
			screenNotLoggedIn.SetActive(true);
		}
	}

	void ProfilePictureCallback(IGraphResult result)
	{
		if(result.Error != null)
		{
			Debug.Log ("problem with getting profile picture");

			FB.API (Util.GetPictureURL("me", 128, 128), HttpMethod.GET, ProfilePictureCallback);
			return;
		}

		Image UserAvatar = faceAvatar.GetComponent<Image>();
		UserAvatar.sprite = Sprite.Create (result.Texture, new Rect(0,0,128,128), new Vector2(0,0));

	}

	void faceUserNameCallback(IResult result)
	{
		if(result.Error != null)
		{
			Debug.Log ("problem with getting profile picture");
			
			FB.API ("/me?fields=id,first_name", HttpMethod.GET, faceUserNameCallback);
			return;
		}

		profile = Util.DeserializeJSONProfile(result.RawResult);

		Text UserMsg = faceUserName.GetComponent<Text>();

		UserMsg.text = "Hello, " + profile["first_name"];
	}

    public void ShareWithFriends()
    {
        FB.ShareLink(
            new Uri("https://developers.facebook.com/"),
            callback: ShareCallback
        );
    }

    private void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || result.Error != null)
        {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else if (result.PostId != null)
        {
            Debug.Log(result.PostId);
        }
        else
        {
            Debug.Log("ShareLink success!");
        }
    }

    public void InviteFriends()
	{
		FB.AppRequest(
			message: "This game is awesome, join me. now.",
			title: "Invite your friends to join you"
			);
	}

	public void QueryScores()
	{
		FB.API ("/app/scores?fields=score,user.limit(20)", HttpMethod.GET, ScoresCallback);
	}

	private void ScoresCallback(IResult result)
	{
		Debug.Log ("Scores callback: " + result.RawResult);

		scoresList = Util.DeserializeScores (result.RawResult);

		foreach (Transform child in scoreScrollList.transform) 
		{
			GameObject.Destroy (child.gameObject);
		}

		foreach (object score in scoresList) 
		{
			var entry = (Dictionary<string,object>) score;
			var user = (Dictionary<string,object>) entry["user"];

			GameObject ScorePanel;
			ScorePanel = Instantiate (scoreEntryPanel) as GameObject;
			ScorePanel.transform.parent = scoreScrollList.transform;

			Transform ThisScoreName = ScorePanel.transform.Find ("FriendName");
			Transform ThisScoreScore = ScorePanel.transform.Find ("FriendScore");
			Text ScoreName = ThisScoreName.GetComponent<Text>();
			Text ScoreScore = ThisScoreScore.GetComponent<Text>();

			ScoreName.text = user["name"].ToString();
			ScoreScore.text = entry["score"].ToString();

			Transform TheUserAvatar = ScorePanel.transform.Find ("FriendAvatar");
			Image UserAvatar = TheUserAvatar.GetComponent<Image>();

			FB.API (Util.GetPictureURL(user["id"].ToString (), 128,128), HttpMethod.GET, delegate (IGraphResult pictureResult)
            {
                if (pictureResult.Error != null)
                {
                    Debug.Log(pictureResult.Error);
                }
                else
                {
                    UserAvatar.sprite = Sprite.Create(pictureResult.Texture, new Rect(0, 0, 128, 128), new Vector2(0, 0));
                }
            });
		}
	}

    public static void SetScore(int score)
    {
        var scoreData = new Dictionary<string, string>();
        scoreData["score"] = score.ToString();

        FB.API("/me/scores", HttpMethod.POST, delegate (IGraphResult result)
        {
            Debug.Log ("Score submit result: " + result.RawResult);
        }, scoreData);
    }

    public static void GetMyScore()
    {
        FB.API("/me/scores", HttpMethod.GET, MyScoreResult);
    }

    public static void MyScoreResult(IGraphResult result)
    {
        if (result.Error != null)
        {
            Debug.Log(result.Error);
        }
        else
        {
            Debug.Log("My Score callback: " + result.RawResult);

            var dict = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
            var datas = (List<object>)dict["data"];
            foreach (var iterator in datas)
            {
                var data = iterator as Dictionary<string, object>;
                Debug.Log("Score is: " + data["score"]);
                Debug.Log(Convert.ToInt32(data["score"]));
                //CooldownController.myScore = Convert.ToInt32(data["score"]);
            }
        }
    }
}


