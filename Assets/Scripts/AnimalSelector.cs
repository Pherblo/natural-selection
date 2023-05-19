using UnityEngine;
using TMPro;

// allows you to select animals, and show information about them
public class AnimalSelector : MonoBehaviour
{
    [Header("Settings")]
    public float selectionRange = 0.5f; // how big is the radius for selecting animals with the mouse
    public string targetTag; // tag used to filter what you can select

    // various referecences to scene gameobjects for displaying data
    [Header("Gameobjects Refs")]
    public Camera cam;
    public Transform ring;
    [Space]
    public GameObject animalPanel;
    public TMP_Text group;
    public TMP_Text state;
    public TMP_Text eCost;
    public TMP_Text lifetime;
    public TMP_Text size;
    public TMP_Text constitution;
    public TMP_Text strength;
    public TMP_Text dexterity;
    public TMP_Text sensing;
    public TMP_Text combined;

    private Animal selectedAnimal;

    void Update()
    {
        // when you left mouse click...
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // find where you clicked in world space
            if (Physics.Raycast(ray, out hit, 100))
            {
                selectedAnimal = null;

                // check for any animals within a range
                foreach (var obj in Physics.OverlapSphere(hit.point, selectionRange))
                {
                    if (obj.transform.gameObject.CompareTag(targetTag))
                    {
                        // if so, select its Animal component
                        selectedAnimal = obj.transform.gameObject.GetComponent<Animal>();
                        break;
                    }
                }
            }
        }

        // show selection ring + UI if theres a selected animal
        ring.gameObject.SetActive(selectedAnimal);
        animalPanel.SetActive(selectedAnimal);

        // set position for ring + data for UI
        if(selectedAnimal)
        {
            ring.transform.position = new Vector3(selectedAnimal.transform.position.x, ring.transform.position.y, selectedAnimal.transform.position.z);

            group.text = "Group: <color=#FF6666>" + selectedAnimal.animalGroup;
            state.text = "State: <color=#FF6666>" + selectedAnimal.GetCurrentState().StateName();
            eCost.text = "Energy Cost: <color=#FF6666>" + selectedAnimal.energyCost.ToString();
            lifetime.text = "Lifetime: <color=#FF6666>" + selectedAnimal.timeToLive.ToString("F1") + "s";
            size.text = "Size: <color=#FF6666>" + selectedAnimal.size.ToString("F1") + "cm";
            constitution.text = "Constitution: <color=#FF6666>" + selectedAnimal.constitution.ToString();
            strength.text = "Strength: <color=#FF6666>" + selectedAnimal.strength.ToString();
            dexterity.text = "Dexterity: <color=#FF6666>" + selectedAnimal.dexterity.ToString();
            sensing.text = "Sensing: <color=#FF6666>" + selectedAnimal.sensing.ToString();
            combined.text = "Combined: <color=#FF0000>" + (selectedAnimal.constitution + selectedAnimal.strength + selectedAnimal.dexterity + selectedAnimal.sensing);
        }
    }

    // allow external scripts to feed the selected animal
    public void FeedSelectedAnimal(int amount)
    {
        if (selectedAnimal)
            selectedAnimal.FeedAnimal(amount);
    }

    // allow external scripts to force mutate the selected animal
    public void MutateSelectedAnimal()
    {
        if (selectedAnimal)
            selectedAnimal.MutateStats();
    }

    // allow external scripts to destroy the selected animal
    public void DestroySelectedAnimal()
    {
        if(selectedAnimal)
            Destroy(selectedAnimal.gameObject);
    }
}