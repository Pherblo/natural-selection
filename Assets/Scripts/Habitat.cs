using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Habitat : MonoBehaviour
{
    // various references to scene objects and prefabs
    [Header("Gameobject Refs")]
    public GameObject cam;
    public GameObject habitatPlane;
    public GameObject animalGO;
    public GameObject foodGO;
    [Space]
    public Light sun;
    [Space]
    public Slider sizeSldr;
    public Slider temperatureSldr;
    public Slider humiditySldr;
    public Slider foodSldr;
    public Toggle feederTggl;
    [Space]
    public TMP_Text sizeTxt;
    public TMP_Text tempTxt;
    public TMP_Text humidityTxt;
    public TMP_Text foodPerTxt;

    [Header("Habitat Settings")]
    [Range(1, 50)] public float size = 5; // size of habitat in meters
    [Range(-50, 50)] public int temperature = 20; // habitat tempurature in Celsius
    [Range(0, 100)]  public int humidity = 10; // habitat humidity in %
    [Space]
    public int lowColorTemp = 3500; // sun temperature when habitate temperature is at its lowest
    public int highColorTemp = 9500; // and when its at its highest
    [Space]
    public bool feederEnabled; // will the habitat spawn food?
    public int foodProduction = 10; // how much food it produces
    public int foodLifetime = 10; // how long the food lasts before it gets destroyed
    public int foodProductionRate = 1; // how often food is produced

    private float foodTimer;

    void Awake()
    {
        // set variables to slider values in case theyre set different
        size = sizeSldr.value;
        temperature = (int)Mathf.Round(temperatureSldr.value);
        humidity = (int)Mathf.Round(humiditySldr.value);
        foodProduction = (int)Mathf.Round(foodSldr.value);
    }

    private void Update()
    {
        // spawn x food each second if the feeder is enabled
        if (foodTimer < foodProductionRate)
            foodTimer += Time.deltaTime;
        else if (feederTggl.isOn)
        {
            Spawn(foodGO, foodProduction);
            foodTimer = 0;
        }
    }

    // a generic way to spawn needed gameobjects on map
    private void Spawn(GameObject obj, int num, System.Action<GameObject> onSpawn = null)
    {
        GameObject[] list = new GameObject[num];
        for (int i = 0; i < num; i++)
        {
            list[i] = Instantiate(obj, new Vector3(Random.Range(-size, size), 0, Random.Range(-size, size)), Quaternion.identity);
            Destroy(list[i], foodLifetime);

            if(onSpawn != null)
                onSpawn(list[i]);
        }
    }

    // public method to set time scale from external scripts / UI
    public void SetTimeScale(float scale) => Time.timeScale = scale;


    // public method to hatch custom eggs from external scripts / UI
    public void HatchEgg(EggSO egg)
    {
        Spawn(animalGO, Random.Range(egg.minChildren, egg.maxChildren), (animalObj) => 
        {
            Animal animal = animalObj.GetComponent<Animal>();
            egg.HatchAnimal(animal);
            animal.habitat = this;
        });
    }

    // public methods that update internal variables from sliders
    #region SliderUpdateFunctions

    public void UpdateSize(Slider slider)
    {
        size = (float)slider.value;
        sizeTxt.text = "Size: " + size.ToString("F1") + "m";

        // phyically scale the habitat, and move the camera to fit
        habitatPlane.transform.localScale = new Vector3(size / 4, 1, size / 4);
        cam.transform.localPosition = new Vector3(0, size * 2.5f, 0);
    }

    public void UpdateTemp(Slider slider)
    {
        temperature = (int)slider.value;
        tempTxt.text = "Temperature: " + temperature + "C";
        
        // set the sun colour to change between 2 colours depending on the tempurature
        sun.colorTemperature = (((float)temperature + 50) / 100) * (lowColorTemp - highColorTemp) + highColorTemp;
    }

    public void UpdateHumidity(Slider slider)
    {
        humidity = (int)slider.value;
        humidityTxt.text = "Humidity: " + humidity + "%";
    }

    public void UpdateFoodProduction(Slider slider)
    {
        foodProduction = (int)slider.value;
        foodPerTxt.text = "Food / Sec: " + foodProduction;
    }

    #endregion
}
