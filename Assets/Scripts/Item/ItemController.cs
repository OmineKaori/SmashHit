using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    //Itemにつけるスクリプト
    //Itemがボールにぶつかったら，ポイントを加算
    private Rigidbody rb;

    private bool isTouched = false;

    private BallCounter ballCounter;
  
    // 音を聞き取るオブジェクト
    private AudioSource audioSource;

    public Item item;

    void Start()
    {
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        ballCounter = FindObjectOfType<BallCounter>();
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    void OnCollisionEnter(Collision other)
    {
        if (isTouched)
            return;
    
        //falseの時だけぶつかった判定をだす
        if (other.gameObject.CompareTag("Ball"))
        {
            isTouched = true;

            //マテリアルの変更(色褪せたように見せる)
            gameObject.GetComponent<Renderer>().material = item.usedItemMaterial;

            //ポイント加算
            AddPoints(item.point);
            //ポイント加算のQuadの表示
            Instantiate(item.itemCanvas, transform.position, Quaternion.identity);
      
            audioSource.PlayOneShot(item.itemCrackingSound);

            //下に落ちるようにする
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }
  
    private void AddPoints(int point)
    {
        ballCounter.IncreaseBallCount(point);
    }
}