using UnityEngine;

public class CollectPowerup : MonoBehaviour
{
	private AudioSource      powerupAudio;
	private CircleCollider2D powerupCollider;
	private Renderer         powerupRenderer;

	void Start()
	{
		powerupAudio    = gameObject.GetComponent<AudioSource>();
		powerupCollider = gameObject.GetComponent<CircleCollider2D>();
		powerupRenderer = gameObject.GetComponent<Renderer>();
	}

	public void PowerupCollected()
	{
		powerupCollider.enabled = false;
		powerupRenderer.enabled = false;
		powerupAudio.Play();
		Destroy(gameObject, powerupAudio.clip.length);
	}
}
