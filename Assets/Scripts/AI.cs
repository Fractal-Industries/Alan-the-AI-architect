using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI : MonoBehaviour
{
    Vector3 LastPosition;
    
    [SerializeField] float BreakForce;
    [SerializeField] float motorTorque;
    [SerializeField] float MaxSteeringAngle;
    [SerializeField] float MaxSpeed;
    [SerializeField] float BreakingSpeed;

    [SerializeField] WheelCollider FR;
    [SerializeField] WheelCollider FL;
    [SerializeField] WheelCollider BR;
    [SerializeField] WheelCollider BL;
    [SerializeField] float[] Rays;

    [SerializeField] Transform FRMesh;
    [SerializeField] Transform FLMesh;
    [SerializeField] Transform BRMesh;
    [SerializeField] Transform BLMesh;

    [SerializeField] LayerMask RaycastMask;

    GameObject[] Buttons;
    public Transform End;

    float Acceleration;
    float Steering;
    public float Fitness;

    bool Breaking;
    bool Running;

    public int MyIndx;
    int ButtonHash = 1;
    int ButtonCount;

    public CarManager Manager;

    public struct Neuron
    {
        public float Activation;
        public float[] weights;
    }

    Neuron[] InputL;
    Neuron[][] LayerNeurons;
    float[,] Biases;
    Neuron[] OutputL = new Neuron[3];

    void Start()
    {
        Buttons = GameObject.FindGameObjectsWithTag("Button");
    }

    void FixedUpdate()
    {
        float Speed = Vector3.Distance(transform.position, LastPosition);
        LastPosition = transform.position;

        float[] Dat = new float[Rays.Length + 4];
        for(int i = 0; i < Rays.Length; i++)
        {
            if (Physics.Raycast(transform.position, -transform.forward + transform.right * Rays[i], out RaycastHit point, 50, RaycastMask))
            {
                Dat[i] = Vector3.Distance(transform.position, point.point) / 50;
                Debug.DrawLine(transform.position, point.point, Color.blue);
            }
            else
            {
                Dat[i] = 1;
                Debug.DrawLine(transform.position, transform.position + Vector3.Normalize(-transform.forward + transform.right * Rays[i]) * 50, Color.green);
            }
        }
        float Closest = 200;
        float Angle = 1;
        if (ButtonCount < Buttons.Length)
            for (int i = 0; i < Buttons.Length; i++)
            {
                if (Vector3.Distance(transform.position, Buttons[i].transform.position) < Closest && ButtonHash % Buttons[i].GetComponent<ButtonConn>().PrimeIndx != 0)
                {
                    Closest = Vector3.Distance(transform.position, Buttons[i].transform.position);
                    Transform Temp = transform.GetChild(0);
                    Temp.LookAt(Buttons[i].transform);
                    Angle = Vector3.Dot(-transform.forward, Temp.forward);
                }
                    
            }
        else
        {
            if (Vector3.Distance(transform.position, End.position) < Closest)
            {
                Closest = Vector3.Distance(transform.position, End.position);
                Transform Temp = transform.GetChild(0);
                Temp.LookAt(End);
                Angle = Vector3.Dot(-transform.forward, Temp.forward);
            }
        }
        Fitness = ButtonCount * 200 + (200 - Closest);
        Dat[Rays.Length] = Closest / 200;
        Dat[Rays.Length + 1] = Mathf.Clamp(Speed, 0, 10) / 10;
        Dat[Rays.Length + 2] = (Steering + 1) / 2;
        Dat[Rays.Length + 3] = (Angle + 1) / 2;

        if (Running)
        {
            RunNetwork(Dat);
            Steering = (OutputL[0].Activation * 2) - 1;
            Acceleration = (OutputL[1].Activation * 2) - 1;
            Breaking = OutputL[2].Activation > 0.5f;
        }
        else
        {
            Steering = 0;
            Acceleration = 0;
            Breaking = true;
        }

        float AppliedForce = 0;
        if (Speed < MaxSpeed && Acceleration > 0 || Speed > -MaxSpeed && Acceleration < 0)
            AppliedForce = -Acceleration * motorTorque;

        if (Speed > BreakingSpeed || Speed < -BreakingSpeed)
            Breaking = true;

        BR.motorTorque = AppliedForce;
        BL.motorTorque = AppliedForce;
        FR.motorTorque = AppliedForce;
        FL.motorTorque = AppliedForce;

        FR.brakeTorque = Breaking ? BreakForce : 0;
        FL.brakeTorque = Breaking ? BreakForce : 0;
        BR.brakeTorque = Breaking ? BreakForce : 0;
        BL.brakeTorque = Breaking ? BreakForce : 0;

        FR.steerAngle = Steering * MaxSteeringAngle;
        FL.steerAngle = Steering * MaxSteeringAngle;

        UpdateWheel(FR, FRMesh);
        UpdateWheel(FL, FLMesh);
        UpdateWheel(BR, BRMesh);
        UpdateWheel(BL, BLMesh);
    }

    void UpdateWheel(WheelCollider col, Transform trans)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        trans.SetPositionAndRotation(pos, rot);
    }

    void CalcualteLayer(Neuron[] Input, Neuron[] Output, int Lin, int Lout, int Lindx)
    {
        for(int i = 0; i < Lout; i++)
        {
            float sum = 0;
            for(int j = 0; j < Lin; j++)
                sum += Input[j].Activation * Output[i].weights[j];
            sum *= Biases[Lindx, i];
            Output[i].Activation = Sigmoid(sum);
        }
    }

    float Sigmoid(float X)
    {
        return 1 / (1 + Mathf.Pow(2.718281828459045f, X));
    }

    void RunNetwork(float[] Data)
    {
        for (int i = 0; i < Data.Length; i++)
            InputL[i].Activation = Data[i];
        CalcualteLayer(InputL, LayerNeurons[0], Data.Length, 16, 0);
        CalcualteLayer(LayerNeurons[0], LayerNeurons[1], 16, 16, 1);
        CalcualteLayer(LayerNeurons[1], OutputL, 16, 3, 2);
    }

    public void Init()
    {
        InputL = new Neuron[Rays.Length + 4];
        LayerNeurons = new Neuron[2][] { new Neuron[16], new Neuron[16] };
        Biases = new float[3, 16];
        for (int i = 0; i < 16; i++)
        {
            LayerNeurons[0][i].weights = new float[Rays.Length + 4];
            LayerNeurons[1][i].weights = new float[16];
        }

        for (int i = 0; i < 3; i++)
            OutputL[i].weights = new float[16];
    }

    public void Begin()
    {
        Running = true;
    }

    public void Randomise(Neuron[][] In, Neuron[] Out, float[,]Bis)
    {
        for (int i = 0; i < 16; i++)
            for (int j = 0; j < Rays.Length + 4; j++)
                LayerNeurons[0][i].weights[j] = In[0][i].weights[j] + UnityEngine.Random.Range(-10f, 10f);
        for (int i = 0; i < 16; i++)
            for (int j = 0; j < 16; j++)
                LayerNeurons[1][i].weights[j] = In[1][i].weights[j] + UnityEngine.Random.Range(-10f, 10f);
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 16; j++)
                OutputL[i].weights[j] = Out[i].weights[j] + UnityEngine.Random.Range(-10f, 10f);
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 16; j++)
                Biases[i, j] = Bis[i, j] + UnityEngine.Random.Range(-10f, 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.name);
    }

    public Neuron[] GetOut()
    {
        Neuron[] Temp = new Neuron[3];
        for (int i = 0; i < 3; i++)
        {
            Temp[i].weights = new float[16];
            for (int j = 0; j < 16; j++)
                Temp[i].weights[j] = OutputL[i].weights[j];
        }
        return Temp;
    }

    public Neuron[][] GetMiddle()
    {
        Neuron[][] Temp = new Neuron[2][] { new Neuron[16], new Neuron[16] };
        for (int i = 0; i < 16; i++)
        {
            Temp[0][i].weights = new float[Rays.Length + 4];
            Temp[1][i].weights = new float[16];
            for(int j = 0; j < Rays.Length + 4; j++)
                Temp[0][i].weights[j] = LayerNeurons[0][i].weights[j];
            for (int j = 0; j < 16; j++)
                Temp[1][i].weights[j] = LayerNeurons[1][i].weights[j];
        }
        return Temp;
    }

    public float[,] GetBiases()
    {
        float[,] Temp = new float[3, 16];
        for(int i = 0; i < 3; i++)
            for(int j = 0; j < 16; j++)
                Temp[i, j] = Biases[i, j];
        return Temp;
    }

    public void PressButton(int primeIndex)
    {
        if (ButtonHash % primeIndex == 0)
            ButtonHash *= primeIndex;
        ButtonCount++;
    }
}
