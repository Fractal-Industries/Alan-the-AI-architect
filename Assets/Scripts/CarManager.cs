using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CarManager : MonoBehaviour
{
    [SerializeField] GameObject CarInstance;
    List<AI> AliveSpecimens = new();

    [SerializeField] int Specimens;

    Transform Exit;

    float Elapsed, totalElapsed;
    int Min, Hour, Day, Generation;
    bool Playing;

    Text TimeDisplay;
    Text MilisDisplay;
    Text GenDisplay;

    AI.Neuron[][] BestLayerNeurons;
    float[,] BestBiases;
    AI.Neuron[] BestOutputL = new AI.Neuron[3];
    float bestScore;

    void Start()
    {
        TimeDisplay = GameObject.FindGameObjectWithTag("Time").GetComponent<Text>();
        MilisDisplay = GameObject.FindGameObjectWithTag("Milis").GetComponent<Text>();
        GenDisplay = GameObject.FindGameObjectWithTag("Gen").GetComponent<Text>();
    }

    void Update()
    {
        if (Playing)
        {
            Elapsed += Time.deltaTime;
            if (Elapsed >= 60)
            {
                Elapsed -= 60;
                Min++;
                if (Min == 60)
                {
                    Min = 0;
                    Hour++;
                    if (Hour == 24)
                    {
                        Hour = 0;
                        Day++;
                        if (Day == 99)
                            Day = 0;
                    }
                }
            }
            TimeDisplay.text = Day.ToString("00") + ":" + Hour.ToString("00") + ":" + Min.ToString("00") + ":" + (Mathf.Floor(Elapsed)).ToString("00") + ".";
            MilisDisplay.text = Mathf.Floor(Elapsed * 100 % 100).ToString("00");

            totalElapsed += Time.deltaTime;
            if (totalElapsed >= 30)
            {
                Generation++;
                GenDisplay.text = Generation.ToString();
                totalElapsed -= 30;
                for(int i = 0; i < Specimens; i++)
                {
                    if (AliveSpecimens[i] != null)
                    {
                        if(AliveSpecimens[i].Fitness > bestScore)
                        {
                            bestScore = AliveSpecimens[i].Fitness;
                            BestLayerNeurons = AliveSpecimens[i].GetMiddle();
                            BestOutputL = AliveSpecimens[i].GetOut();
                            BestBiases = AliveSpecimens[i].GetBiases();
                        }
                        Destroy(AliveSpecimens[i].gameObject);
                    }
                }
                AliveSpecimens = new();
                SaveData();
                Begin(Exit);
            }
        }
    }

    public void Begin(Transform End)
    {
        Exit = End;
        Playing = true;
        Init();
        GetData();
        for (int i = 0; i < Specimens; i++)
        {
            GameObject Temp = Instantiate(CarInstance);
            Temp.transform.SetParent(this.transform);
            Temp.transform.localPosition = new Vector3(0, 3, -1);
            Temp.transform.localRotation = Quaternion.identity;
            Temp.transform.SetParent(null);
            AliveSpecimens.Add(Temp.GetComponent<AI>());
            AliveSpecimens[i].Init();
            AliveSpecimens[i].Manager = this;
            AliveSpecimens[i].MyIndx = i;
            AliveSpecimens[i].Randomise(BestLayerNeurons, BestOutputL, BestBiases);
            AliveSpecimens[i].End = End;
            AliveSpecimens[i].gameObject.name = "AIcar";
            AliveSpecimens[i].Begin();
        }
    }
    
    void Init()
    {
        BestLayerNeurons = new AI.Neuron[2][] { new AI.Neuron[16], new AI.Neuron[16] };
        BestBiases = new float[3, 16];
        for (int i = 0; i < 16; i++)
        {
            BestLayerNeurons[0][i].weights = new float[13];
            BestLayerNeurons[1][i].weights = new float[16];
        }

        for (int i = 0; i < 3; i++)
            BestOutputL[i].weights = new float[16];
    }

    public void ResetWorld()
    {
        for (int i = 0; i < Specimens; i++)
            if (AliveSpecimens[i] != null)
                Destroy(AliveSpecimens[i].gameObject);
        AliveSpecimens = new();
    }

    public void DeathCar(int indx)
    {
        if (AliveSpecimens[indx].Fitness > bestScore)
        {
            bestScore = AliveSpecimens[indx].Fitness;
            BestLayerNeurons = AliveSpecimens[indx].GetMiddle();
            BestOutputL = AliveSpecimens[indx].GetOut();
            BestBiases = AliveSpecimens[indx].GetBiases();
            Destroy(AliveSpecimens[indx].gameObject);
        }
    }

    void SaveData()
    {
        StreamWriter writer;
        writer = new StreamWriter(Application.dataPath + "/Data.txt");
        for(int i = 0; i < 16; i++)
            for (int j = 0; j < 13; j++)
                writer.WriteLine(BestLayerNeurons[0][i].weights[j].ToString());

        for (int i = 0; i < 16; i++)
            for (int j = 0; j < 16; j++)
                writer.WriteLine(BestLayerNeurons[1][i].weights[j].ToString());

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 16; j++)
                writer.WriteLine(BestOutputL[i].weights[j].ToString());

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 16; j++)
                writer.WriteLine(BestBiases[i, j].ToString());
        writer.Close();
    }

    void GetData()
    {
        StreamReader reader;
        reader = new StreamReader(Application.dataPath + "/Data.txt");
        if (reader == null)
            return;
        for (int i = 0; i < 16; i++)
            for (int j = 0; j < 13; j++)
            {
                string Temp = reader.ReadLine();
                float.TryParse(Temp, out float T);
                BestLayerNeurons[0][i].weights[j] = T;
            }
                

        for (int i = 0; i < 16; i++)
            for (int j = 0; j < 16; j++)
            {
                string Temp = reader.ReadLine();
                float.TryParse(Temp, out float T);
                BestLayerNeurons[1][i].weights[j] = T;
            }

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 16; j++)
            {
                string Temp = reader.ReadLine();
                float.TryParse(Temp, out float T);
                BestOutputL[i].weights[j] = T;
            }

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 16; j++)
            {
                string Temp = reader.ReadLine();
                float.TryParse(Temp, out float T);
                BestBiases[i, j] = T;
            }
        reader.Close();
    }
}
