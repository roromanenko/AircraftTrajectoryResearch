using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
	[Header("Cameras")]
	public Camera mainCamera;
	public Camera bottomCamera;

	[Header("Shake Settings (only bottom camera)")]
	public float rotationAmplitude = 0.2f; // максимальный угол тряски (в градусах)
	public float shakeFrequency = 5f;      // скорость вибрации
	public float noiseAmplitude = 0.2f;    // дополнительный случайный шум

	private Camera activeCamera;
	private Quaternion baseRotation;
	private float shakeTimer;

	void Start()
	{
		SetActiveCamera(mainCamera);
	}

	void Update()
	{
		// переключаем камеры по клавише C
		if (Input.GetKeyDown(KeyCode.C))
		{
			if (activeCamera == mainCamera)
				SetActiveCamera(bottomCamera);
			else
				SetActiveCamera(mainCamera);
		}

		// применяем тряску только для нижней камеры
		if (activeCamera == bottomCamera)
			ApplyRotationalShake();
	}

	void SetActiveCamera(Camera cam)
	{
		mainCamera.enabled = false;
		bottomCamera.enabled = false;

		activeCamera = cam;
		activeCamera.enabled = true;

		baseRotation = activeCamera.transform.localRotation;
	}

	void ApplyRotationalShake()
	{
		shakeTimer += Time.deltaTime * shakeFrequency * 5f; // ускоряем частоту в 10 раз

		// мелкие быстрые колебания — больше случайного шума, меньше синуса
		float pitch = Mathf.PerlinNoise(shakeTimer, 0f) * 2f - 1f;
		float yaw = Mathf.PerlinNoise(0f, shakeTimer * 1.2f) * 2f - 1f;
		float roll = Mathf.PerlinNoise(shakeTimer * 0.8f, shakeTimer * 1.5f) * 2f - 1f;

		pitch *= rotationAmplitude;
		yaw *= rotationAmplitude * 0.8f;
		roll *= rotationAmplitude * 0.5f;

		// мелкий случайный дребезг поверх
		pitch += (Random.value - 0.5f) * noiseAmplitude;
		yaw += (Random.value - 0.5f) * noiseAmplitude;
		roll += (Random.value - 0.5f) * noiseAmplitude * 0.5f;

		Quaternion shakeRotation = Quaternion.Euler(pitch, yaw, roll);
		activeCamera.transform.localRotation = baseRotation * shakeRotation;
	}

}
