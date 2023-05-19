using UnityEngine;

public class Animal : MonoBehaviour
{
    [Header("Stats")]
    public string animalGroup; // used to track which egg an animal hatched from
    [Space]
    public int minChildren = 5; // min amount of children that can be produced when birthing
    public int maxChildren = 10; // and max amount
    [Space]
    public int comfortTemp = 20; // ideal temperature in Celsius
    public int comfortMoisture = 10; // ideal humidity in %
    [Space]
    public float size = 25; // size in cm
    public int constitution = 10; // how resiliant the animal is
    public int strength = 5; // eventually used for measuring how good the animal is at attacking
    public int dexterity = 5; // how fast the animal is
    public int sensing = 5; // how far the animal can see

    [Header("Misc")]
    public MeshRenderer meshRenderer; // meshrenderer to change the color of depending on stats
    public Habitat habitat; // reference to habitats to read current conditions
    [Space]
    public LayerMask food; // layer to look for food on
    [Space]
    public float breedCost; // how much energy it costs to breed
    public float energyToBreed; // how much energy the animal will need before it will breed. prevents running out of energy
    public int foodEnergy; // how much energy food gives
    [Space]
    public float mutationChance; // chance to mutate
    public int mutationAmount; // how much a mutated stat can change up to
    [Space]
    public float energy; // current energy
    public float energyCost; // energy used per second by animal
    public float timeToLive; // how long the animal will live for without eating

    private float energyTimer;
    private AnimalState currentState;

    void Start()
    {
        // when an animal hatches, mutate it
        MutateStats();

        // set the starting state to be scanning
        currentState = new Scanning(this, transform, Random.Range(-360, 360));

        // set starting energy to be size
        energy = size;

        // calculate the energy cost based from stats, and calculate how long the animal can live without eating
        energyCost = (float)(constitution + dexterity + sensing + size) / 10f;
        timeToLive = size / energyCost;

        // change colour based off stats
        // RED = strength
        // GREEN = constitution
        // BLUE = sensing
        meshRenderer.material.color = new Color((float)strength / 10, (float)constitution / 10, (float)sensing / 10, 255);

        // set physical scale to match size
        transform.localScale = new Vector3(size/20, size / 20, size / 20);
    }

    // chance to slightly alter all stats
    public void MutateStats()
    {
        // based off random mutation chance, add or remove stat points based from a random mutation amount
        if (Random.Range(0f, 1f) < mutationChance)
            dexterity += Random.Range(-mutationAmount, mutationAmount);
        if (Random.Range(0f, 1f) < mutationChance)
            strength += Random.Range(-mutationAmount, mutationAmount);
        if (Random.Range(0f, 1f) < mutationChance)
            constitution += Random.Range(-mutationAmount, mutationAmount);
        if (Random.Range(0f, 1f) < mutationChance)
            sensing += Random.Range(-mutationAmount, mutationAmount);

        if (Random.Range(0f, 1f) < mutationChance)
            size += Random.Range(-mutationAmount * 2, mutationAmount * 2);
        if (Random.Range(0f, 1f) < mutationChance)
            comfortTemp += Random.Range(-mutationAmount * 2, mutationAmount * 2);
        if (Random.Range(0f, 1f) < mutationChance)
            comfortMoisture += Random.Range(-mutationAmount * 2, mutationAmount * 2);

        // prevent values from falling bellow what they should be..
        strength = Mathf.Max(strength, 1);
        constitution = Mathf.Max(constitution, 1);
        dexterity = Mathf.Max(strength, 1);
        sensing = Mathf.Max(constitution, 1);
        size = Mathf.Max(size, 0.1f);
    }

    // allow external scripts to feed the animal
    public void FeedAnimal(int amount) => energy += amount;

    // allow external scripts to read the current state
    public AnimalState GetCurrentState() => currentState;

    void Update()
    {
        // make sure animal is comfortable in habitat, otherwise destroy it
        if(Mathf.Abs(habitat.temperature - comfortTemp) > constitution * 2 ||
            habitat.humidity - comfortMoisture > constitution * 2)
            Destroy(gameObject);

        // update state to move, hunt, etc.
        currentState.OnUpdate((state)=>
        {
            currentState = state;
        });

        // remove energy every second
        if (energyTimer < 1)
            energyTimer += Time.deltaTime;
        else
        {
            energy -= energyCost;
            energyTimer = 0;
        }

        // destroy the animal if it ran out of energy
        if (energy <= 0)
            Destroy(gameObject);

        // otherwise breed if enough energy
        else if(energy >= energyToBreed)
        {
            energy -= breedCost;
            for (int i = 1; i < Random.Range(minChildren, maxChildren) + 1; i++)
            {
                Instantiate(gameObject, new Vector3(transform.position.x + i, 1, transform.position.z), Quaternion.identity);
            }
        }
    }

    // when the animal collides...
    private void OnCollisionStay(Collision other)
    {
        // if you collide with food, eat it!
        if(other.collider.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            FeedAnimal(foodEnergy);
        }
    }

    private void OnDrawGizmos()
    {
        // draw a red sphere representing how far it can see
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * (sensing / 2), sensing);
    }
}
