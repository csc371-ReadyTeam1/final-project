using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour {

    public Sprite CapturedSprite;
    public Text CaptureText;
    public float CaptureTime = 1.0f;

    /// <summary>
    /// Whether or not this checkpoint is required for the platformer to win
    /// </summary>
    public bool IsFinish = false;

    public GameObject ForcefieldToggle;
	public GameObject ShopCanvas;
    public Material circleMaterial;


    float captureAmount;
    Pawn currentCapturer = null;
    PlayerController lastCapturer = null;
    bool isCaptured = false;
    SpriteRenderer render;
    private AudioSource activateSrc;
    private Mesh circleMesh; //Mesh for the 'progress' semicircle

    // Use this for initialization
    void Start () {
        render = GetComponent<SpriteRenderer>();
        activateSrc = GetComponent<AudioSource>();
        circleMesh = new Mesh();

        if (ShopCanvas != null)
		    ShopCanvas.SetActive (false);
    }


    private List<int> indices = new List<int>();
    private List<Vector3> vertices = new List<Vector3>();
    void regenerateMesh(float percent, float size, int numPoints)
    {
        //Regenerate buffers if pointcount changes (it shouldn't much)
        if (vertices.Count != numPoints + 1)
        {
            Debug.Log("REGENERATING CHECKPOINT CIRCLE MESH: " + 
                vertices.Count + " -> " + (numPoints + 1) + " points");

            for (int i = 0; i < numPoints + 1; i++)
            {
                vertices.Add(Vector3.zero);
            }

            //Setup index buffer (doesn't change so we can do it once)
            int center = numPoints;
            for (int i = 0; i < numPoints - 1; i++)
            {
                indices.Add(center);
                indices.Add(i + 1);
                indices.Add(i);
            }

            circleMesh.Clear();
            circleMesh.SetVertices(vertices);
            circleMesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        }

        //Recreate the mesh points
        vertices[numPoints] = Vector3.zero;
        for (int i = 0; i < numPoints; i++)
        {
            float p = i * 1.0f / (numPoints - 1);
            float ang = p * Mathf.PI * 2 * percent + Mathf.PI * 0.5f;
            vertices[i] = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), 0) * size;
        }


        circleMesh.SetVertices(vertices);
    }

    void checkOwnership()
    {
        if (currentCapturer == null) { return; }

        //If capturer changes, progress should not reset but should be 'counted against'
        if (currentCapturer.Controller != lastCapturer)
        {
            captureAmount = -captureAmount;
            lastCapturer = currentCapturer.Controller;
        }
    }

    //Utility function that returns a color with a specified alpha channel
    private Color ColorAlpha(Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }
	
	// Update is called once per frame
	void Update ()
    {
        Graphics.DrawMesh(circleMesh, transform.position + new Vector3(0, 0, -0.5f), transform.rotation, circleMaterial, 10);
        if (currentCapturer == null || isCaptured) return;

        //Regenerate
        float p = Mathf.Abs(captureAmount / CaptureTime);
        regenerateMesh(p, 0.07f, 25);

        //Update capture text
        captureAmount += Time.deltaTime;
        CaptureText.text = Mathf.Round(captureAmount * 100 / CaptureTime) + "% captured";

        //Update color with who has the current 'majority'
        checkOwnership();
        PlayerController other = GameController.instance.GetOther(currentCapturer.Controller);
        CaptureText.color = (captureAmount >= 0 ? currentCapturer.Controller : other).PlayerColor;
        circleMaterial.SetColor("_Color", ColorAlpha(CaptureText.color, 0.5f));

        //If the capture amount reaches the top, mark this as captured
        if (captureAmount > CaptureTime)
        {
            Capture(currentCapturer.Controller);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Pawn pawn = collision.gameObject.GetComponent<Pawn>();
        if (pawn != null && pawn == GameController.instance.platformer)
        {
            currentCapturer = pawn;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Pawn pawn = collision.gameObject.GetComponent<Pawn>();
        if (pawn != null && pawn == currentCapturer)
        {
            currentCapturer = null;
            CaptureText.color = Color.white;
        }
    }

    public void Capture(PlayerController capturer)
    {
        render.sprite = CapturedSprite;
        CaptureText.text = "CAPTURED!!!";
        isCaptured = true;

        activateSrc.Play();

        if (IsFinish)
        {
            GameController.instance.Finish(capturer);
        }
        else
        {
            ForcefieldToggle.SetActive(true);
			OpenShop ();
        }
    }

	void OpenShop()
	{
		ShopCanvas.SetActive (false);
		Time.timeScale = 0;
	}

	public void CloseShop()
	{
		ShopCanvas.SetActive (false);
		Debug.Log ("Closed Shop.");
		Time.timeScale = 1;
	}
}
