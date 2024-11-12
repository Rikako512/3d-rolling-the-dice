using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

// This script gets values from CSVData script
// It instantiates points and particles according to values read

public class PointRenderer : MonoBehaviour {

    //********Public Variables********

    // Bools for editor options
    public bool renderPointPrefabs = true;  // データ点をすべて描画する
    public bool renderParticles =  true;
    public bool renderPrefabsWithColor = true;
    public bool only1plot = false; // 散布図行列上で一つの散布図だけ描画する（デバッグ用）

    // Name of the input file, no extension
    //public string inputfile;

    // Indices for columns to be assigned
    public int column1 = 0;
    public int column2 = 1;
    public int column3 = 2;

    // Full column names from CSV (as Dictionary Keys)
    public string xColumnName;
    public string yColumnName;
    public string zColumnName;

    // Scale of particlePoints within graph, WARNING: Does not scale with graph frame
    private float plotScale = 10;


    // Scale of the prefab particlePoints
    [Range(0.0f, 0.5f)]
    public float pointScale = 0.25f;

    // Changes size of particles generated
    [Range(0.0f, 2.0f)]
    public float particleScale = 5.0f;

    // The prefab for the data particlePoints that will be instantiated
    public GameObject PointPrefab;

    // Object which will contain instantiated prefabs in hiearchy
    public GameObject PointHolder;

    // Objects which will contain axis labels in hiearchy
    public GameObject XLabels;
    public GameObject YLabels;
    public GameObject ZLabels;

    // Color for the glow around the particlePoints
    private Color GlowColor;

    //********Private Variables********
        // Minimum and maximum values of columns
    private float xMin;
    private float yMin;
    private float zMin;

    private float xMax;
    private float yMax;
    private float zMax;

    // Number of rows
    private int rowCount;

    // HdbscanAnalysisのインスタンスを保持するフィールド
    private HdbscanAnalysis hdbscanAnalysis;

    // List for holding data from CSV reader
    //private List<Dictionary<string, object>> pointList;

    // Particle system for holding point particles
    private ParticleSystem.Particle[] particlePoints;


    //********Methods********

    void Awake()
    {
        //Run CSV Reader
        //pointList = CSVReader.Read(inputfile);
        //pointList = CSVData.pointList;
    }

    // Use this for initialization
    void Start ()
	  {
        Debug.Log("---------- PointRenderer開始 ----------");

        // HdbscanAnalysisをインスタンス化
        hdbscanAnalysis = new HdbscanAnalysis();

        // Store dictionary keys (column names in CSV) in a list
        List<string> columnList = new List<string>(CSVData.pointList[1].Keys);

        // Debug.Log("There are " + columnList.Count + " columns in the CSV");
        //
        // foreach (string key in columnList)
        //     Debug.Log("Column name is " + key);

        // Assign column names according to index indicated in columnList
        xColumnName = columnList[column1];
        yColumnName = columnList[column2];
        zColumnName = columnList[column3];

        // Get maxes of each axis, using FindMaxValue method defined below
        //xMax = FindMaxValue(xColumnName);
        //yMax = FindMaxValue(yColumnName);
        //zMax = FindMaxValue(zColumnName);
        xMax = Convert.ToSingle(CSVData.min_maxList[xColumnName][1]);
        yMax = Convert.ToSingle(CSVData.min_maxList[yColumnName][1]);
        zMax = Convert.ToSingle(CSVData.min_maxList[zColumnName][1]);

        // Get minimums of each axis, using FindMinValue method defined below
        // xMin = FindMinValue(xColumnName);
        // yMin = FindMinValue(yColumnName);
        // zMin = FindMinValue(zColumnName);
        xMin = Convert.ToSingle(CSVData.min_maxList[xColumnName][0]);
        yMin = Convert.ToSingle(CSVData.min_maxList[yColumnName][0]);
        zMin = Convert.ToSingle(CSVData.min_maxList[zColumnName][0]);


        // Debug.Log(xMin + " " + yMin + " " + zMin); // Write to console

        AssignLabels();

        if (renderPointPrefabs == true)
        {
            // Call PlacePoint methods defined below
            PlacePrefabPoints();
        }
        else // クラスタリング処理後の簡略化したデータ点を描画
        {
            if (only1plot == true)
            {
                // column1, column2, column3が特定の場合のみ実行
                if (column1 == 1 && column2 == 0 && column3 == 3)
                {

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    
                    SimplifiedPoints();

                    stopwatch.Stop();
                    Debug.Log($"HDBSCANの実行時間: {stopwatch.ElapsedMilliseconds} ミリ秒");
                }
            }
            else
            {
                SimplifiedPoints();
            }
            
        }

        // If statement to turn particles on and off
        if ( renderParticles == true)
        {
            // Call CreateParticles() for particle system
            CreateParticles();

            // Set particle system, for point glow- depends on CreateParticles()
            GetComponent<ParticleSystem>().SetParticles(particlePoints, particlePoints.Length);
        }

        Debug.Log("---------- PointRenderer終了：(" + column1 + ", " + column2 + ", " + column3 + ") ----------");
    }


    // Update is called once per frame
    void Update ()
    {
      //Activate Particle System
      //GetComponent<ParticleSystem>().SetParticles(particlePoints, particlePoints.Length);

    }

    // Places the prefabs according to values read in
    private void PlacePrefabPoints()
    {
      Debug.Log("---------- PlacePrefabPoints 開始 ----------");
      // Get count (number of rows in table)
      rowCount = CSVData.pointList.Count;

      for (var i = 0; i < rowCount; i++)
      {

        // Set x/y/z, standardized to between 0-1
        float x = (Convert.ToSingle(CSVData.pointList[i][xColumnName]) - xMin) / (xMax - xMin);
        float y = (Convert.ToSingle(CSVData.pointList[i][yColumnName]) - yMin) / (yMax - yMin);
        float z = (Convert.ToSingle(CSVData.pointList[i][zColumnName]) - zMin) / (zMax - zMin);

        // Create vector 3 for positioning particlePoints
        Vector3 position = new Vector3 (x, y, z) * plotScale;

        //instantiate as gameobject variable so that it can be manipulated within loop
        GameObject dataPoint = Instantiate (PointPrefab, Vector3.zero, Quaternion.identity);

        // Make child of PointHolder object, to keep particlePoints within container in hiearchy
        dataPoint.transform.parent = PointHolder.transform;

        // Position point at relative to parent
        dataPoint.transform.localPosition = position;
        pointScale = 0.25f;
        dataPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

        // Converts index to string to name the point the index number
        string dataPointName = i.ToString();

        // Assigns name to the prefab
        dataPoint.transform.name = dataPointName;

        // データ点の色を指定
        if (renderPrefabsWithColor == true)
        {
          // Sets color according to x/y/z value
          dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, z, 1.0f);

          // Activate emission color keyword so we can modify emission color
          dataPoint.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

          dataPoint.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(x, y, z, 1.0f));

        }

      }

      Debug.Log("---------- PlacePrefabPoints 終了 ----------");
    }


    // HDBSCANを行ってデータ点の数を減らして簡略化したデータ点分布を描画
    private void SimplifiedPoints()
    {
        Debug.Log("---------- SimplifiedPoints 開始 ----------");


        // 特徴量のインデックスを設定
        hdbscanAnalysis.SetFeatureIndices(column1, column2, column3);

        // クラスタリング処理を実行し、結果を取得
        List<List<double>> clusterCenters = hdbscanAnalysis.PerformAnalysisAndGetClusterCenters();

        for (int i = 0; i < clusterCenters.Count; i++)
            {
                List<double> center = clusterCenters[i];

                Debug.Log($"クラスタ {i} の中心点: ({string.Join(", ", center)})");

                // クラスタ中心点の座標を0-1の範囲に正規化
                float x = (float)((center[0] - xMin) / (xMax - xMin));
                float y = (float)((center[1] - yMin) / (yMax - yMin));
                float z = (float)((center[2] - zMin) / (zMax - zMin));

                // 座標をプロットスケールに合わせて調整
                Vector3 position = new Vector3(x, y, z) * plotScale;

                // プレハブをインスタンス化
                GameObject dataPoint = Instantiate(PointPrefab, Vector3.zero, Quaternion.identity);

                // PointHolderの子オブジェクトとして設定
                dataPoint.transform.parent = PointHolder.transform;

                // 相対位置を設定
                dataPoint.transform.localPosition = position;

                Debug.Log($"最終的な位置: {position}");

                // スケールを設定
                pointScale = 2.5f;
                dataPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

                // データポイントに名前を付ける
                dataPoint.transform.name = $"Cluster_Center_{i}";

                // データ点の色を指定（オプション）
                if (renderPrefabsWithColor)
                {
                    Color pointColor = new Color(x, y, z, 1.0f);
                    dataPoint.GetComponent<Renderer>().material.color = pointColor;
                    dataPoint.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                    dataPoint.GetComponent<Renderer>().material.SetColor("_EmissionColor", pointColor);
                }
            }


        Debug.Log("---------- SimplifiedPoints 終了 ----------");
    }



    // creates particlePoints in the Particle System game object
    //
    //
    private void CreateParticles()
    {
      //pointList = CSVReader.Read(inputfile);

      rowCount = CSVData.pointList.Count;
      // Debug.Log("Row Count is " + rowCount);

      particlePoints = new ParticleSystem.Particle[rowCount];

      for (int i = 0; i < CSVData.pointList.Count; i++)
      {
        // Convert object from list into float
        float x = (Convert.ToSingle(CSVData.pointList[i][xColumnName]) - xMin) / (xMax - xMin);
        float y = (Convert.ToSingle(CSVData.pointList[i][yColumnName]) - yMin) / (yMax - yMin);
        float z = (Convert.ToSingle(CSVData.pointList[i][zColumnName]) - zMin) / (zMax - zMin);

        // Debug.Log("Position is " + x + y + z);

        // Set point location
        particlePoints[i].position = new Vector3(x, y, z) * plotScale;

        //GlowColor =
        // Set point color
        particlePoints[i].startColor = new Color(x, y, z, 1.0f);
        particlePoints[i].startSize = particleScale;
      }

    }

    // Finds labels named in scene, assigns values to their text meshes
    // WARNING: game objects need to be named within scene
    private void AssignLabels()
    {
        // Update point counter
        //GameObject.Find("Point_Count").GetComponent<TextMesh>().text = pointList.Count.ToString("0");

        // Update title according to inputfile name
        //GameObject.Find("Dataset_Label").GetComponent<TextMesh>().text = inputfile;

        // Update axis titles to ColumnNames
        // GameObject.Find("X_Title").GetComponent<TextMesh>().text = xColumnName;
        // GameObject.Find("Y_Title").GetComponent<TextMesh>().text = yColumnName;
        // GameObject.Find("Z_Title").GetComponent<TextMesh>().text = zColumnName;
        XLabels.transform.Find("X_Title").gameObject.GetComponent<TextMesh>().text = xColumnName;
        YLabels.transform.Find("Y_Title").gameObject.GetComponent<TextMesh>().text = yColumnName;
        ZLabels.transform.Find("Z_Title").gameObject.GetComponent<TextMesh>().text = zColumnName;

        // Set x Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        // GameObject.Find("X_Min_Lab").GetComponent<TextMesh>().text = xMin.ToString("0.0");
        // GameObject.Find("X_Mid_Lab").GetComponent<TextMesh>().text = (xMin + (xMax - xMin) / 2f).ToString("0.0");
        // GameObject.Find("X_Max_Lab").GetComponent<TextMesh>().text = xMax.ToString("0.0");
        XLabels.transform.Find("X_Min_Lab").gameObject.GetComponent<TextMesh>().text = xMin.ToString("0.0");
        XLabels.transform.Find("X_Mid_Lab").gameObject.GetComponent<TextMesh>().text = (xMin + (xMax - xMin) / 2f).ToString("0.0");
        XLabels.transform.Find("X_Max_Lab").gameObject.GetComponent<TextMesh>().text = xMax.ToString("0.0");

        // Set y Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        // GameObject.Find("Y_Min_Lab").GetComponent<TextMesh>().text = yMin.ToString("0.0");
        // GameObject.Find("Y_Mid_Lab").GetComponent<TextMesh>().text = (yMin + (yMax - yMin) / 2f).ToString("0.0");
        // GameObject.Find("Y_Max_Lab").GetComponent<TextMesh>().text = yMax.ToString("0.0");
        YLabels.transform.Find("Y_Min_Lab").gameObject.GetComponent<TextMesh>().text = yMin.ToString("0.0");
        YLabels.transform.Find("Y_Mid_Lab").gameObject.GetComponent<TextMesh>().text = (yMin + (yMax - yMin) / 2f).ToString("0.0");
        YLabels.transform.Find("Y_Max_Lab").gameObject.GetComponent<TextMesh>().text = yMax.ToString("0.0");

        // Set z Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        // GameObject.Find("Z_Min_Lab").GetComponent<TextMesh>().text = zMin.ToString("0.0");
        // GameObject.Find("Z_Mid_Lab").GetComponent<TextMesh>().text = (zMin + (zMax - zMin) / 2f).ToString("0.0");
        // GameObject.Find("Z_Max_Lab").GetComponent<TextMesh>().text = zMax.ToString("0.0");
        ZLabels.transform.Find("Z_Min_Lab").gameObject.GetComponent<TextMesh>().text = zMin.ToString("0.0");
        ZLabels.transform.Find("Z_Mid_Lab").gameObject.GetComponent<TextMesh>().text = (zMin + (zMax - zMin) / 2f).ToString("0.0");
        ZLabels.transform.Find("Z_Max_Lab").gameObject.GetComponent<TextMesh>().text = zMax.ToString("0.0");

    }

    //Method for finding max value, assumes PointList is generated
    // private float FindMaxValue(string columnName)
    // {
    //     //set initial value to first value
    //     float maxValue = Convert.ToSingle(pointList[0][columnName]);
    //
    //     //Loop through Dictionary, overwrite existing maxValue if new value is larger
    //     for (var i = 0; i < pointList.Count; i++)
    //     {
    //         if (maxValue < Convert.ToSingle(pointList[i][columnName]))
    //             maxValue = Convert.ToSingle(pointList[i][columnName]);
    //     }
    //
    //     //Spit out the max value
    //     return maxValue;
    // }
    //
    // //Method for finding minimum value, assumes PointList is generated
    // private float FindMinValue(string columnName)
    // {
    //     //set initial value to first value
    //     float minValue = Convert.ToSingle(pointList[0][columnName]);
    //
    //     //Loop through Dictionary, overwrite existing minValue if new value is smaller
    //     for (var i = 0; i < pointList.Count; i++)
    //     {
    //         if (Convert.ToSingle(pointList[i][columnName]) < minValue)
    //             minValue = Convert.ToSingle(pointList[i][columnName]);
    //     }
    //
    //     return minValue;
    // }

}
