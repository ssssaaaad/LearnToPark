using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MoveTo : Agent
{
    [SerializeField] Transform targetT;
    [SerializeField] Material winM;
    [SerializeField] Material loseM;
    [SerializeField] MeshRenderer floorMR;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-2f, 2), 0.5f, Random.Range(-2f, 2f));
        targetT.localPosition = new Vector3(Random.Range(-4f, 4), 0.5f, Random.Range(-4f, 4f));
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetT.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float x = actions.ContinuousActions[0];
        float z = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(x, 0, z) * Time.deltaTime * 5;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Goal")
        {
            SetReward(1f);
            floorMR.material = winM;
            EndEpisode();
        }
        else if(other.gameObject.tag == "Wall")
        {
            SetReward(-1f);
            floorMR.material = loseM;
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionOut = actionsOut.ContinuousActions;
        continuousActionOut[0] = Input.GetAxis("Horizontal");
        continuousActionOut[1] = Input.GetAxis("Vertical");
    }
}
