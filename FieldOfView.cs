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

    public bool focused;
    public float timer;
    public RaycastHit[] hits;

    [SerializeField] Controls controls;

    private void Start(){StartCoroutine(FindTargetsWithDelay(.1f));}

    IEnumerator FindTargetsWithDelay(float delay){
        while (true){
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void ClearTargets(){
        visibleTargets.Clear();
        invisibleTargets.Clear();
        frontTargets.Clear();
        notFrontTargets.Clear();
        directBehindTargets.Clear();
        notDirectBehindTargets.Clear();
    }

    void SetOcclusion(RaycastHit hit, Transform target){
        OccludedObject occludedObject = target.GetComponent<OccludedObject>() ?? null;
        if (occludedObject){
            OcclusionObject occlusionObject = hit.transform.gameObject.GetComponent<OcclusionObject>();
            float frequencyReduction = occlusionObject.frequencyReduction;
            float volumeReduction = occlusionObject.volumeReduction * hits.Length;
            occludedObject.occluded = true;
            occludedObject.frequencyReduction = frequencyReduction;
            occludedObject.volumeReduction = volumeReduction;
        }    
    }

    void SetOcclusionToFalse(Transform target){
        OccludedObject occludedObject = target.gameObject.GetComponent<OccludedObject>() ?? null;
        if (occludedObject) occludedObject.occluded = false;
    }

    void CheckIfVisible(Transform target, Vector3 dirToTarget, float dstToTarget){
        if (!(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)) invisibleTargets.Add(target);
        if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, targetMask, QueryTriggerInteraction.Collide)) visibleTargets.Add(target);
    }

    void CheckInView(Transform target, Vector3 dirToTarget, float dstToTarget){
        if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2) frontTargets.Add(target);
        if (!(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)) notFrontTargets.Add(target);
        if (Vector3.Angle(transform.forward, dirToTarget) >= 160){
            directBehindTargets.Add(target); 
            return;
        }
        notDirectBehindTargets.Add(target);
    }
    
    void FindVisibleTargets(){
        ClearTargets();
        Collider[] positionalAudioSources = Physics.OverlapSphere(transform.position, viewRadiusFocused);
        // DOES A SPHERE CHECKING FOR ALL COLLIDERS
        for (int i = 0; i < positionalAudioSources.Length; i++){
            Transform target = positionalAudioSources[i].transform;

            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float dstToTarget = Vector3.Distance(transform.position, target.position);
            // CHECKS TO SEE IF OCCLUDED BY OBSTACLE
            hits = Physics.RaycastAll(transform.position, dirToTarget, dstToTarget, obstacleMask, QueryTriggerInteraction.Ignore);
            foreach (RaycastHit hit in hits) SetOcclusion(hit, target);

            ChangeAfterFocus changeAfterFocus = target.gameObject.GetComponent<ChangeAfterFocus>() ?? null;
            if (changeAfterFocus) CheckIfVisible(target, dirToTarget, dstToTarget);
            // IF OBSTACLE ISNT IN THE WAY REMOVE OCCLUSION AND MARK FALSE
            if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask, QueryTriggerInteraction.Collide)){
                SetOcclusionToFalse(target);
                IncreaseSourceVolume increaseSourceVolume = target.gameObject.GetComponent<IncreaseSourceVolume>() ?? null;
                if (increaseSourceVolume) CheckInView(target, dirToTarget, dstToTarget);
            }
        }
    }

    void SetInViewHitAndDirectBehind(){
        foreach (Transform target in visibleTargets) if (controls.inZoom) target.gameObject.GetComponent<ChangeAfterFocus>().hit = true;
        foreach (Transform target in frontTargets) target.gameObject.GetComponent<IncreaseSourceVolume>().inView = true;
        foreach (Transform target in invisibleTargets) target.gameObject.GetComponent<ChangeAfterFocus>().hit = false;
        foreach (Transform target in notFrontTargets) target.gameObject.GetComponent<IncreaseSourceVolume>().inView = false;
        foreach (Transform target in directBehindTargets) target.gameObject.GetComponent<IncreaseSourceVolume>().directBehind = true;
        foreach (Transform target in notDirectBehindTargets) target.gameObject.GetComponent<IncreaseSourceVolume>().directBehind = false;
    }

    void Update(){
        SetInViewHitAndDirectBehind();
        if (controls.enteredZoom){
            timer = 0;
            focused = true;
        }
        if (controls.inZoom) timer += Time.deltaTime;
        if (!controls.inZoom) focused = false;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal){
        if (!angleIsGlobal) angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    } 
}

