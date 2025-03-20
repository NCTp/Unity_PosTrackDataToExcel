using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using System.Runtime;
using UnityEngine;
using UnityEngine.Video;

public struct EyeData
{
    // EyeData Export to csv file.
    public string fixation;
    public string fixationX;
    public string fixationY;
    public string saccade;
    public string timeline;
    public string duration;

    // Time start record eye data.

    public EyeData(string _fixation, string _fixationX, string _fixationY, string _saccade, string _timeline, string _duration)
    {
        fixation = _fixation;
        fixationX = _fixationX;
        fixationY = _fixationY;
        saccade = _saccade;
        timeline = _timeline;
        duration = _duration;
    }
}


public class EyeDataExporter : MonoBehaviour
{
    
    private List<string[]> posData = new List<string[]>();
    private float _timeline = 0.0f;

    private EyeData _eyeData;
    private List<EyeData> _eyeDataList = new List<EyeData>();

    public float width = 100.0f; // width of square

    public float trackDuration = 1.0f;
    private float _trackTimer = 0.0f;
    private float _duration = 0.0f;
    private string _eyeCoord;
    private string _preEyeCoord;
    private int _eyePosX;
    private int _eyePosY;
    private bool _isTracking = false;

    void WriteStart()
    {
        Debug.Log("Resolution : " + Screen.width + " , " + Screen.height);
        EyeData tempEyeData = new EyeData("Fixations",
            "X_Fixation",
            "Y_Fixation",
            "saccades",
            "TimeLines",
            "Durations");
        _eyeDataList.Add(tempEyeData);

    }

    private void WriteEnd()
    {
        string[][] output = new string[_eyeDataList.Count][];
        Debug.Log(_eyeDataList.Count);
        string[] tempEyeData = new string[3];

        int length = output.GetLength(0);

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            sb.AppendLine(_eyeDataList[i].fixation  + "," + 
                          _eyeDataList[i].fixationX + "," +
                          _eyeDataList[i].fixationY + "," +
                          _eyeDataList[i].saccade + "," +
                          _eyeDataList[i].timeline + "," + 
                          _eyeDataList[i].duration);
        }
        
        string filePath = getPath();

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();

        Debug.Log("Write csv file complete");
    }

    private string getPath()
    {
        #if UNITY_EDITOR
        return Application.dataPath + "/CSV/PosData.csv";
        #else
        return Applcation.dataPath + "/ "+" PosData.csv";
        #endif
    }

    private void SaveEyeData(Vector2 pos)
    {
        int eyePosX = (int)(pos.x / width) + 65;
        int eyePosY = (int)(pos.y / width);
        _eyeCoord = (char) eyePosX + eyePosY.ToString();
        Debug.Log("(" + pos.x + ", " + pos.y + ")");
        EyeData tempEyeData = new EyeData(_preEyeCoord,
            pos.x.ToString(),
            pos.y.ToString(),
            "saccade", 
            _timeline.ToString()
            , _duration.ToString());
            
        _eyeDataList.Add(tempEyeData);
        Debug.Log("Save Eye Data Completed");
    }

    void TrackEyePos(float x, float y)
    {
        _eyePosX = (int)(x / width) + 65;
        _eyePosY = (int)(y / width);
        
        if (_eyeCoord == (char)_eyePosX + _eyePosY.ToString())
        {
            //Debug.Log("Same Coordinate");
            _preEyeCoord = _eyeCoord;
            _trackTimer += Time.deltaTime;
            if (_trackTimer >= trackDuration)
            {
                _isTracking = true;
                _duration += Time.deltaTime; // 머무른 시간 측정 시작
            }
            else
            {
                _trackTimer += Time.deltaTime;
            }
        }
        else
        {
            Debug.Log("Different Coordinate");
            //Debug.Log(_trackTimer);
            if (_isTracking)
            {
                _duration += trackDuration;
                SaveEyeData(Input.mousePosition);
                _duration = 0.0f;
                _trackTimer = 0.0f;
                _isTracking = false;
                Debug.Log("Eye Data Saved");
            }
            else
            {
                _trackTimer = 0.0f;
            }
        }
        
        _eyeCoord = (char) _eyePosX + _eyePosY.ToString();
        //Debug.Log(_eyeCoord);
    }
    // Start is called before the first frame update
    void Start()
    {
        WriteStart();
    }

    // Update is called once per frame
    void Update()
    {
        _timeline += Time.deltaTime;
        TrackEyePos(Input.mousePosition.x,Input.mousePosition.y);
    }

    private void OnApplicationQuit()
    {
        WriteEnd();
        //throw new NotImplementedException();
    }
    void OnGUI()
    {
        GUI.color = Color.green;
        GUI.Label(new Rect(125, 20, 200, 20), "Fixation : " + _eyeCoord);
        GUI.Label(new Rect(125, 40, 200, 20), "Duration : " + _duration);
        GUI.Label(new Rect(125, 60, 200, 20), "TrackTimer : " + _trackTimer);
        GUI.Label(new Rect(125, 80, 200, 20), "Timeline : " + _timeline);
    }
}
