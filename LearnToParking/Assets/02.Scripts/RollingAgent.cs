using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollingAgent : Agent
{
    Rigidbody rbody;

    public Transform Target;


    void Start()
    {
        rbody = GetComponent<Rigidbody>();    
    }


    public override void OnEpisodeBegin() // 에피소드 시작시 에이전트가 할 행동
    {
        if(transform.localPosition.y < 0)
        {
            rbody.angularVelocity = Vector3.zero;
            rbody.velocity = Vector3.zero;

            transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }
    public override void CollectObservations(VectorSensor sensor) // 에이전트가 주변 관측시 자동실행
    {
        sensor.AddObservation(Target.localPosition); // x, y ,z  3개
        sensor.AddObservation(transform.localPosition);// x, y, z 3개

        sensor.AddObservation(rbody.velocity.x); // 1개
        sensor.AddObservation(rbody.velocity.x); // 1개
        // Vector Obserbation.SpaceSize = 총 8개
    }

    public override void OnActionReceived(ActionBuffers actions) // 에이전트의 정책결정 (보상 결정
    {
        Vector3 coontrolSignal = Vector3.zero;

        coontrolSignal.x = actions.ContinuousActions[0];
        coontrolSignal.z = actions.ContinuousActions[1];

        rbody.AddForce(coontrolSignal * 10);

        float distanceTarget = Vector3.Distance(transform.localPosition, Target.localPosition);

        if(distanceTarget < 1.4f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else if(transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxis("Horizontal");
        continuousAction[1] = Input.GetAxis("Vertical");

    }

}
