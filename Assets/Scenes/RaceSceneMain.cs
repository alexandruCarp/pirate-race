using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    int lanes_number;
    public GameObject linePrefab; // Prefab for the line
    public Camera mainCamera; // Reference to the main camera

    public GameObject boatPrefab; // Prefab for the boat

    public static GameObject[] boats; // Array of boats

    float spacing; // Spacing between lanes

    public GameObject obstaclePrefab;
    float obstacleSpawnInterval = 2.0f; 
    float nextObstacleSpawnTime;
    public static List<GameObject>  obstacles = new List<GameObject>();
    public float obstacleSpeed = 5f;
    float obstacleHeight;


    // Start is called before the first frame update
    void Start()
    {
        lanes_number = SocketIOManager.getConnectedPlayers();
        // Get the height of the screen in world units
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;

        // Calculate the spacing between lanes
        spacing = screenHeight / (lanes_number);
        for (int i = 0; i < lanes_number - 1; i++)
        {
            GameObject line = Instantiate(linePrefab);

            // Set the line's parent to this GameObject for better organization
            line.transform.SetParent(transform);

            // Calculate the y position for this line
            float yPos = (screenHeight / 2) - (spacing * (i + 1));

            // Position the line
            line.transform.position = new Vector3(0, yPos, -1);

            // Adjust the line's scale to match the screen width
            line.transform.localScale = new Vector3(screenWidth, line.transform.localScale.y, line.transform.localScale.z);
        }

        // Create the boats
        boats = new GameObject[lanes_number];
        for (int i = 0; i < lanes_number; i++)
        {
            // Calculate the y position for this boat
            float yPos = (screenHeight / 2) + spacing / 2 - (spacing * (i + 1));

            // Create the boat
            boats[i] = Instantiate(boatPrefab);

            // Set the boat's parent to this GameObject for better organization
            boats[i].transform.SetParent(transform);

            // Position the boat
            boats[i].transform.position = new Vector3(-screenWidth * 1 / 4, yPos, -1);
        }

        nextObstacleSpawnTime = Time.time + obstacleSpawnInterval;
        obstacleHeight = obstaclePrefab.GetComponent<Renderer>().bounds.size.y;

    }

    // Update is called once per frame
    void Update()
    {
        if (SocketIOManager.event_available())
        {
            SocketIOManager.ControlEvent controlEvent = SocketIOManager.get_event();
            Debug.Log("Event received: " + controlEvent.player_id + " " + controlEvent.type);
            switch (controlEvent.type)
            {
                case SocketIOManager.ControlEventTypes.BUTTON_UP:
                    boats[controlEvent.player_id].transform.position += new Vector3(0, obstacleHeight, 0);
                    break;
                case SocketIOManager.ControlEventTypes.BUTTON_DOWN:
                    boats[controlEvent.player_id].transform.position += new Vector3(0, -obstacleHeight, 0);
                    break;
                case SocketIOManager.ControlEventTypes.BUTTON_FIRE:
                    // Fire logic
                    break;
            }
        }


        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].transform.position += new Vector3(-obstacleSpeed * Time.deltaTime, 0, 0);
        }

        if (Time.time >= nextObstacleSpawnTime)
        {
            SpawnObstacle();
            nextObstacleSpawnTime = Time.time + obstacleSpawnInterval;
        }
    }

    void SpawnObstacle()
    {
        // Get the screen width
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;

        // Randomly select a lane
        int randomLane = Random.Range(0, lanes_number);
        Vector3 boatPosition = boats[randomLane].transform.position;
        float xOffset = obstaclePrefab.transform.localScale.x; // Adjust this value as needed
        float xPos = Random.Range(boatPosition.x + 5f, screenWidth - 10f);

        // Calculate the y position for the obstacle
        float yPos = boatPosition.y;

        // Create the obstacle
        GameObject obstacle = Instantiate(obstaclePrefab);

        // Set the obstacle's parent to this GameObject for better organization
        obstacle.transform.SetParent(transform);

        // Position the obstacle
        obstacle.transform.position = new Vector3(xPos, yPos, -1);

        obstacles.Add(obstacle);
    }

}