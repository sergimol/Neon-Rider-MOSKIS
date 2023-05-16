using UnityEngine;
public class NextScene : MonoBehaviour
{
	private void OnTriggerEnter2D()
	{
		GameManager.instance.ChangeScene(); // Llama al método del GameManager que avanza a la siguiente escena
	}
}
