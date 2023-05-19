using UnityEngine;

// allows you to create scriptable objects for customizing starting stats when hatching animals
[CreateAssetMenu(menuName = "Egg")]
public class EggSO : ScriptableObject
{
    public string animalGroup; // used to track which egg an animal hatched from
    [Space]
    [Min(1)] public int minChildren = 5; // min amount of children that can be produced when birthing
    [Min(1)] public int maxChildren = 10; // and max amount
    [Space]
    public int comfortTemp = 20; // ideal temperature in Celsius
    public int comfortMoisture = 10; // ideal humidity in %
    [Space]
    [Min(0.1f)] public float size = 25; // size in cm
    [Min(1)] public int constitution = 10; // how resiliant the animal is
    [Min(1)] public int strength = 5; // eventually used for measuring how good the animal is at attacking
    [Min(1)] public int dexterity = 5; // how fast the animal is
    [Min(1)] public int sensing = 5; // how far the animal can see

    // you can set an Animal to copy this eggs base stats for when hatching
    public void HatchAnimal(Animal anim)
    {
        anim.animalGroup = animalGroup;

        anim.comfortTemp = comfortTemp;
        anim.comfortMoisture = comfortMoisture;

        anim.size = size;
        anim.constitution = constitution;
        anim.strength = strength;
        anim.dexterity = dexterity;
        anim.sensing = sensing;
    }
}
