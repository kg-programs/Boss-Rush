using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltScript : MonoBehaviour
{
    public Transform spawnPointNorth;
    public Transform spawnPointSouth;
    public Transform spawnPointEast;
    public Transform spawnPointWest;
    public Transform spawnPointNE;
    public Transform spawnPointSE;
    public Transform spawnPointNW;
    public Transform spawnPointSW;
    
    public GameObject spear;
    [SerializeField] float spearSpeed;

    public void CastUlt()
    {
        GameObject spearObjNorth = Instantiate(spear, spawnPointNorth.transform.position, spawnPointNorth.transform.rotation) as GameObject;
        GameObject spearObjSouth = Instantiate(spear, spawnPointSouth.transform.position, spawnPointSouth.transform.rotation) as GameObject;
        GameObject spearObjEast = Instantiate(spear, spawnPointEast.transform.position, spawnPointEast.transform.rotation) as GameObject;
        GameObject spearObjWest = Instantiate(spear, spawnPointWest.transform.position, spawnPointWest.transform.rotation) as GameObject;
        GameObject spearObjNE = Instantiate(spear, spawnPointNE.transform.position, spawnPointNE.transform.rotation) as GameObject;
        GameObject spearObjSE = Instantiate(spear, spawnPointSE.transform.position, spawnPointSE.transform.rotation) as GameObject;
        GameObject spearObjNW = Instantiate(spear, spawnPointNW.transform.position, spawnPointNW.transform.rotation) as GameObject;
        GameObject spearObjSW = Instantiate(spear, spawnPointSW.transform.position, spawnPointSW.transform.rotation) as GameObject;

        Rigidbody spearRigNorth = spearObjNorth.GetComponent<Rigidbody>();
        Rigidbody spearRigSouth = spearObjSouth.GetComponent<Rigidbody>();
        Rigidbody spearRigEast = spearObjEast.GetComponent<Rigidbody>();
        Rigidbody spearRigWest = spearObjWest.GetComponent<Rigidbody>();
        Rigidbody spearRigNE = spearObjNE.GetComponent<Rigidbody>();
        Rigidbody spearRigSE = spearObjSE.GetComponent<Rigidbody>();
        Rigidbody spearRigNW = spearObjNW.GetComponent<Rigidbody>();
        Rigidbody spearRigSW = spearObjSW.GetComponent<Rigidbody>();

        spearRigNorth.AddForce(spearRigNorth.transform.forward * spearSpeed);
        spearRigSouth.AddForce(spearRigSouth.transform.forward * spearSpeed);
        spearRigEast.AddForce(spearRigEast.transform.forward * spearSpeed);
        spearRigWest.AddForce(spearRigWest.transform.forward * spearSpeed);
        spearRigNE.AddForce(spearRigNE.transform.forward * spearSpeed);
        spearRigSE.AddForce(spearRigSE.transform.forward * spearSpeed);
        spearRigNW.AddForce(spearRigNW.transform.forward * spearSpeed);
        spearRigSW.AddForce(spearRigSW.transform.forward * spearSpeed);

        Destroy(spearObjNorth, 3f);
        Destroy(spearObjSouth, 3f);
        Destroy(spearObjEast, 3f);
        Destroy(spearObjWest, 3f);
        Destroy(spearObjNE, 3f);
        Destroy(spearObjSE, 3f);
        Destroy(spearObjNW, 3f);
        Destroy(spearObjSW, 3f);
    }
}
