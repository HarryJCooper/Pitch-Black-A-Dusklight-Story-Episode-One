using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimals : MonoBehaviour
{
    public bool moveAnimals0, moveAnimals1, moveAnimals2;
    public GameObject elephants, monkeys;
    public float speedMultiplier, maxZ0, maxZ1, maxZ2;
    

    // THIS IS THE 'CONTROLLER SCRIPT'

    // THESE ARE SET BY THREE TRIGGERS

    // HAVE AN 'ELEPHANT BEHAVIOR' ETC. SCRIPT ON EACH ANIMAL WHICH IS ALSO TURNED ON BY TRIGGER
    

    void Update()
    {
        if (moveAnimals0 && (elephants.transform.position.z < maxZ0))
        {
            elephants.transform.position += Vector3.forward * Time.deltaTime * speedMultiplier;
            monkeys.transform.position += Vector3.forward * Time.deltaTime * speedMultiplier;
        }

        if (moveAnimals1 && (elephants.transform.position.z < maxZ1))
        {
            elephants.transform.position += Vector3.forward * Time.deltaTime * speedMultiplier;
            monkeys.transform.position += Vector3.forward * Time.deltaTime * speedMultiplier;
        }

        if (moveAnimals2 && (elephants.transform.position.z < maxZ2))
        {
            elephants.transform.position += Vector3.forward * Time.deltaTime * speedMultiplier;
            monkeys.transform.position += Vector3.forward * Time.deltaTime * speedMultiplier;
        }
    }
}
