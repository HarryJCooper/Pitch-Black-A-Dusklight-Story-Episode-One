using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class FieldOfView : MonoBehaviour
{
    float frequencyReduction, volumeReduction;

    public float viewRadiusFocused;
    [Range(0, 360)]
    public float viewAngle;
    
    public float viewRadiusStandard;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();
    public List<Transform> invisibleTargets = new List<Transform>();
    public List<Transform> frontTargets = new List<Transform>();
    public List<Transform> notFrontTargets = new List<Transform>();
    public List<Transform> directBehindTargets = new List<Transform>();
    public List<Transform> notDirectBehindTargets = new List<Transform>();

    public Collider[] allSourcesInRadius;

    public ChangeAfterFocus component;
    public ChangeAfterFocus components;

    bool waitAMoment;

    public bool focused;

    public float timer;

    public RaycastHit[] hits;

    private void Start()
    {
        StartCoroutine(FindTargetsWithDelay(.5f));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    
    
    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        invisibleTargets.Clear();
        frontTargets.Clear();
        notFrontTargets.Clear();
        directBehindTargets.Clear();
        notDirectBehindTargets.Clear();

        Collider[] positionalAudioSources = Physics.OverlapSphere(transform.position, viewRadiusFocused);

        // DOES A SPHERE CHECKING FOR ALL COLLIDERS

        for (int i = 0; i < positionalAudioSources.Length; i++)
        {

            Transform target = positionalAudioSources[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float dstToTarget = Vector3.Distance(transform.position, target.position);

            // CHECKS TO SEE IF OCCLUDED BY OBSTACLE
            hits = Physics.RaycastAll(transform.position, dirToTarget, dstToTarget, obstacleMask, QueryTriggerInteraction.Ignore);
            
            foreach (RaycastHit hit in hits)
            {
                if (target.GetComponent<OccludedObject>())
                {
                    // TELLS IS OCCLUDED
                    frequencyReduction = hit.transform.gameObject.GetComponent<OcclusionObject>().frequencyReduction;
                    volumeReduction = hit.transform.gameObject.GetComponent<OcclusionObject>().volumeReduction * hits.Length;
                    target.GetComponent<OccludedObject>().occluded = true;
                    target.GetComponent<OccludedObject>().frequencyReduction = frequencyReduction;
                    target.GetComponent<OccludedObject>().volumeReduction = volumeReduction;
                }    
            }

            // TO SEE IF BETTER TO ALLOW FOCUS WHEN OBJECT OCCLUDED
            if (!(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2))
            {
                if (target.gameObject.GetComponent<ChangeAfterFocus>())
                {
                    invisibleTargets.Add(target);
                }
            }
            if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, targetMask, QueryTriggerInteraction.Collide))
            {
                if (target.gameObject.GetComponent<ChangeAfterFocus>())
                {
                    visibleTargets.Add(target);
                }
            }

            // IF OBSTACLE ISNT IN THE WAY REMOVE OCCLUSION AND MARK FALSE
            if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask, QueryTriggerInteraction.Collide))
            {
                if (target.gameObject.GetComponent<OccludedObject>())
                {
                    target.gameObject.GetComponent<OccludedObject>().occluded = false;
                }
                // CHECKS IF IN FRONT OF THE TARGET
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {

                    // DOES THE RAYCAST TO CHECK IF THE TARGET HAS THE RIGHT TYPE OF MASK (POSITIONAL AMBIENCE)
                    if (Physics.Raycast(transform.position, dirToTarget, dstToTarget))
                    {
                        if (target.gameObject.GetComponent<IncreaseSourceVolume>())
                        {
                            frontTargets.Add(target);

                        }
                    }
                }

                // IF GAMEOBJECT IS NOT IN FRONT OF YOU, ADDS TO INVISIBLE TARGETS AND NOT FRONT TARGETS
                if (!(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2))
                {
                    if (target.gameObject.GetComponent<IncreaseSourceVolume>())
                    {
                        notFrontTargets.Add(target);
                    }
                }

                // CHECKS IF TARGET IS DIRECTLY BEHIND, LOWERS VOLUME IF SO
                if (Vector3.Angle(transform.forward, dirToTarget) > 160)
                {
                    if (target.gameObject.GetComponent<IncreaseSourceVolume>())
                    {
                        directBehindTargets.Add(target);
                    }
                }
                // IF NOT, RESTORES VOLUME
                else if (Vector3.Angle(transform.forward, dirToTarget) < 160)
                {
                    if (target.gameObject.GetComponent<IncreaseSourceVolume>())
                    {
                        notDirectBehindTargets.Add(target);
                    }
                }
            }
        }
    }

    // CHECK HOW MANY WALLS IN THE WAY OF ANY POSITIONAL AUDIO SOURCE WITHIN A GIVEN RADIUS
    // IF THEY ARE IN THE WAY, CHECK WHAT OCCLUSION VALUES EACH WALL HAS, THIS IS VOLUME AND ROLL OFF
    // ADD VOLUMES TOGETHER (IF POSITIVE) TAKE AWAY, ONLY APPLY ROLL OFF IF LOWER
    // APPLY THAT TO THE LOW PASS AND VOLUME OF AN OBJECT
    



    void Update()
    {
        
        foreach (Transform target in visibleTargets)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                target.gameObject.GetComponent<ChangeAfterFocus>().hit = true;
            }
        }

        foreach (Transform target in frontTargets)
        {
            target.gameObject.GetComponent<IncreaseSourceVolume>().inView = true;
        }

        foreach (Transform target in invisibleTargets)
        {
            target.gameObject.GetComponent<ChangeAfterFocus>().hit = false;
        }

        foreach (Transform target in notFrontTargets)
        {
            target.gameObject.GetComponent<IncreaseSourceVolume>().inView = false;
        }

        foreach (Transform target in directBehindTargets)
        {
            target.gameObject.GetComponent<IncreaseSourceVolume>().directBehind = true;
        }

        foreach (Transform target in notDirectBehindTargets)
        {
            target.gameObject.GetComponent<IncreaseSourceVolume>().directBehind = false;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            timer = 0;
            focused = true;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            timer += Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            focused = false;
        }
        
        
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    
}

