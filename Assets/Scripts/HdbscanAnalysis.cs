using UnityEngine;
using Python.Runtime;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class HdbscanAnalysis
{
    private List<List<double>> clusterCenters;

    // 使用する特徴量の列インデックスを指定
    //private static readonly int[] featureIndices = new int[] { 1, 2, 3 }; // 例: 2番目、3番目、4番目の列を使用
    public int[] featureIndices = new int[3];
    // SetFeatureIndices メソッドを追加
    public void SetFeatureIndices(int index1, int index2, int index3)
    {
        featureIndices[0] = index1;
        featureIndices[1] = index2;
        featureIndices[2] = index3;
    }


    public List<List<double>> PerformAnalysisAndGetClusterCenters()
    {
        PerformClusterAnalysis();
        return clusterCenters;
    }

    private void PerformClusterAnalysis()
    {
        Debug.Log("---------- HdbscanAnalysis 開始 ----------");
        Debug.Log("featureIndices: " + featureIndices[0] + ", " + featureIndices[1] + ", " + featureIndices[2]);

        PythonEngine.Initialize();

        using (Py.GIL())
        {
            // Pythonスクリプトのパスを設定
            string scriptPath = Path.Combine(Application.dataPath, "StreamingAssets", "myproject", "hdbscan_analysis.py");
            // Debug.Log("Script Path: " + scriptPath);

            // スクリプトを動的にインポート
            using (PyModule sys = (PyModule)Py.Import("sys"))
            {
                sys.Get("path").InvokeMethod("append", new PyString(Path.GetDirectoryName(scriptPath)));
            }

            using (PyModule clusterModule = (PyModule)Py.Import("hdbscan_analysis"))
            {

                // CSVデータをPythonリストに変換
                using (PyList dataList = new PyList())
                {
                    foreach (var row in CSVData.pointList)
                    {
                        using (PyList pyRow = new PyList())
                        {

                            foreach (int index in featureIndices)
                            {
                                string columnName = CSVData.pointList[0].Keys.ElementAt(index);
                                pyRow.Append(new PyFloat(float.Parse(row[columnName].ToString())));
                            }
                            dataList.Append(pyRow);
                        }
                    }

                    Debug.Log("DataList count: " + dataList.Length());
                    if (dataList.Length() == 0)
                    {
                        Debug.LogError("DataList is empty. Check CSV data.");
                        return;
                    }

                    // クラスタ分析を実行
                    using (PyObject resultPy = clusterModule.InvokeMethod("perform_hdbscan_analysis", dataList))
                    {
                      DisplayResults(resultPy);
                    }

                }
            }
        }

        // Pythonエンジンの終了
        PythonEngine.Shutdown();
        Debug.Log("---------- HdbscanAnalysis 終了 ----------");
    }

    private List<int> ConvertPyListToIntList(PyObject pyList)
    {
        var result = new List<int>();
        using (PyList list = new PyList(pyList))
        {
            foreach (PyObject item in list)
            {
                result.Add(item.As<int>());
            }
        }
        return result;
    }

    private List<double> ConvertPyListToDoubleList(PyObject pyList)
    {
        var result = new List<double>();
        using (PyList list = new PyList(pyList))
        {
            foreach (PyObject item in list)
            {
                result.Add(item.As<double>());
            }
        }
        return result;
    }

    private List<List<double>> ConvertPyListToListOfLists(PyObject pyList)
    {
        var result = new List<List<double>>();
        using (PyList list = new PyList(pyList))
        {
            foreach (PyObject item in list)
            {
                result.Add(ConvertPyListToDoubleList(item));
            }
        }
        return result;
    }

    private void DisplayResults(PyObject resultPy)
    {
        using (PyDict pyDict = new PyDict(resultPy))
        {
            int numberOfClusters = pyDict["number_of_clusters"].As<int>();
            List<int> clusterSizes = ConvertPyListToIntList(pyDict["cluster_sizes"]);
            clusterCenters = ConvertPyListToListOfLists(pyDict["cluster_centers"]);
            int noisePoints = pyDict["noise_points"].As<int>();
            List<double> markerSizes = ConvertPyListToDoubleList(pyDict["marker_sizes"]);

            Debug.Log($"クラスタ数: {numberOfClusters}");
            Debug.Log($"クラスタサイズ: {string.Join(", ", clusterSizes)}");
            Debug.Log($"クラスタ中心点:\n{string.Join("\n", clusterCenters.Select(center => $"({string.Join(", ", center)})"))}");
            Debug.Log($"ノイズポイント数: {noisePoints}");
            Debug.Log($"マーカーサイズ: {string.Join(", ", markerSizes)}");
        }
    }

    public List<List<double>> GetClusterCenters()
    {
        return clusterCenters;
    }


}
