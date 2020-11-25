using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Window_Graph : MonoBehaviour
{
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private Sprite pointSprite;

    private string pointName = "point";

    private void Awake()
    {
        
    }

    private void CreatePoint(Vector2 anchoredPos)
    {
        GameObject gameObject = new GameObject(pointName, typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = pointSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;
        rectTransform.sizeDelta = new Vector2(15, 15);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
    }

    public void Graph(List<Vector2> vals, Vector2 xAxis, Vector2 yAxis)
    {
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        foreach(Vector2 value in vals)
        {
            float xPos = (value.x / xAxis[1]) * graphWidth;
            float yPos = (value.y / yAxis[1]) * graphHeight;

            CreatePoint(new Vector2(xPos, yPos));
        }
    }

    public void Clear()
    {
        //iterate through all points and clear (Destroy) them
        int numChildren = graphContainer.transform.childCount;
        Transform trans;
        for(int i = 0; i < numChildren; i++)
        {
            trans = graphContainer.transform.GetChild(i);
            if(string.Compare(pointName, trans.name) == 0)
            {
                Destroy(trans.gameObject);
            }
        }
    }
}
