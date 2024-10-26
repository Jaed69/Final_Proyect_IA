using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent 
{
    //Coin variables
    [SerializeField] private Transform target;
    public int coinCount = 1;
    public GameObject food;
    [SerializeField] private List<GameObject> spawnedCoinsList = new List<GameObject>();


    //Agent variables
    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody rb;

    //Environment variables
    [SerializeField] private Transform environmentLocation;
    Material envMaterial;
    public GameObject env;


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        envMaterial = env.GetComponent<Renderer>().material;
    }

    public override void OnEpisodeBegin()
    {
        //Agent
        transform.localPosition = new Vector3(Random.Range(-4f,4f), 0.3f, Random.Range(-4f, 4f));

        //Coin
        CreateCoin();

    }

    private void CreateCoin()
    {

        if (spawnedCoinsList.Count > 0)
        {
            RemoveCoin(spawnedCoinsList);
        }

        for (int i = 0; i < coinCount; i++)
        {
            //Spawing coin
            GameObject newCoin = Instantiate(food);
            //Make coin a child of the agent
            newCoin.transform.parent = environmentLocation;
            //Give random spawn location
            Vector3 coinLocation = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));
            //"Spawn" in new location
            newCoin.transform.localPosition = coinLocation;
            spawnedCoinsList.Add(newCoin);
        }
    }

    private void RemoveCoin(List<GameObject> toBeDeletedGameObjectList)
    {
        foreach (GameObject i in toBeDeletedGameObjectList)
        {
            Destroy(i.gameObject);
        }
        toBeDeletedGameObjectList.Clear();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);


        /*
        Vector3 velocity = new Vector3(moveX, 0f, moveZ);
        velocity = velocity.normalized * Time.deltaTime * moveSpeed;

        transform.localPosition += velocity;
        */
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Coin"))
        {
            //Remove from list
            spawnedCoinsList.Remove(other.gameObject);
            Destroy(other.gameObject);
            AddReward(10f);
            if(spawnedCoinsList.Count == 0)
            {
                envMaterial.color = Color.green;
                RemoveCoin(spawnedCoinsList);
                AddReward(5f);
                EndEpisode();
            }
        }
        if(other.gameObject.CompareTag("Wall"))
        {
            envMaterial.color = Color.red;
            RemoveCoin(spawnedCoinsList);
            AddReward(-15f);
            EndEpisode();
        }

    }


}
