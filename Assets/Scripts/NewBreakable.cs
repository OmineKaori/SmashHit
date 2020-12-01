using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBreakable : MonoBehaviour
{
  public float minImpactToBreak = 20.0f;
  
  public GameObject block;
  
  // 最小のブロックの大きさ
  private float minBreakScale = 0.4f;
  
  // 元のオブジェクトのマテリアル
  private Material originMaterial;

  // 障害物が割れた時の音
  public AudioClip crackingClip;
  
  // 音を聞き取るオブジェクト
  private AudioSource audioSource;
  
  void Start()
  {
    audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
    
    // 元のマテリアルを取得しておく
    originMaterial = gameObject.GetComponent<Renderer>().material;
  }

  void Update()
  {
    // 一定の高さまで落ちると削除する
    if (transform.position.y < -100)
      Destroy(gameObject);
  }

  void OnCollisionEnter(Collision coll)
  {
    if (coll.gameObject.CompareTag("Ball") && coll.relativeVelocity.magnitude > minImpactToBreak)
    {
      audioSource.PlayOneShot(crackingClip);
      Break();
    } 
  }

  void Break()
  {
    // REQUIRES: 大きさ1以下の立方体, Prefabsに"block"(大きさ1)が存在
    // EFFECTS: グローバルスケールが0.4以上の時は1/8の体積のcube8個に分割
    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    
    // ボールがぶつかるほど再帰的にcubeが小さくなる仕組み
    if (transform.lossyScale.x > minBreakScale || transform.lossyScale.y > minBreakScale || transform.lossyScale.z > minBreakScale)
    {
      for (int ix = 0; ix <= 1; ix++)
      {
        for (int iy = 0; iy <= 1; iy++)
        {
          for (int iz = 0; iz <= 1; iz++)
          {
            // 割れたように見せるブロックを生成
            Vector3 vec = new Vector3(ix - 0.5f, iy - 0.5f, iz - 0.5f);
            GameObject newBlock = Instantiate(block, transform.position + vec, Quaternion.identity);
            Transform newTransform = newBlock.transform;

            // 一辺の長さを1/2に変更
            newTransform.localScale = new Vector3(transform.lossyScale.x / 2, transform.lossyScale.y / 2, transform.lossyScale.z / 2);

            // 重さを1/8に変更し，重力をオン
            var rb = newBlock.GetComponent<Rigidbody>();
            rb.mass = GetComponent<Rigidbody>().mass / 8;
            rb.useGravity = true;
            
            newBlock.GetComponent<Renderer>().material = originMaterial;

            if (newTransform .localScale.x < minBreakScale) 
              newTransform.tag = "Trash";
          }
        }
      }
      // 元のブロックを削除
      Destroy(gameObject);
    }
  }
}
