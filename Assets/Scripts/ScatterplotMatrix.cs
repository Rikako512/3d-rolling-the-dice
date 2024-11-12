using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// This script gets values from CSVData script
// It instantiates scatterplots which will compose scatterplot matrix.
/* ScatterplotMatrix にアタッチされているスクリプト */

public class ScatterplotMatrix : MonoBehaviour
{
    // Bools for editor options
    public bool renderGraphsPrefabs = true;
    // public bool renderParticles =  true;
    // public bool renderPrefabsWithColor = true;

    // The prefab of scatterplot that will be instantiated
    public GameObject GraphPrefab; // 量産するオブジェクトのプレハブ

    // Object which will contain instantiated prefabs of scatterplot in hiearchy
    public GameObject GraphContainer; // 量産したオブジェクトを格納する場所

    public float innerRadius = 10.0f; // 最も内側の半径
    public float radiusSpace = 10.0f; // 半径の間隔
    public float maxRadian = 1.0f; // 展開する範囲の角度
    private int dimension; // 次元の数
    private float bottom = 3.0f; // 一番下のグラフの底面の高さ

    // Start is called before the first frame update
    void Start()
    { 
      Debug.Log("ScatterplotMatrix開始");
      // Store dictionary keys (column names in CSV) in a list
      List<string> columnList = new List<string>(CSVData.pointList[1].Keys);
      dimension = columnList.Count;
     
      if (renderGraphsPrefabs == true)
      {
          // Call PlacePoint methods defined below
          PlacePrefabGraphs();
      }

      Debug.Log("ScatterplotMatrix終了");
    }

    // Update is called once per frame
    void Update()
    {

    }

    // プレハブを3次元行列の形に配置
    private void PlacePrefabGraphs()
    {
      float start_radian = (1.0f-maxRadian)/2.0f; // 始まりの角度

      // プレハブを元に、インスタンスを生成
      for(int m = 0; m < dimension; m++){
        float radian = (start_radian + m * maxRadian / (dimension-1)) * Mathf.PI; // 各グラフの原点に対する中心核
        Quaternion rot = Quaternion.Euler(0, 90 - (radian * 180 / Mathf.PI), 0); // 各グラフの回転の向き

        for(int n = 0; n < dimension; n++){
          float y = radiusSpace * n + bottom;

          for(int k = 0; k < dimension; k++){
            float radius = innerRadius + radiusSpace * k;
            float x = radius * Mathf.Cos(radian);
            float z = radius * Mathf.Sin(radian);

            //instantiate as gameobject variable so that it can be manipulated within loop
            GameObject graph = Instantiate (GraphPrefab, new Vector3(x, y, z), rot);

            // Make child of PointHolder object, to keep particlePoints within container in hiearchy
            graph.transform.parent = GraphContainer.transform;

            // Converts index to string to name the point the index number
            string graphName = $"{m}, {n}, {k}";
            // Assigns name to the prefab
            graph.transform.name = graphName;

            if(GraphPrefab.name == "Scatterplot"){
              // scatterplotのcolumn1,2,3に格納する
              PointRenderer scatterplotter = graph.transform.Find("GraphFrame/Plotter").gameObject.GetComponent<PointRenderer>();
              scatterplotter.column1 = m;
              scatterplotter.column2 = n;
              scatterplotter.column3 = k;
            }

          }

        }

      }

    }

}
