using UnityEngine;
using UnityEngine.UI;

public class DisplayTotalEntitiesAmount : MonoBehaviour
{
    public Text titleText;
    public Text treesText;
    public Text foodText;
    public Text totalAnimalsText;
    public Text sheepText;
    public Text longhornsText;
    public Text wolvesText;

    private EntityFactory entityFactory = null;
    private EntityManager entityManager = null;

    private void Start()
    {
        entityFactory = GameObject.Find("GameManager").GetComponent<EntityFactory>();
        entityManager = GameObject.Find("GameManager").GetComponent<EntityManager>();
    }

    void Update()
    {
        int trees = entityManager.entitiesByType[(int)EntityManager.EntityType.TREE].Count;
        int food = entityManager.entitiesByType[(int)EntityManager.EntityType.FOOD].Count;
        int sheep = entityFactory.GetCurrentAmountOfAnimal(AnimalManager.Species.SHEEP);
        int longhorns = entityFactory.GetCurrentAmountOfAnimal(AnimalManager.Species.LONGHORN);
        int wolves = entityFactory.GetCurrentAmountOfAnimal(AnimalManager.Species.WOLF);
        int totalAnimals = sheep + longhorns + wolves;
        int totalEntities = trees + food + totalAnimals;

        string totalEntitiesToString = totalEntities > 10000 ? (totalEntities / 1000).ToString() + "k" : totalEntities.ToString();
        string totalAnimalsToString = totalAnimals > 10000 ? (totalAnimals / 1000).ToString() + "k" : totalAnimals.ToString();

        titleText.text = "Entities on scene: " + totalEntitiesToString;
        treesText.text = "Trees: " + trees.ToString();
        foodText.text = "Food: " + food.ToString();
        totalAnimalsText.text = "Total animals: " + totalAnimalsToString;
        sheepText.text = "Sheep: " + sheep.ToString();
        longhornsText.text = "Longhorns: " + longhorns.ToString();
        wolvesText.text = "Wolves: " + wolves.ToString();
    }
}