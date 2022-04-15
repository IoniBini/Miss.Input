using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public int enemyHealth = 3;

    //damage variables
    public bool isHit = false;
    public bool beingDamaged = false;
    public float hitDelay = 0.5f;

    //materials used to make enemy flash when hit
    public Material damagedShader;
    public Material regularShader;
    public Material skeletonBody;

    //enemy movement variables
    public Vector3 enemyMovement;
    public float enemySpeed = 10f;
    private Rigidbody enemyController;
    public float orientation = 1;

    private void Start()
    {
        //floorDetection = this.gameObject.transform.GetChild(0).GetComponent<>
        enemyController = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        enemyMovement = new Vector3(orientation, 0f, 0f);

        //when the enemy is struck, it deducts one health point and returns isHit to false
        if (isHit == true && beingDamaged == false)
        {
            StartCoroutine(Damage());
        }
    }

    private void FixedUpdate()
    {
        moveEnemy(enemyMovement);
    }

    void moveEnemy(Vector3 direction)
    {
        enemyController.MovePosition(transform.position + (direction * enemySpeed * Time.deltaTime));
    }

    IEnumerator Damage()
    {
        //needs to immidetally become false again, otherwise the number goes does super fast
        enemyHealth--;
        isHit = false;

        //prevents the player from spamming and killing the enemy instantly
        beingDamaged = true;

        //enemy self destructs when health is 0
        if (enemyHealth == 0)
        {
            Destroy(this.gameObject);
        }

        //briefly changes the material so it flashes, then returns it to normal
        this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material = damagedShader;

        yield return new WaitForSeconds(hitDelay);

        beingDamaged = false;

        this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material = regularShader;
    }

    public void Flip()
    {
        var currentRotation = GetComponent<Transform>().eulerAngles.y;

        GetComponent<Transform>().eulerAngles = new Vector3(0, currentRotation * -1, 0);
        orientation = orientation * -1;
    }
}
