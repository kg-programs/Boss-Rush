using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearThrowSpawn : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject spear;
    [SerializeField] float spearSpeed;

    public void SpawnSpear()
    {
        GameObject spearObj = Instantiate(spear, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
        Rigidbody spearRig = spearObj.GetComponent<Rigidbody>();
        spearRig.AddForce(spearRig.transform.forward * spearSpeed);
        Destroy(spearObj, 3f);
    }
}
