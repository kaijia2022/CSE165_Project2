using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using TMPro;

public class CheckPointParser : MonoBehaviour
{
    public GameObject prefab; // Prefab of the game object you want to instantiate
    public GameObject drone;
    public GameObject map;
    public TextAsset file;
    public TextMeshProUGUI distanceOffset;
    public TextMeshProUGUI angleOffset;
    public TextMeshProUGUI finsih;
    public TextMeshProUGUI timer;
    private List<GameObject> CheckPoints;
    private int currIndex = 0;
    public Color lineColor = Color.red;
    public Color overlapColor = Color.green;
    private LineRenderer lineRenderer;
    public float countDown;
    private float timeCount = 0;
    private bool done = false;
    private Vector3 lastPosition;
    public TextMeshProUGUI Crush;
    public TextMeshProUGUI Reach;
    private float crushCD;
    public GameObject Landscape1;
    public GameObject Landscape2;


    void Start()
    {
        crushCD = 3; 
        done = false;
        List<Vector3> coordinates = ParseFile();
        CheckPoints = new List<GameObject>();
        for (int i = 0; i < coordinates.Count; i++)
        {
            GameObject newCheckPoint = Instantiate(prefab, coordinates[i], Quaternion.identity);
            //newCheckPoint.transform.parent = map.transform;
            newCheckPoint.name = "CheckPoint_" + i;
            CheckPoints.Add(newCheckPoint);
        }
        lastPosition = new Vector3(CheckPoints[currIndex].transform.position.x, CheckPoints[currIndex].transform.position.y, CheckPoints[currIndex].transform.position.z);
        drone.transform.position = new Vector3(CheckPoints[currIndex].transform.position.x, CheckPoints[currIndex].transform.position.y, CheckPoints[currIndex].transform.position.z);
        lineRenderer = GetComponent<LineRenderer>();
        SetLineRenderer(lineColor, 0.1f);
        DisplayBeginMessage(); 
    }


    void Update()
    {


        if (countDown > 0)
        {
            countDown -= Time.deltaTime;
            timer.text = countDown.ToString("F1");
            finsih.text = "Wait!";
            drone.GetComponent<Rigidbody>().isKinematic = true;
        } else
        {
            if(done == false){
                timeCount += Time.deltaTime;
                timer.text = timeCount.ToString("F1");
                finsih.text = "";
                drone.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                finsih.text = "Finish!";
            }

        }



        if (drone != null && CheckPoints != null)
        {
            // Get the center position of the OVR camera (assuming the camera is a child of the drone)
            Vector3 cameraCenter = drone.transform.position; 

            // Get the position of the checkpoint
            Vector3 checkpointPos = CheckPoints[currIndex].transform.position;

            Vector3 cameraDirection = Camera.main.transform.forward;

            lineRenderer.SetPosition(0, cameraCenter);
            lineRenderer.SetPosition(1, checkpointPos);

            DisplayDistance(checkpointPos-cameraCenter);
            DisplayAngle(cameraDirection, checkpointPos - cameraCenter);
        }
        if (!CheckPoints[currIndex].activeInHierarchy)
        {
            DisplayNextCheckPoint();
        }
        if (!Landscape1.activeInHierarchy)
        {
            GetCrush();
        }

        Reach.text = currIndex.ToString("F1") + "/" + CheckPoints.Count.ToString("F1");
    }

    List<Vector3> ParseFile()
    {
        float ScaleFactor = 1.0f / 39.37f;
        List<Vector3> positions = new List<Vector3>();
        string content = file.ToString();
        string[] lines = content.Split('\n');
 //       finsih.text = lines[8];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] coords = lines[i].Split(' ');
            Vector3 pos = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
            positions.Add(pos * ScaleFactor);
        }
        return positions;
    }

    void GetCrush()
    {
        
        drone.transform.position = lastPosition;
        if ( crushCD > 0)
        {
            crushCD -= Time.deltaTime;
            Crush.text = crushCD.ToString("F1");
            drone.GetComponent<Rigidbody>().isKinematic = true;
        } else
        {
            drone.GetComponent<Rigidbody>().isKinematic = false;
            Crush.text = "";
            crushCD = 3;
            Landscape1.SetActive(true);
            Landscape2.SetActive(true);
        }
        

    }

    void DisplayNextCheckPoint()
    {
        lastPosition = new Vector3(CheckPoints[currIndex].transform.position.x, CheckPoints[currIndex].transform.position.y, CheckPoints[currIndex].transform.position.z);
        currIndex += 1;
        if (currIndex == CheckPoints.Count)
        {
            DisplayFinishMessage();
        }
    }

    void DisplayFinishMessage()
    {
        done = true;
    }

    void DisplayBeginMessage()
    {
    }

    void DisplayDistance(Vector3 checkpointDir)
    {
        float distance = checkpointDir.magnitude;
        distanceOffset.text = distance.ToString("F1") + "m";
    }

    void DisplayAngle(Vector3 cameraDirection, Vector3 checkpointDir)
    {
        float angle = Vector3.Angle(cameraDirection, checkpointDir);
        if (angle == 0f)
        {
            angleOffset.color = Color.yellow;
        }
        angleOffset.text = angle.ToString("F1") + "deg";
    }


    void SetLineRenderer(Color color, float width)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }
}
