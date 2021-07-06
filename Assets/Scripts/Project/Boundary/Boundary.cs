using UnityEngine;

// This is a handy little script I adapted from one I found ages ago on Stack Overflow. 
// By assigning it to four "Boundary" GameObjects with BoxCollider2D components attached, you can create trigger areas to deal with objects that try to, or move off screen.
// Just assign each of your four barriers a Left, Top, Right or Bottom direction from the Inspector. 
public class Boundary : MonoBehaviour
{
	public enum BoundaryLocation
	{
		LEFT, RIGHT, TOP, BOTTOM
	};

	[Tooltip("The location of this boundary (LEFT/RIGHT/TOP/BOTTOM).")]
	public BoundaryLocation location = BoundaryLocation.LEFT;

	[Tooltip("The width of this boundary.")]
	[SerializeField] float boundaryWidth = 0.8f;

	[Tooltip("The overhang: We add this to the length of this boundaries to ensure there are no gaps at the corners of the screen." +
			 "This to prevent any physics objects from escaping through the cracks.")]
	[SerializeField] float overhang = 1.0f; 

	void Start()
	{
		// Get the the world coordinates of the corners of the camera viewport.
		Vector3 topLeft    = Camera.main.ScreenToWorldPoint(new Vector3(0                     , Camera.main.pixelHeight, 0));
		Vector3 topRight   = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0));
		Vector3 lowerLeft  = Camera.main.ScreenToWorldPoint(new Vector3(0                     , 0                      , 0));
		Vector3 lowerRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0                      , 0));

		// Get this game objects BoxCollider2D
		BoxCollider2D barrier = GetComponent<BoxCollider2D>();

		// Depending on the assigned 'direction' of the Boundary we adjust the size and position based on the camera viewport as obtained above
		switch (location)
		{
			case BoundaryLocation.LEFT:
				{
					barrier.size = new Vector2(Mathf.Abs(boundaryWidth), Mathf.Abs(lowerLeft.y) + Mathf.Abs(lowerRight.y) + overhang);
					barrier.offset = new Vector2(-boundaryWidth / 2, 0);
					transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight / 2, 1));
				}
				break;

			case BoundaryLocation.RIGHT:
				{
					barrier.size = new Vector2(Mathf.Abs(boundaryWidth), Mathf.Abs(lowerLeft.y) + Mathf.Abs(lowerRight.y) + overhang);
					barrier.offset = new Vector2(boundaryWidth / 2, 0);
					transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight / 2, 1));
				}
				break;

			case BoundaryLocation.TOP:
				{
					barrier.size       = new Vector2(Mathf.Abs(topLeft.x) + Mathf.Abs(topRight.x) + overhang, Mathf.Abs(boundaryWidth));
					barrier.offset     = new Vector2(0, boundaryWidth / 2);
					transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight, 1));
				}
				break;

			case BoundaryLocation.BOTTOM:
				{
					barrier.size       = new Vector2(Mathf.Abs(topLeft.x) + Mathf.Abs(topRight.x) + overhang, Mathf.Abs(boundaryWidth));
					barrier.offset     = new Vector2(0, -boundaryWidth / 2);
					transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, 0, 1));
				}
				break;
		}

		// refresh boundary manager so it works with the latest data
		BoundaryManager.Instance.Refresh();
	}
}
