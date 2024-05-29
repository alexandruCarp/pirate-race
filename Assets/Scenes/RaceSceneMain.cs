using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    float screenHeight = 0;
    float screenWidth = 0;
    int lanes_number;
    public GameObject linePrefab; // Prefab for the line
    public GameObject treasurePrefab; // Prefab for the treasure
    public GameObject cannonPrefab; // Prefab for the cannon
    public Camera mainCamera; // Reference to the main camera

    public GameObject boat0Prefab; // Prefab for the boat type 0
    public GameObject boat1Prefab; // Prefab for the boat type 1
    public GameObject boat2Prefab; // Prefab for the boat type 2

    public GameObject[] boats; // Array of boats

    private int[] boat_positions;

    float spacing; // Spacing between lanes
    float subspacing; // Spacing between positions in the same lane

    float treasure_timer = -1;

    private ArrayList treasures = new ArrayList();
    private int[] player_treasure_count = new int[5];

    private int cannon_x;
    int cannon_spacing;
    private GameObject[][] cannons;

    public GameObject obstaclePrefab;
    float obstacleSpawnInterval = 2.0f; 
    float nextObstacleSpawnTime;
    public static List<GameObject>  obstacles = new List<GameObject>();
    public float obstacleSpeed = 5f;
    float obstacleHeight;


    // Start is called before the first frame update
    void Start()
    {
        cannons = new GameObject[5][];
        for (int i = 0; i < 5; i++)
        {
            cannons[i] = new GameObject[3];
            player_treasure_count[i] = 0;
        }
        lanes_number = SocketIOManager.getConnectedPlayers();
        // Get the height of the screen in world units
        screenHeight = 2f * mainCamera.orthographicSize;
        screenWidth = screenHeight * mainCamera.aspect;

        cannon_spacing = (int)(screenWidth / 15);
        cannon_x = (int)(-screenWidth / 2 + 0.5 * cannon_spacing);

        GameObject[] boatPrefabs = new GameObject[] { boat0Prefab, boat1Prefab, boat2Prefab };

        // Calculate the spacing between lanes
        spacing = screenHeight / (lanes_number);
        
        subspacing = spacing / 3;

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
        boat_positions = new int[lanes_number];
        for (int i = 0; i < lanes_number; i++)
        {
            // Calculate the y position for this boat
            float yPos = (screenHeight / 2) + spacing / 2 - (spacing * (i + 1));

            // Create the boat
            boats[i] = Instantiate(boatPrefabs[SocketIOManager.getPlayerBoatId(i)]);

            // Set the boat's parent to this GameObject for better organization
            boats[i].transform.SetParent(transform);

            // Position the boat
            boats[i].transform.position = new Vector3(-screenWidth * 1 / 4, yPos, -1);

            boat_positions[i] = 1; // mid position
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
                    if (boat_positions[controlEvent.player_id] < 2)
                    {
                        boat_positions[controlEvent.player_id]++;
                        boats[controlEvent.player_id].transform.position += new Vector3(0, spacing / 3, 0);
                    }
                    break;
                case SocketIOManager.ControlEventTypes.BUTTON_DOWN:
                    if (boat_positions[controlEvent.player_id] > 0)
                    {
                        boat_positions[controlEvent.player_id]--;
                        boats[controlEvent.player_id].transform.position += new Vector3(0, -spacing / 3, 0);
                    }
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
        if (treasure_timer < 0) {
            treasure_timer = Random.Range(1, 2);
            int lane = Random.Range(0, lanes_number);
            int position = Random.Range(0, 3);
            GameObject treasure = Instantiate(treasurePrefab);
            float yPos = (screenHeight / 2) + spacing / 2 - (spacing * (lane + 1));
            yPos = yPos + (position - 1) * subspacing;
            int xPos = (int)screenWidth / 2;
            treasure.transform.position = new Vector3(xPos, yPos, -1);
            treasures.Add(treasure);
        } else {
            treasure_timer -= Time.deltaTime;
        }
        for (int i = 0; i < treasures.Count; i++)
        {
            GameObject treasure = (GameObject)treasures[i];
            if (treasure.GetComponent<TreasureCollide>().collided)
            {
                treasures.RemoveAt(i);
                Destroy(treasure);
            }
            treasure.transform.position += new Vector3(-2f * Time.deltaTime, 0, 0);
            if (treasure.transform.position.x < -screenWidth / 2)
            {
                treasures.RemoveAt(i);
                Destroy(treasure);
            }
        }
        for (int i = 0; i < lanes_number; i++)
        {
            if (boats[i].GetComponent<BoatCollision>().collected_treasure)
            {
                print("Player " + i + " collected treasure");
                if (player_treasure_count[i] < 3)
                {
                    player_treasure_count[i]++;
                    GameObject cannon = Instantiate(cannonPrefab);
                    cannon.transform.position = new Vector3(cannon_x + cannon_spacing * (player_treasure_count[i] - 1), (screenHeight / 2) + spacing / 2 + subspacing * 1.25f - (spacing * (i + 1)), -1);
                    cannons[i][player_treasure_count[i] - 1] = cannon;
                }
                boats[i].GetComponent<BoatCollision>().collected_treasure = false;
            }
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