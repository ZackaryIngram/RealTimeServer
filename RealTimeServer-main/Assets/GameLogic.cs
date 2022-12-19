using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private LinkedList<int> connectedClientIDs;
    float durationUntilNextBalloon;

    private int lastRefID;

    private LinkedList<BalloonInfo> activeBalloons;

    void Start()
    {
        connectedClientIDs = new LinkedList<int>();
        NetworkedServerProcessing.SetGameLogic(this);
        activeBalloons = new LinkedList<BalloonInfo>();
    }

    void Update()
    {
        durationUntilNextBalloon -= Time.deltaTime;

        if (durationUntilNextBalloon < 0)
        {
            durationUntilNextBalloon = 1f;
            lastRefID++;

            float screenPositionXPercent = Random.Range(0.0f, 1.0f);
            float screenPositionYPercent = Random.Range(0.0f, 1.0f);

            BalloonInfo bi = new BalloonInfo(screenPositionXPercent, screenPositionYPercent, lastRefID);

            string msg = bi.Deserialize(); //Send message that balloon spawned to client 

            foreach (var cid in connectedClientIDs) NetworkedServerProcessing.SendMessageToClient(msg, cid);

            activeBalloons.AddLast(bi);
        }
    }

    public void AddConnectedClient(int clientID)
    {
        foreach (BalloonInfo bi in activeBalloons)
        {
            string msg = bi.Deserialize();
            NetworkedServerProcessing.SendMessageToClient(msg, clientID);
        }

        connectedClientIDs.AddLast(clientID);

        connectedClientIDs.AddLast(clientID);
    }

    public void RemoveConnectedClient(int clientID)
    {
        connectedClientIDs.Remove(clientID);
    }

    public void BalloonWasClicked(int balloonID)
    {
        BalloonInfo bi = FindBalloonWithID(balloonID);

        if (bi != null)
        {
            activeBalloons.Remove(bi);
            string msg = ServerToClientSignifiers.BalloonPopped + "," + balloonID;

            foreach (int cid in connectedClientIDs)
            {
                NetworkedServerProcessing.SendMessageToClient(msg, cid);
            }
        }
    }

    private BalloonInfo FindBalloonWithID(int id)
    {
        foreach (BalloonInfo bi in activeBalloons)
        {
            return bi;
        }
        return null;
    }
}

public class BalloonInfo
{
    public float xPosPercent, yPosPercent;
    public int id;

    public BalloonInfo(float XPosPercent, float YPosPercent, int ID)
    {
        id = ID;
        xPosPercent = XPosPercent;
        yPosPercent = YPosPercent;
    }

    public string Deserialize()
    {
        return ServerToClientSignifiers.BalloonSpawned + "," + xPosPercent + "," +  yPosPercent + "," + id;
    }
}
