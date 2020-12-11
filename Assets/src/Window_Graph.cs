using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Window_Graph : MonoBehaviour
{
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private Sprite pointSprite;

    [SerializeField] private RectTransform XAxisLabelTemplate;
    [SerializeField] private RectTransform YAxisLabelTemplate;
    [SerializeField] private RectTransform XAxisLineTemplate;
    [SerializeField] private RectTransform YAxisLineTemplate;

    private string pointName = "point";
    private string connectorName = "connector";
    private string labelName = "label";
    private string lineName = "line";

    private GameObject CreatePoint(Vector2 anchoredPos)
    {
        GameObject gameObject = new GameObject(pointName, typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = pointSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;
        rectTransform.sizeDelta = new Vector2(15, 15);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    public void Graph(List<Vector2> vals, Vector2 xAxis, Vector2 yAxis)
    {
        float graphWidth = graphContainer.rect.width;//graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.rect.height;//graphContainer.sizeDelta.y;

        GameObject lastPoint = null;
        GameObject thisPoint = null;

        //Assuming that the passed list is sorted low to high
        float minVal = vals[0].x;
        float maxVal = vals[vals.Count - 1].x;
        float scaler = graphWidth / (maxVal - minVal);

        foreach(Vector2 value in vals)
        {
            float xPos = (value.x - minVal) * scaler;
            float yPos = (value.y / yAxis[1]) * graphHeight;

            thisPoint = CreatePoint(new Vector2(xPos, yPos));

            if(lastPoint != null)
            {
                Vector2 temp1 = lastPoint.GetComponent<RectTransform>().anchoredPosition;
                Vector2 temp2 = thisPoint.GetComponent<RectTransform>().anchoredPosition;
                ConnectPoints(temp1, temp2);
            }
            lastPoint = thisPoint;
        }

        int diff = (int)Mathf.Ceil(xAxis[1]) - (int)Mathf.Floor(xAxis[0]);
        int axisLabelCount = (int)Mathf.Min(diff, 10); // never have more than 10 labels
        for(int i = 0; i <= axisLabelCount; i++)
        {
            //Add X Axis Labels
            float normalizer = i * 1.0f / axisLabelCount;
            float xPos = (normalizer * graphWidth);
            CreateXAxisLabel(xPos, (Mathf.FloorToInt(xAxis[0] + i * (diff / axisLabelCount))).ToString("0.#"));
            CreateXAxisLine(xPos);

            //Add Y Axis Labels
            float yPos = normalizer * graphHeight;
            CreateYAxisLabel(yPos, (normalizer * yAxis[1]).ToString("0.#"));
            CreateYAxisLine(yPos);
        }

    }

    private void ConnectPoints(Vector2 point1, Vector2 point2)
    {
        //Create the connection line
        GameObject line = new GameObject(connectorName, typeof(Image));
        line.transform.SetParent(graphContainer, false);
        Image lineImage = line.GetComponent<Image>();
        lineImage.color = new Color(1, 1, 1, 0.5f);

        //Set up the rect transform
        RectTransform rectTransform = line.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        //Set the line to actually connect the two points
        Vector2 dir = (point2 - point1).normalized;
        float dist = Vector2.Distance(point1, point2);
        rectTransform.anchoredPosition = point1 + (dir * dist * 0.5f); // place the line halfway between the two points
        rectTransform.sizeDelta = new Vector2(dist, 4f); //Size it to fit
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);//Rotate it to connect the two points in a straight line
    }

    private void CreateXAxisLabel(float xPos, string label)
    {
        RectTransform thisLabel = Instantiate(XAxisLabelTemplate);
        thisLabel.name = labelName;
        thisLabel.SetParent(graphContainer, false);
        thisLabel.gameObject.SetActive(true);
        thisLabel.anchoredPosition = new Vector2(xPos, XAxisLabelTemplate.transform.position.y);
        TMP_Text txt = thisLabel.GetComponent<TMP_Text>();
        txt.SetText(label);
    }

    private void CreateXAxisLine(float xPos)
    {
        RectTransform thisLine = Instantiate(XAxisLineTemplate);
        thisLine.name = lineName;
        thisLine.SetParent(graphContainer, false);
        thisLine.gameObject.SetActive(true);
        thisLine.anchoredPosition = new Vector2(xPos, XAxisLineTemplate.transform.position.y);
    }

    private void CreateYAxisLabel(float yPos, string label)
    {
        RectTransform thisLabel = Instantiate(YAxisLabelTemplate);
        thisLabel.name = labelName;
        thisLabel.SetParent(graphContainer, false);
        thisLabel.gameObject.SetActive(true);
        thisLabel.anchoredPosition = new Vector2(YAxisLabelTemplate.transform.position.x, yPos);
        TMP_Text txt = thisLabel.GetComponent<TMP_Text>();
        txt.SetText(label);
    }

    private void CreateYAxisLine(float yPos)
    {
        RectTransform thisLine = Instantiate(YAxisLineTemplate);
        thisLine.name = lineName;
        thisLine.SetParent(graphContainer, false);
        thisLine.gameObject.SetActive(true);
        thisLine.anchoredPosition = new Vector2(YAxisLineTemplate.position.x, yPos);
    }

    public void Clear()
    {
        //iterate through all points and clear (Destroy) them
        int numChildren = graphContainer.transform.childCount;
        Transform trans;
        for(int i = 0; i < numChildren; i++)
        {
            trans = graphContainer.transform.GetChild(i);
            if(string.Compare(trans.name, pointName)        == 0 || 
               string.Compare(trans.name, connectorName)    == 0 ||
               string.Compare(trans.name, labelName)        == 0 ||
               string.Compare(trans.name, lineName)         == 0)
            {
                Destroy(trans.gameObject);
            }
        }
    }
}
