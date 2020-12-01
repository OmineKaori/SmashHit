using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [SerializeField, Tooltip("Playerの進む速度")]
  float playerSpeed = 2f;

  [Tooltip("衝突時に点滅させるアニメーション")] 
  public Animator alertAnimator;

  private BallCounter ballCounter;

  void Start()
  {
    ballCounter = FindObjectOfType<BallCounter>();
  }
  
  void Update()
  {
    transform.position += Vector3.forward * playerSpeed * Time.deltaTime;
  }
  
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.CompareTag("Goal"))
    {
      Debug.Log("Goal");
    }
    
    if (other.gameObject.CompareTag("Obstacle"))
    {
      alertAnimator.SetTrigger("Alert");
      
      //何回もぶつかるのを防ぐため障害物の幅だけ進ませる
      transform.position += Vector3.forward * other.transform.localScale.z;
      StartCoroutine("CameraShake");
      
      ballCounter.DecreaseBallCount(5);
    }
  }

  private IEnumerator CameraShake()
  {
    Vector3 position = transform.localPosition;
    float elapsed = 0f;

    while (elapsed < 1f)
    {
      // playerの位置を-1~1でランダムに揺らす
      var x = position.x + Random.Range(-0.5f, 0.5f);
      var y = position.y + Random.Range(-0.5f, 0.5f);

      transform.localPosition = new Vector3(x, y, position.z);
      
      elapsed += Time.deltaTime;
    
      yield return null;
    }

    // playerの座標を元に戻す
    transform.localPosition = new Vector3(0, 0, transform.position.z);
  }
}
