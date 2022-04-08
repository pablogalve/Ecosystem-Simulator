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
    private List<GameObject> UIelements = new List<GameObject> ();

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

        for (int i = 0; i < entityManager.entities.Count; i++)
        {
            List<int> listToAdd = new List<int>();
            entitiesAmountHistory.Add(listToAdd);            
        }

        StartCoroutine(UpdateCharts());
    }

    private IEnumerator UpdateCharts()
    {
        // Remove previous UI elements
        for (int i = 0; i < UIelements.Count; i++) Destroy(UIelements[i]);
        UIelements.Clear();

        for(int i = 0; i < entitiesAmountHistory.Count; i++)
        {
            // Update values
            int currNum = entityManager.entities[i].Count;
            entitiesAmountHistory[i].Add(currNum);

            // Update UI
            float rgbValue = (float)(i * 1.0f / entitiesAmountHistory.Count * 1.0f);
            ShowGraph(entitiesAmountHistory[i], new Color(rgbValue, rgbValue, rgbValue));
        }        

        yield return new WaitForSeconds(5f);
        StartCoroutine(UpdateCharts());
    }

    private void ShowGraph(List<int> valueList, Color color)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float xSize = graphContainer.sizeDelta.x / valueList.Count;
        float yMaximum = valueList.Max();

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition), color);

            if (lastCircleGameObject != null)
            {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                                    circleGameObject.GetComponent<RectTransform>().anchoredPosition,
                                    color);
            }
            lastCircleGameObject = circleGameObject;

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -20f);
            labelX.GetComponent<Text>().text = i.ToString();
        }

        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-xSize * 1.5f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * yMaximum).ToString();
        }
    }

    private GameObject CreateCircle(Vector2 anchoredPosition, Color color)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        gameObject.GetComponent<Image>().color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        UIelements.Add(gameObject);
        return gameObject;
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color color)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.5f);
        UIelements.Add(gameObject);

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
