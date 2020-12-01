using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableParent : MonoBehaviour
{ 
    public GameObject block;

    void Start()
    {
        foreach (Transform child in transform)
        { 
            SetCubes(child);
        }
    }

    private void SetCubes(Component child)
    {
        // REQUIRES: Prefabsに"Block"(大きさ1)が存在, アニメーションは親オブジェクトの方につけておく
        // ブロックのグローバルサイズが一定以上ならブロックを作り変える
        // 整数値でない場合は切り捨てる
        int localX = (int)child.transform.lossyScale.x;
        int localY = (int)child.transform.lossyScale.y;
        int localZ = (int)child.transform.lossyScale.z;

        if (localX > 1 || localY > 1 || localZ > 1)
        {
            // 元のcubeは無効に(そうしないと吹っ飛ぶ)
            child.gameObject.SetActive(false);
      
            // scale1のcubeの集合に作り変え
            for (int x = (-localX / 2 - localX % 2 + 1); x <= localX / 2; x++)
            {
                for (int y = (-localY / 2 - localY % 2 + 1); y <= localY / 2; y++)
                {
                    for (int z = (-localZ / 2 - localZ % 2 + 1); z <= localZ / 2; z++)
                    {
                        // 新しく大きさ1のブロックを生成
                        // Scaleが奇数か偶数かでオブジェクトの位置を0.5ずらしている
                        Vector3 vec = new Vector3(x - (localX % 2 == 0 ? 0.5f : 0), y - (localY % 2 == 0 ? 0.5f : 0), z - (localZ % 2 == 0 ? 0.5f : 0));
                        GameObject newBlock = Instantiate(block, child.transform.position + vec, Quaternion.identity);
                        Rigidbody newRigidbody = newBlock.GetComponent<Rigidbody>();

                        // ボールが直で当たったとこ以外は動かないように
                        newRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                        // 元のマテリアルを反映
                        newBlock.GetComponent<Renderer>().material = child.GetComponent<Renderer>().material;
                        // タグ付け
                        newBlock.tag = "Obstacle";

                        // 親オブジェクトの子にすることで，アニメーション可能に
                        newBlock.transform.parent = transform;
                    }
                }
            }

            // 元のブロックを削除
            Destroy(child.gameObject);
        }
    }
}