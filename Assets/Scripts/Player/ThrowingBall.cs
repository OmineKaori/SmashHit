using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ThrowingBall : MonoBehaviour
{
  // 画面中心からタップした方向に向かって射出する
  
  [Tooltip("飛ばすボールPrefab")]
  public GameObject ballPrefab;

  [Tooltip("PlayerのCamera")]
  public new Camera camera;

  [Tooltip("球を生成する位置(カメラからの相対位置")]
  public float offset = 2f;

  [Tooltip("タップ時にrayを飛ばす距離")]
  public int rayDist = 500;
  
  [Tooltip("球を投げた時の音")]
  public AudioClip throwingBallSound;
  
  // 音を聞き取るオブジェクト
  private AudioSource audioSource;
  
  // ボールを飛ばせる距離の限界(z方向)
  float distanceLimit = 500f;

  // 発射地点の座標
  private Vector3 throwingPoint;

  // ボールの増減を管理するオブジェクト
  BallCounter ballCounter;

  void Start()
  {
    audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
    ballCounter = FindObjectOfType<BallCounter>();
  }
  
  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Vector3 targetPosition = GetTargetPosition(Input.mousePosition);
      float throwingAngle = Input.mousePosition.y / Screen.height * 50f;
      
      Throwing(targetPosition, throwingAngle);
      ballCounter.DecreaseBallCount(1);
    }
  }

  private Vector3 GetTargetPosition(Vector2 mousePosition)
  {
    // クリックした2D座標を受け取り，targetとなるオブジェクトを返すメソッド
    Vector3 clickedPoint = new Vector3(mousePosition.x, mousePosition.y, 0);
    Ray ray = camera.ScreenPointToRay(clickedPoint);
    Vector3 targetPosition;

    // rayをrayDistだけ飛ばして何かにぶつかったらぶつかった3次元位置を返す
    // 何にもぶつからなかったら適当な距離の3次元位置を返す
    if (Physics.Raycast(ray, out var hitInfo, rayDist))
    {
      targetPosition = hitInfo.point;
    }
    else
    {
      clickedPoint += Vector3.forward * 50;
      targetPosition = camera.ScreenToWorldPoint(clickedPoint);
    }

    return CheckTargetPosition(targetPosition);
  }
  
  private Vector3 CheckTargetPosition(Vector3 targetPosition)
  {
    float targetDistance = (targetPosition - throwingPoint).magnitude;
    
    if (targetDistance > distanceLimit)
      targetPosition = (targetPosition - throwingPoint) / targetDistance * distanceLimit + throwingPoint;
    
    return targetPosition;
  }
  
  private void Throwing(Vector3 targetPosition, float throwingAngle)
  {
    // 発射位置を求める
    // オフセットは生成時に球の中身が映されるのを防ぐため
    throwingPoint = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, offset));
    
    Vector3 velocity = CalculateVelocity(targetPosition, throwingAngle);
    
    // 球の生成
    GameObject ball = Instantiate(ballPrefab, throwingPoint, Quaternion.identity);
    Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
    
    // 射出
    audioSource.PlayOneShot(throwingBallSound);
    ballRigidbody.AddForce(velocity * ballRigidbody.mass, ForceMode.Impulse);
  }

  private Vector3 CalculateVelocity(Vector3 targetPoint, float throwingAngle)
  {
    // タップをした位置を通過するように放物線の方程式から計算し，
    // ボールを投げるのに必要なvelocityを返すメソッド

    // 射出角をラジアンに変換
    float rad = throwingAngle * Mathf.PI / 180;

    // 水平方向の距離h
    float hDist = Vector2.Distance(new Vector2(throwingPoint.x, throwingPoint.z), new Vector2(targetPoint.x, targetPoint.z));

    // 垂直方向の距離v
    float vDist = throwingPoint.y - targetPoint.y;

    // 斜方投射の公式を初速度について解く
    float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(hDist, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (hDist * Mathf.Tan(rad) + vDist)));

    // 条件を満たす初速を算出できなければVector3.zeroを返す
    if (float.IsNaN(speed))
      return Vector3.zero;

    return new Vector3(targetPoint.x - throwingPoint.x, hDist * Mathf.Tan(rad), targetPoint.z - throwingPoint.z).normalized * speed;
  }
}
