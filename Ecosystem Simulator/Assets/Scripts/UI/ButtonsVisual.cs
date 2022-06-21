using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonsVisual : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    private Button button;
    private RawImage image;
    
    public Texture buttonImage;
    public Texture buttonHoveredImage;
    public Texture buttonPressedImage;

    void Start()
    {
        button = GetComponent<Button>();
        image = GetComponent<RawImage>();
        button.onClick.AddListener(TaskOnClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.texture = buttonHoveredImage;
    }    

    public void OnPointerExit(PointerEventData eventData)
    {
        image.texture = buttonImage;
    }

    void TaskOnClick()
    {
        image.texture = buttonPressedImage;
    }
}