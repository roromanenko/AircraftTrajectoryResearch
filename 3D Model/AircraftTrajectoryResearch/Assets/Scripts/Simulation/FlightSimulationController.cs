using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlightSimulationController : MonoBehaviour
{
	[System.Serializable]
	public class PathData
	{
		public List<Vector2> points = new();
	}

	[System.Serializable]
	public class PathList
	{
		public List<PathData> paths = new();
	}

	[Header("Simulation Settings")]
	public float simulationSpeed = 100f;
	public float height = 1000f;
	public int controlLaw = 1;
	public double _Z0 = 0;
	public double _X0 = -5000;
	public double dt = 0.01;
	public double _w = 0;
	public double _NV = 0;
	public bool autoStart = true;

	[Header("References")]
	public GameObject aircraft;
	public Camera mainCamera;
	public Button returnToMenuButton;

	[Header("Materials")]
	public Material previousPathMaterial;
	public Material currentPathMaterial;

	[Header("Path Settings")]
	public float currentPathWidth = 2f;
	public float previousPathWidth = 2f;
	public float viewScaleFactor = 500f;

	public SimulationResult result;
	private int currentIndex = 0;
	private float time = 0f;
	private bool simulationRunning = false;
	private bool simulationFinished = false;
	private bool viewMode = false;

	private LineRenderer currentPath;
	private List<Vector3> points = new();

	// =============================================================

	void Awake()
	{
		// автонастройка камеры и кнопки
		if (mainCamera == null) mainCamera = Camera.main;
		if (returnToMenuButton == null)
		{
			var go = GameObject.Find("Button_ReturnToMenu");
			if (go != null) returnToMenuButton = go.GetComponent<Button>();
		}

		if (returnToMenuButton != null)
			returnToMenuButton.gameObject.SetActive(false);
	}

	void Start()
	{
		// загрузка параметров
		if (PlayerPrefs.HasKey("ControlLaw")) controlLaw = PlayerPrefs.GetInt("ControlLaw");
		if (PlayerPrefs.HasKey("X0")) _X0 = PlayerPrefs.GetFloat("X0");
		if (PlayerPrefs.HasKey("Z0")) _Z0 = PlayerPrefs.GetFloat("Z0");
		if (PlayerPrefs.HasKey("dt")) dt = PlayerPrefs.GetFloat("dt");
		if (PlayerPrefs.HasKey("w")) _w = PlayerPrefs.GetFloat("w");
		if (PlayerPrefs.HasKey("NV")) _NV = PlayerPrefs.GetFloat("NV");

		viewMode = PlayerPrefs.GetInt("ViewMode", 0) == 1;

		LoadPreviousPaths();

		if (viewMode)
		{
			simulationRunning = false;
			simulationFinished = true;
			ShowAllPathsTopView();
			ShowReturnButton();
		}
		else if (autoStart)
		{
			RunSimulation();
		}
	}

	// =============================================================

	public void RunSimulation()
	{
		if (currentPath != null)
			Destroy(currentPath.gameObject);
		points.Clear();

		var parameters = new Params();
		var coeffs = new CoefficientsCalculator();
		var control = new CalculateControlLaw();
		var sim = new Simulation(coeffs, parameters, control);

		result = sim.Run(dt, _w, _NV, controlLaw, _X0, _Z0);

		currentIndex = 0;
		time = 0f;
		simulationRunning = true;
		simulationFinished = false;

		currentPath = new GameObject("CurrentPath").AddComponent<LineRenderer>();
		currentPath.material = currentPathMaterial;
		currentPath.widthMultiplier = currentPathWidth;
		currentPath.positionCount = 0;
		currentPath.useWorldSpace = true;
		currentPath.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
	}

	void Update()
	{
		if (!simulationRunning || result == null || result.Time.Count == 0)
			return;

		time += Time.deltaTime * simulationSpeed;

		while (currentIndex < result.Time.Count - 1 && result.Time[currentIndex] < time)
			currentIndex++;

		float x = (float)result.X[currentIndex];
		float z = (float)result.Z[currentIndex];
		float gamma = (float)result.Gamma[currentIndex];
		float psi = (float)result.Psi_g[currentIndex];

		Vector3 position = new Vector3(x, height, z);
		aircraft.transform.position = position;
		aircraft.transform.rotation = Quaternion.Euler(gamma, -psi, 0);

		points.Add(position);
		currentPath.positionCount = points.Count;
		currentPath.SetPositions(points.ToArray());

		if (currentIndex >= result.Time.Count - 1 && !simulationFinished)
		{
			simulationFinished = true;
			simulationRunning = false;
			OnSimulationEnd();
		}
	}

	// =============================================================

	void OnSimulationEnd()
	{
		Debug.Log("Симуляция завершена, камера отдаляется...");

		// финальная фиксация линии
		if (currentPath != null && points.Count > 1)
		{
			currentPath.positionCount = points.Count;
			currentPath.SetPositions(points.ToArray());
			currentPath.widthMultiplier *= viewScaleFactor;
			currentPath.material = currentPathMaterial;
		}

		// делаем старые пути толще — без тегов
		foreach (var lr in FindObjectsOfType<LineRenderer>())
		{
			if (lr != currentPath)
				lr.widthMultiplier *= viewScaleFactor;
		}

		SavePath(points);
		StartCoroutine(SmoothTopView());
	}

	public int CurrentIndexSafe()
	{
		if (result == null || result.Time == null || result.Time.Count == 0)
			return 0;

		return Mathf.Clamp(currentIndex, 0, result.Time.Count - 1);
	}

	// =============================================================

	System.Collections.IEnumerator SmoothTopView()
	{
		if (mainCamera == null) yield break;
		if (points.Count == 0) yield break;

		Vector3 min = points[0], max = points[0];
		foreach (var p in points)
		{
			min = Vector3.Min(min, p);
			max = Vector3.Max(max, p);
		}

		Vector3 center = (min + max) / 2f;
		float distance = Vector3.Distance(min, max);
		float targetHeight = Mathf.Max(1000f, distance * 0.6f);

		Vector3 startPos = mainCamera.transform.position;
		Quaternion startRot = mainCamera.transform.rotation;
		Vector3 targetPos = new Vector3(center.x, targetHeight, center.z);
		Quaternion targetRot = Quaternion.Euler(90, 0, 0);

		float t = 0;
		while (t < 1f)
		{
			t += Time.deltaTime / 2f;
			mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
			mainCamera.transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
			yield return null;
		}

		//Автоматическое масштабирование толщины линий
		float scaleFactor = Mathf.Clamp(targetHeight / 500f, 1f, 50f);
		foreach (var lr in FindObjectsOfType<LineRenderer>())
		{
			lr.widthMultiplier = previousPathWidth * scaleFactor;
		}

		ShowReturnButton();
	}

	// =============================================================

	void SavePath(List<Vector3> path)
	{
		PathList allPaths = LoadAllPaths();
		PathData newPath = new();

		foreach (var p in path)
			newPath.points.Add(new Vector2(p.x, p.z));

		allPaths.paths.Add(newPath);

		string json = JsonUtility.ToJson(allPaths);
		PlayerPrefs.SetString("SavedPaths", json);
		PlayerPrefs.Save();
	}

	PathList LoadAllPaths()
	{
		string json = PlayerPrefs.GetString("SavedPaths", "");
		if (string.IsNullOrEmpty(json))
			return new PathList();
		return JsonUtility.FromJson<PathList>(json);
	}

	void LoadPreviousPaths()
	{
		PathList allPaths = LoadAllPaths();
		foreach (var path in allPaths.paths)
		{
			if (path.points.Count < 2) continue;

			var go = new GameObject("PreviousPath");
			var lr = go.AddComponent<LineRenderer>();
			lr.material = previousPathMaterial;
			lr.widthMultiplier = previousPathWidth * (viewMode ? viewScaleFactor : 1f);
			lr.positionCount = path.points.Count;
			lr.useWorldSpace = true;
			lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			Vector3[] positions = new Vector3[path.points.Count];
			for (int i = 0; i < path.points.Count; i++)
				positions[i] = new Vector3(path.points[i].x, height, path.points[i].y);

			lr.SetPositions(positions);
		}
	}

	void ShowAllPathsTopView()
	{
		PathList allPaths = LoadAllPaths();
		if (allPaths.paths.Count == 0)
		{
			Debug.LogWarning("Нет сохранённых путей для отображения");
			return;
		}

		// Собираем все точки из всех путей
		List<Vector3> allPoints = new();
		foreach (var path in allPaths.paths)
		{
			foreach (var p in path.points)
				allPoints.Add(new Vector3(p.x, height, p.y));
		}

		if (allPoints.Count == 0) return;

		// Находим границы и центр
		Vector3 min = allPoints[0], max = allPoints[0];
		foreach (var p in allPoints)
		{
			min = Vector3.Min(min, p);
			max = Vector3.Max(max, p);
		}

		Vector3 center = (min + max) / 2f;
		float distance = Vector3.Distance(min, max);
		float targetHeight = Mathf.Max(1000f, distance * 0.6f);

		if (mainCamera == null)
			mainCamera = Camera.main;

		// Устанавливаем камеру
		mainCamera.transform.position = new Vector3(center.x, targetHeight, center.z);
		mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);

		// Подстраиваем толщину линий под высоту
		float scaleFactor = Mathf.Clamp(targetHeight / 500f, 1f, 50f);
		foreach (var lr in FindObjectsOfType<LineRenderer>())
		{
			lr.widthMultiplier = previousPathWidth * scaleFactor;
		}

		Debug.Log($"📸 Камера сверху, масштаб путей x{scaleFactor:F1}, высота: {targetHeight:F1}");
	}



	// =============================================================

	void ShowReturnButton()
	{
		if (returnToMenuButton == null)
		{
			var go = GameObject.Find("Button_ReturnToMenu");
			if (go != null) returnToMenuButton = go.GetComponent<Button>();
		}

		if (returnToMenuButton != null)
		{
			returnToMenuButton.gameObject.SetActive(true);
			returnToMenuButton.onClick.RemoveAllListeners();
			returnToMenuButton.onClick.AddListener(ReturnToMenu);
		}
		else
		{
			Debug.LogWarning("Button 'Button_ReturnToMenu' not found in scene!");
		}
	}

	void ReturnToMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
