using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    // Coin variables
    [SerializeField] private Transform target;
    public int coinCount;
    public GameObject food;
    [SerializeField] private List<GameObject> spawnedCoinsList = new List<GameObject>();

    // Agent variables
    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody rb;

    // Environment variables
    [SerializeField] private Transform environmentLocation;
    Material envMaterial;
    public GameObject env;

    // Time keeping variables
    [SerializeField] private int timeForEpisode;
    private float timeLeft;

    private LayerMask wallLayer;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        envMaterial = env.GetComponent<Renderer>().material;
        wallLayer = LayerMask.GetMask("Wall"); // Define the layer for walls
    }

    public override void OnEpisodeBegin()
    {
        // Agent
        transform.localPosition = new Vector3(Random.Range(-3.5f, 3.5f), 0.3f, Random.Range(-3.5f, 3.5f));

        // Coin
        CreateCoin();

        // Timer to determine when episode ends
        EpisodeTimerNew();
    }

    private void Update()
    {
        CheckRemainingTime();
    }

    private void CreateCoin()
    {
        if (spawnedCoinsList.Count > 0)
        {
            RemoveCoin(spawnedCoinsList);
        }

        for (int i = 0; i < coinCount; i++)
        {
            // Spawning coin
            Vector3 coinLocation = GetValidPosition();
            GameObject newCoin = Instantiate(food, coinLocation, Quaternion.identity, environmentLocation);
            spawnedCoinsList.Add(newCoin);
        }
    }

    private Vector3 GetValidPosition()
    {
        Vector3 position;
        int maxAttempts = 10;
        float coinRadius = 0.5f; // Adjust this according to your coin's size

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            position = new Vector3(Random.Range(-3.5f, 3.5f), 0.3f, Random.Range(-3.5f, 3.5f));

            // Check if there are collisions with other coins or walls
            Collider[] hitColliders = Physics.OverlapSphere(position, coinRadius, wallLayer);
            if (hitColliders.Length == 0) // No collisions with walls
            {
                bool isPositionValid = true;
                foreach (var coin in spawnedCoinsList)
                {
                    if (Vector3.Distance(position, coin.transform.localPosition) < 1f) // Adjust 1f based on coin radius
                    {
                        isPositionValid = false;
                        break;
                    }
                }
                if (isPositionValid)
                {
                    return position;
                }
            }
        }

        return new Vector3(0, 0.3f, 0); // Backup position if no valid position found
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
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            spawnedCoinsList.Remove(other.gameObject);
            Destroy(other.gameObject);
            AddReward(10f);
            if (spawnedCoinsList.Count == 0)
            {
                envMaterial.color = Color.green;
                RemoveCoin(spawnedCoinsList);
                AddReward(5f);
                EndEpisode();
            }
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            envMaterial.color = Color.red;
            RemoveCoin(spawnedCoinsList);
            AddReward(-15f);
            EndEpisode();
        }
    }

    private void EpisodeTimerNew()
    {
        timeLeft = Time.time + timeForEpisode;
    }

    private void CheckRemainingTime()
    {
        if (Time.time >= timeLeft)
        {
            envMaterial.color = Color.blue;
            RemoveCoin(spawnedCoinsList);
            AddReward(-15f);
            EndEpisode();
        }
    }
}
