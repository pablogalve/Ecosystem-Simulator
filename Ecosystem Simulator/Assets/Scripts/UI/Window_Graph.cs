using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Window_Graph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;

    private EntityManager entityManager = null;
    private List<List<int>> entitiesAmountHistory = new List<List<int>>();
    private List<List<GameObject>> lineElements = new List<List<GameObject>>();
    private List<List<GameObject>> circleElements = new List<List<GameObject>>();
    private List<RectTransform> labelXElements = new List<RectTransform>();
    private List<RectTransform> labelYElements = new List<RectTransform>();

    private float yMaximum;

    // Inspector
    public int maxLabelsX = 5;
    public int maxLabelsY = 5;

    private void Awake()
    {
        graphContainer = GameObject.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = GameObject.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = GameObject.Find("labelTemplateY").GetComponent<RectTransform>();
        entityManager = GameObject.Find("GameManager").GetComponent<EntityManager>();

        StartCoroutine(InitializeCharts());
    }

    private IEnumerator InitializeCharts()
    {
        yield return new WaitForSeconds(1f); // Wait for other scripts

        for (int i = 0; i < entityManager.entitiesByType.Count; i++)
        {
            List<int> listOfInts = new List<int>();
            entitiesAmountHistory.Add(listOfInts);

            List<GameObject> listOfGO = new List<GameObject>();
            lineElements.Add(listOfGO);

            List<GameObject> listOfCircles = new List<GameObject>();
            circleElements.Add(listOfCircles);
        }

        StartCoroutine(UpdateCharts());
    }

    private IEnumerator UpdateCharts()
    {
        // Remove previous UI elements
        /*for (int i = 0; i < UIelements.Count; i++)
        { 
            Destroy(UIelements[i]); 
        }
        UIelements.Clear();*/

        for(int i = 0; i < entitiesAmountHistory.Count; i++)
        {
            // Update values
            int currNum = entityManager.entitiesByType[i].Count;
            entitiesAmountHistory[i].Add(currNum);

            if (currNum > yMaximum) yMaximum = currNum;

            // Update UI
            float rgbValue = (float)(i * 1.0f / entitiesAmountHistory.Count * 1.0f);
            ShowGraph(entitiesAmountHistory[i], new Color(rgbValue, rgbValue, rgbValue), i);
        }        

        yield return new WaitForSeconds(5f);
        StartCoroutine(UpdateCharts());
    }

    private void ShowGraph(List<int> valueList, Color color, int speciesIndex)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float xSize = graphContainer.sizeDelta.x / valueList.Count;        

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), color, i, speciesIndex);
              
            if (lastCircleGameObject != null)
            {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                                    circleGameObject.GetComponent<RectTransform>().anchoredPosition,
                                    color,
                                    i,
                                    speciesIndex);
            }
            lastCircleGameObject = circleGameObject;
        }

        //float xLabelStep = Mathf.Ceil((float)valueList.Count / (float)maxLabelsX);
        for (int i = 0; i < maxLabelsX; i++)
        {
            //int amountOfXValues = valueList.Count;
            int xValue = Mathf.FloorToInt(((float)valueList.Count-1.0f) * ((float)i / (float)maxLabelsX));
            //int xValue = valueList[xLabelStep];
            //if (i % xLabelStep != 0) continue;

            RectTransform labelX;
            if (labelXElements.Count <= i) // Create new  
            {
                labelX = Instantiate(labelTemplateX);
                labelXElements.Add(labelX);
            }
            else // Reuse existing  
            {
                labelX = labelXElements[i];
            }

            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);

            float xPos = (graphContainer.sizeDelta.x * ((float)i / (float)maxLabelsX));
            labelX.anchoredPosition = new Vector2(xPos, -20.0f);
            labelX.GetComponent<Text>().text = xValue.ToString();
        }

        // Display a maximum of "maxLabelsY" labels on the y-axis
        for (int i = 0; i <= maxLabelsY; i++)
        {
            RectTransform labelY;
            if (labelYElements.Count <= i) // Create new  
            {
                labelY = Instantiate(labelTemplateY);
                labelYElements.Add(labelY);
            }
            else // Reuse existing  
            {
                labelY = labelYElements[i];
            }

            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / maxLabelsY;

            float xPos = (graphContainer.sizeDelta.x - gameObject.GetComponent<RectTransform>().rect.width) * 0.8f;
            labelY.anchoredPosition = new Vector2(xPos, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * yMaximum).ToString();
        }
    }

    private GameObject CreateCircle(Vector2 anchoredPosition, Color color, int i, int speciesIndex)
    {
        GameObject gameObject;
        if (circleElements[speciesIndex].Count <= i) // Create new  
        {
            gameObject = new GameObject("circle", typeof(Image));
            circleElements[speciesIndex].Add(gameObject);
        }
        else // Reuse existing  
        {
            gameObject = circleElements[speciesIndex][i];
        }

        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        gameObject.GetComponent<Image>().color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        
        return gameObject;
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color color, int i, int speciesIndex)
    {
        GameObject gameObject;
        if (lineElements[speciesIndex].Count <= i) // Create new  
        {
            gameObject = gameObject = new GameObject("dotConnection", typeof(Image));
            lineElements[speciesIndex].Add(gameObject);
        }
        else // Reuse existing  
        {
            gameObject = lineElements[speciesIndex][i];
        }

        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.5f);

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI);
    }
}
