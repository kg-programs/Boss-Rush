using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearShotgun : MonoBehaviour
{
    public Transform spawnPointLeft;
    public Transform spawnPointMid;
    public Transform spawnPointRight;
    public GameObject spear;
    [SerializeField] float spearSpeed;

    public void ShootSpears()
    {
        GameObject spearObjLeft = Instantiate(spear, spawnPointLeft.transform.position, spawnPointLeft.transform.rotation) as GameObject;
        GameObject spearObjMid = Instantiate(spear, spawnPointMid.transform.position, spawnPointMid.transform.rotation) as GameObject;
        GameObject spearObjRight = Instantiate(spear, spawnPointRight.transform.position, spawnPointRight.transform.rotation) as GameObject;
        
        Rigidbody spearRigLeft = spearObjLeft.GetComponent<Rigidbody>();
        Rigidbody spearRigMid = spearObjMid.GetComponent<Rigidbody>();
        Rigidbody spearRigRight = spearObjRight.GetComponent<Rigidbody>();
        
        spearRigLeft.AddForce(spearRigLeft.transform.forward * spearSpeed);
        spearRigMid.AddForce(spearRigMid.transform.forward * spearSpeed);
        spearRigRight.AddForce(spearRigRight.transform.forward * spearSpeed);
        
        Destroy(spearObjLeft, 3f);
        Destroy(spearObjMid, 3f);
        Destroy(spearObjRight, 3f);
    }
}
