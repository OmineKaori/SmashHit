using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallCounter : MonoBehaviour
{
  // ボールの個数の増減を管理する
  
  [Tooltip("スタートの時のボールの数")]
  public static int startBallNum = 20;
  
  // 残りのball数
  private int currentBallNum;

  [Tooltip("残りのボールの数を表示するtext")]
  public Text ballCountText;

  void Start()
  {
    currentBallNum = startBallNum;
    ShowBallCount(currentBallNum);
  }
  
  private void ShowBallCount(int count)
  {
    ballCountText.text = count.ToString();
  }

   public void IncreaseBallCount(int value)
  {
    currentBallNum += value;
    ShowBallCount(currentBallNum);
  }
   
  public void DecreaseBallCount(int value)
  {
    currentBallNum -= value;

    if (currentBallNum < 0)
    {
      SceneController.instance.EndGame();
    }
    else
    {
      ShowBallCount(currentBallNum);
    }
  }
}
