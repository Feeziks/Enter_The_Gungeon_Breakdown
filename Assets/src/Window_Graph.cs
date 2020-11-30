using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Window_Graph : MonoBehaviour
{
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private Sprite pointSprite;

    private string pointName = "point";
    private string connectorName = "connector";

    private void Awake()
    {
        
    }

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
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        GameObject lastPoint = null;
        GameObject thisPoint = null;

        foreach(Vector2 value in vals)
        {
            float xPos = (value.x / xAxis[1]) * graphWidth;
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

    public void Clear()
    {
        //iterate through all points and clear (Destroy) them
        int numChildren = graphContainer.transform.childCount;
        Transform trans;
        for(int i = 0; i < numChildren; i++)
        {
            trans = graphContainer.transform.GetChild(i);
            if(string.Compare(pointName, trans.name) == 0 || string.Compare(connectorName, trans.name) == 0)
            {
                Destroy(trans.gameObject);
            }
        }
    }
}
