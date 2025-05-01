using System;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Transform BallTransform;
    public Transform PathTransform;
    private Camera mainCamera;
    private Vector3 startBallPosition, startMousePosition;
    private bool moveTheBall;
    [Range(0f, 1f)] public float maxSpeed;
    [Range(0f, 1f)] public float camSpeed;
    [Range(0f, 50f)] public float pathSpeed;
    [Range(0f, 1000f)] public float ballRotateSpeed;
    private float velocity;
    private float camVelocityx, camVelocityy;
    private Rigidbody rb;
    private Collider col;
    private Renderer BallRenderer;
    public ParticleSystem ColliderParticleSystem;
    public ParticleSystem AirParticleSystem;
    public ParticleSystem BalltTrailParticleSystem;
    public Material[] BallMaterials = new Material[2];
    void Start()
    {
        BallTransform = transform;

        maxSpeed = 0.5f;
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        BallRenderer = BallTransform.GetChild(1).GetComponent<Renderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && MenuManager.MenuManagerInstance.GameState)
        {
            moveTheBall = true;
            BalltTrailParticleSystem.Play();
            Plane newPlan = new Plane(Vector3.up, 0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (newPlan.Raycast(ray, out var distance))
            {
                startMousePosition = ray.GetPoint(distance);
                startBallPosition = BallTransform.position;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            moveTheBall = false;
        }
        if (moveTheBall)
        {
            Plane newPlan = new Plane(Vector3.up, 0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (newPlan.Raycast(ray, out var distance))
            {
                Vector3 mouseNewPosition = ray.GetPoint(distance);
                Vector3 MouseNewPosition = mouseNewPosition - startMousePosition;
                Vector3 DesireBallPosition = MouseNewPosition + startBallPosition;

                DesireBallPosition.x = Mathf.Clamp(DesireBallPosition.x, -1.5f, 1.5f);

                BallTransform.position = new Vector3(Mathf.SmoothDamp(BallTransform.position.x, DesireBallPosition.x, ref velocity, maxSpeed),
                BallTransform.position.y, BallTransform.position.z);
            }
        }
        if (MenuManager.MenuManagerInstance.GameState)
        {
            BallTransform.GetChild(1).Rotate(Vector3.right * ballRotateSpeed * Time.deltaTime);
            var pathNewPos = PathTransform.position;
            PathTransform.position = new Vector3(pathNewPos.x, pathNewPos.y, Mathf.MoveTowards(pathNewPos.z, -1000f, pathSpeed * Time.deltaTime));
        }


    }
    private void LateUpdate()
    {
        if (rb.isKinematic)
        {
            var CameraNewPos = mainCamera.transform.position;
            mainCamera.transform.position = new Vector3(Mathf.SmoothDamp(CameraNewPos.x, BallTransform.position.x, ref camVelocityx, camSpeed),
            Mathf.SmoothDamp(CameraNewPos.y, BallTransform.position.y + 4f, ref camVelocityy, camSpeed), CameraNewPos.z);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            gameObject.SetActive(false);
            MenuManager.MenuManagerInstance.GameState = false;
            MenuManager.MenuManagerInstance.menuElement[2].SetActive(true);
            MenuManager.MenuManagerInstance.menuElement[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            "YOU LOSE";
        }
        switch (other.tag)
        {
            case "RedBall":
                other.gameObject.SetActive(false);
                BallMaterials[1] = other.GetComponent<Renderer>().material;
                BallRenderer.materials = BallMaterials;
                var particleRed = Instantiate(ColliderParticleSystem, transform.position, Quaternion.identity);
                particleRed.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                var ballTrailRed = BalltTrailParticleSystem.trails;
                ballTrailRed.colorOverLifetime = other.GetComponent<Renderer>().material.color;
                break;
            case "GreenBall":
                other.gameObject.SetActive(false);
                BallMaterials[1] = other.GetComponent<Renderer>().material;
                BallRenderer.materials = BallMaterials;
                var particleGreen = Instantiate(ColliderParticleSystem, transform.position, Quaternion.identity);
                particleGreen.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                var ballTrailGreen = BalltTrailParticleSystem.trails;
                ballTrailGreen.colorOverLifetime = other.GetComponent<Renderer>().material.color;
                break;
            case "BlueBall":
                other.gameObject.SetActive(false);
                BallMaterials[1] = other.GetComponent<Renderer>().material;
                BallRenderer.materials = BallMaterials;
                var particleBlue = Instantiate(ColliderParticleSystem, transform.position, Quaternion.identity);
                particleBlue.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                var ballTrailBlue = BalltTrailParticleSystem.trails;
                ballTrailBlue.colorOverLifetime = other.GetComponent<Renderer>().material.color;
                break;
            case "YellowBall":
                other.gameObject.SetActive(false);
                BallMaterials[1] = other.GetComponent<Renderer>().material;
                BallRenderer.materials = BallMaterials;
                var particleYellow = Instantiate(ColliderParticleSystem, transform.position, Quaternion.identity);
                particleYellow.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                var ballTrailYellow = BalltTrailParticleSystem.trails;
                ballTrailYellow.colorOverLifetime = other.GetComponent<Renderer>().material.color;
                break;
        }
        if (other.gameObject.name.Contains("ColorBall"))
        {
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + 10);
            MenuManager.MenuManagerInstance.menuElement[1].GetComponent<TextMeshProUGUI>().text =
                PlayerPrefs.GetInt("score").ToString();
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Path"))
        {
            rb.isKinematic = false;
            col.isTrigger = false;
            rb.linearVelocity = new Vector3(0, 8f, 0f);
            pathSpeed = pathSpeed * 2f;

            var particleAir = AirParticleSystem.main;
            particleAir.simulationSpeed = 10f;

            BalltTrailParticleSystem.Stop();
            ballRotateSpeed = 1000f;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Path"))
        {
            rb.isKinematic = true;
            col.isTrigger = true;
            pathSpeed = 30f;

            var particleAir = AirParticleSystem.main;
            particleAir.simulationSpeed = 3f;

            BalltTrailParticleSystem.Play();
            ballRotateSpeed = 500f;

        }

    }
}