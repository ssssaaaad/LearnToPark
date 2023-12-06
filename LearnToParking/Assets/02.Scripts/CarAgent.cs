using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarAgent : Agent
{

    [SerializeField] Transform target;
    [SerializeField] Transform startPoint;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Material winM;
    [SerializeField] Material loseM;
    [SerializeField] MeshRenderer floorMR;

    [SerializeField] Transform[] parkingPos;
    [SerializeField] Transform[] obstacleObjects;
    [SerializeField] Transform[] guides;

    public float moveSpeed = 2;
    public float rotationSpeed = 2;

    float distanceGoal = 10;
    float rotationGoal = 50;
    float accumulate = 0;
    float angle = 0;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = startPoint.localPosition;
        transform.localRotation = startPoint.localRotation;

        int index = 2;

        distanceGoal = 10;
        rotationGoal = 50;
        accumulate = 0;
        angle = 0;
        for (int i = 0; i < parkingPos.Length; i++)
        {
            obstacleObjects[i].localPosition = Vector3.zero;
            obstacleObjects[i].localRotation = Quaternion.identity;

            if (i != index)
            {
                parkingPos[i].GetComponent<MeshRenderer>().enabled = false;
                parkingPos[i].tag = "Untagged";
                obstacleObjects[i].gameObject.SetActive(true);
            }
            else
            {
                parkingPos[i].GetComponent<MeshRenderer>().enabled = true; 
                parkingPos[i].tag = "Goal";
                obstacleObjects[i].gameObject.SetActive(false);
            }
        }
        target = parkingPos[index];
        target.position = parkingPos[index].position;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition); // x, y ,z  3개
        sensor.AddObservation(target.localRotation);
        sensor.AddObservation(transform.localPosition);// x, y, z 3개
        sensor.AddObservation(transform.localRotation);

        sensor.AddObservation(rigidbody.velocity.x); // 1개
        sensor.AddObservation(rigidbody.velocity.x); // 1개

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float x = actions.ContinuousActions[0];
        float y = actions.ContinuousActions[1];
        transform.position += transform.forward * x * moveSpeed * Time.deltaTime;
        transform.rotation *= Quaternion.Euler(new Vector3(0, y, 0)* rotationSpeed);

    }

    private void LateUpdate()
    {
        if (Vector3.Distance(transform.position, target.position) < distanceGoal)
        {
            SetReward(1 / distanceGoal);

            Debug.Log("dis : " + 1 / distanceGoal);
            distanceGoal = Vector3.Distance(transform.position, target.position) - 2f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            if (Vector3.Distance(transform.position, other.transform.position) < 0.8f)
            {
                
                SetReward(10f);
                floorMR.material = winM;
                EndEpisode();
                
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            accumulate += -0.1f;
            SetReward(-0.1f);
            if(accumulate <= -1)
            {
                floorMR.material = loseM;
                EndEpisode();
            }
        }
        else if (collision.gameObject.tag == "Wall")
        {
            SetReward(-2f);
            floorMR.material = loseM;
            EndEpisode();
        }
      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            if (!check)
            {
                Debug.Log("goal");
                SetReward(5f);
                check = true;
            }
        }
    }
    bool check = false;
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxis("Vertical");
        continuousAction[1] = Input.GetAxis("Horizontal");
    }
}
