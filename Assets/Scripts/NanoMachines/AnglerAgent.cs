using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.SceneManagement;

public class AnglerAgent : Agent
{
    public AnguraController controller;
    public Transform[] targets;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        foreach (Transform t in targets)
        {
            sensor.AddObservation(t.localPosition);
        }
        //base.CollectObservations(sensor);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];


        controller.hori = moveX;
        controller.verti = moveY;

        
        if (actions.DiscreteActions[0] == 1)
        {
            controller.GroundAttack();
        }
        
        //base.OnActionReceived(actions);
    }

    private void Update()
    {
        if (controller.Health == 0)
        {
            SetReward(-100f);
            EndEpisode();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        if (targets.Length == 0)
        {
            AddReward(100);
            EndEpisode();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
