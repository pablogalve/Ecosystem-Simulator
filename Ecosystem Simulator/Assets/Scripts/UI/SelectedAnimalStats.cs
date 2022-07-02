using UnityEngine;
using UnityEngine.UI;

public class SelectedAnimalStats : MonoBehaviour
{
    public Text textTitle;

    public Text textSpecies;
    public Text textGender;
    public Text textAge;

    public Text textHunger;
    public Text reproductionText;    

    private bool isSelected = false;

    private Reproduction scriptReproduction = null;
    private Animal scriptAnimal = null;
    private AgeController scriptAgeController = null;

    public void SelectAnimal(GameObject selectedAnimal)
    {
        scriptReproduction = selectedAnimal.GetComponent<Reproduction>();
        scriptAnimal = selectedAnimal.GetComponent<Animal>();
        scriptAgeController = selectedAnimal.GetComponent<AgeController>();
        isSelected = true;
    }

    public void DeselectAnimal()
    {
        scriptReproduction = null;
        scriptAnimal = null;
        scriptAgeController = null;
        isSelected = false;
    }

    void Update()
    {
        if(isSelected)
        {
            textTitle.text = "Selected Animal";
            textSpecies.text = "Species: " + scriptAnimal.species.ToString();
            textAge.text = "Age: " + scriptAgeController.age.ToString() + "/" + scriptAgeController.maxAge.ToString();
            textGender.text = scriptReproduction.gender == 1 ? "Gender: Male" : "Gender: Female";
            textHunger.text = "Food: " + scriptAnimal.hunger + " / " + scriptAnimal.maxNeed;
            reproductionText.text = "Reproduction: " + scriptAnimal.reproductionUrge + " / " + scriptAnimal.maxNeed;
        }
        else
        {
            textTitle.text = "";
            textSpecies.text = "";
            textAge.text = "";
            textGender.text = "";            
            textHunger.text = "";
            reproductionText.text = "";            
        }
    }
}