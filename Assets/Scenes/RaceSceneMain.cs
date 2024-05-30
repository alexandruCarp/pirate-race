using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NewBehaviourScript : MonoBehaviour
{
    float screenHeight = 0;
    float screenWidth = 0;
    int lanes_number;
    public GameObject linePrefab; // Prefab for the line
    public GameObject treasurePrefab; // Prefab for the treasure
    public GameObject cannonPrefab; // Prefab for the cannon
    public GameObject bombPrefab; // Prefab for the bomb
    public Camera mainCamera; // Reference to the main camera

    public GameObject boat0Prefab; // Prefab for the boat type 0
    public GameObject boat1Prefab; // Prefab for the boat type 1
    public GameObject boat2Prefab; // Prefab for the boat type 2

    public GameObject[] boats; // Array of boats

    private int[] boat_positions;

    float boat_speed = 1f;
    int boat_slow_down_time = 2;
    private float[] boat_cooldowns;
    private int[] boat_hits;
    ArrayList boats_left = new ArrayList();

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

    private ArrayList bombs = new ArrayList();

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
        }

        // Create the boats
        boats = new GameObject[lanes_number];
        boat_positions = new int[lanes_number];
        boat_cooldowns = new float[lanes_number];
        boat_hits = new int[lanes_number];
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
            boat_cooldowns[i] = 0;
            boat_hits[i] = 0;
            boats_left.Add(i);
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
                        if (boats[controlEvent.player_id] != null)
                            boats[controlEvent.player_id].transform.position += new Vector3(0, spacing / 3, 0);
                    }
                    break;
                case SocketIOManager.ControlEventTypes.BUTTON_DOWN:
                    if (boat_positions[controlEvent.player_id] > 0)
                    {
                        boat_positions[controlEvent.player_id]--;
                        if (boats[controlEvent.player_id] != null)
                            boats[controlEvent.player_id].transform.position += new Vector3(0, -spacing / 3, 0);
                    }
                    break;
                case SocketIOManager.ControlEventTypes.BUTTON_FIRE:
                    if (boats[controlEvent.player_id] != null)
                    {
                        if (player_treasure_count[controlEvent.player_id] > 0)
                        {
                            player_treasure_count[controlEvent.player_id]--;
                            Destroy(cannons[controlEvent.player_id][player_treasure_count[controlEvent.player_id]]);
                            GameObject cannonball = Instantiate(bombPrefab);
                            cannonball.transform.position = new Vector3(boats[controlEvent.player_id].transform.position.x + 1, boats[controlEvent.player_id].transform.position.y, -1);
                            cannonball.GetComponent<Rigidbody2D>().velocity = new Vector2(10, 0);
                            bombs.Add(cannonball);
                        }
                    }
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
            treasure_timer = Random.Range(3, 6);
            int lane = (int)boats_left[(Random.Range(0, boats_left.Count))];
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
            if (boats[i] != null && boats[i].GetComponent<BoatCollision>().collected_treasure)
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
            if (boats[i] != null && boats[i].GetComponent<BoatCollision>().should_slow_down)
            {
                if (boat_cooldowns[i] <= 0)
                {
                    boat_cooldowns[i] = boat_slow_down_time;
                    boat_hits[i]++;
                    if (boat_hits[i] >= 3)
                    {
                        print("Player " + i + " lost the game");
                        SocketIOManager.emit("player_lost", i.ToString());
                        boats_left.Remove(i);
                        Destroy(boats[i]);
                        boats[i] = null;
                        if (boats_left.Count == 0)
                        {
                            print("Game over");
                            SceneManager.LoadScene("GameOver");
                        }
                    }
                }
                if (boats[i] != null)
                    boats[i].GetComponent<BoatCollision>().should_slow_down = false;
            }
            if (boats[i] != null && boat_cooldowns[i] > 0)
            {
                boats[i].transform.position += new Vector3(-boat_speed * Time.deltaTime, 0, 0);
                boat_cooldowns[i] -= Time.deltaTime;
            }
        }

        for (int i = 0; i < bombs.Count; i++)
        {
            GameObject bomb = (GameObject)bombs[i];
            if (bomb.transform.position.x > screenWidth / 2 || bomb.gameObject.GetComponent<BombCollider>().collided)
            {
                bombs.RemoveAt(i);
                Destroy(bomb);
            }
        }

        obstacleSpawnInterval -= Time.deltaTime * 0.01f;
    }

    void SpawnObstacle()
    {
        // Get the screen width
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;

        // Randomly select a lane
        int randomLane = (int)boats_left[(Random.Range(0, boats_left.Count))];
        Vector3 boatPosition = boats[randomLane].transform.position;
        float xPos = Random.Range(boatPosition.x + 5f, screenWidth / 2);

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