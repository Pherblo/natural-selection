using UnityEngine;

// base class for all animal states
public abstract class AnimalState
{
    public Animal animal;
    public Transform transform;

    public AnimalState(Animal animal, Transform transform)
    {
        this.transform = transform; 
        this.animal = animal;
    }

    public abstract string StateName(); // used to view current state

    public virtual void OnUpdate(System.Action<AnimalState> changeState) { } // called every frame from external script

    // moves the animal forward based on its dexterity
    public void MoveForward()
    {
        transform.position += transform.forward * animal.dexterity * Time.deltaTime;

        // clamp position so it doesnt fall off the map
        float mapSize = animal.habitat.size;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -mapSize, mapSize), transform.position.y, Mathf.Clamp(transform.position.z, -mapSize, mapSize));
    }

    // utility function to get the closest object from an array
    public RaycastHit GetClosest(RaycastHit[] hits)
    {
        RaycastHit hit = hits[0];
        foreach (var item in hits)
        {
            if (Vector3.Distance(hit.transform.position, transform.position) > Vector3.Distance(item.transform.position, transform.position))
                hit = item;
        }
        return hit;
    }
}

// state to have the animal wander
public class Wandering : AnimalState
{
    const float wanderTime = 1f; // how long the animal wonders before changing direction
    private float timer;

    public Wandering(Animal animal, Transform transform) : base(animal, transform) {}

    public override string StateName() => "Wandering";

    public override void OnUpdate(System.Action<AnimalState> changeState)
    {
        // move forward
        if(timer < wanderTime)
        {
            timer += Time.deltaTime;
            MoveForward();
        }

        // after timer, change direction randomly
        else
            changeState(new Scanning(animal, transform, Random.Range(-360, 360)));

        // look for any nearby food
        var list = Physics.SphereCastAll(transform.position + transform.forward * (animal.sensing / 2), animal.sensing, Vector3.forward, animal.sensing, animal.food);

        // find closest food target if available and chase it
        if (list.Length > 0)
            changeState(new Hunting(animal, transform, GetClosest(list).transform));
    }
}

// state to have the animal rotate a set amount of degrees
public class Scanning : AnimalState
{
    private const float secPerFullTurn = 1f; // speed of turn
    private float rotateTime; // how much time it will take to spin the set amount of degrees

    private float rotateTimer = 0;
    private Quaternion target;
    private Quaternion orig;

    public Scanning(Animal animal, Transform transform, float degrees) : base(animal, transform)
    {
        orig = transform.rotation;
        target = Quaternion.Euler(new Vector3(0, transform.rotation.y + degrees, 0));
        rotateTime = (degrees / 360) * secPerFullTurn;
    }

    public override string StateName() => "Scanning";

    public override void OnUpdate(System.Action<AnimalState> changeState)
    {
        // spin around for a set amount of degrees
        transform.rotation = Quaternion.Lerp(orig, target, rotateTimer / rotateTime);

        // once you're done rotating, go back to wandering
        if(rotateTimer < rotateTime)
            rotateTimer += Time.deltaTime;
        else
            changeState(new Wandering(animal, transform));

        // look for any nearby food
        var list = Physics.SphereCastAll(transform.position + transform.forward * (animal.sensing / 2), animal.sensing, Vector3.forward, animal.sensing, animal.food);

        // find closest food target if available and chase it
        if (list.Length > 0)
            changeState(new Hunting(animal, transform, GetClosest(list).transform));
    }
}

// state to have animal chase a target
public class Hunting : AnimalState
{
    private Transform target;

    public Hunting(Animal animal, Transform transform, Transform target) : base(animal, transform)
    {
        this.target = target;
    }

    public override string StateName() => "Hunting";

    public override void OnUpdate(System.Action<AnimalState> changeState)
    {
        // if target is lost or eaten, scan again
        if (target == null || (target.position - transform.position).magnitude > animal.sensing * 2)
            changeState(new Scanning(animal, transform, Random.Range(-360, 360)));

        else
        {
            // otherwise look at and move towards target
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            MoveForward();
        }
    }
}