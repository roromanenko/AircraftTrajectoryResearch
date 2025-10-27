using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
	private TMP_InputField inputX0;
	private TMP_InputField inputZ0;
	private TMP_InputField inputDt;
	private TMP_InputField inputW;
	private TMP_InputField inputNV;
	private TMP_Dropdown controlLawDropdown;

	private Button startButton;
	private Button viewPathsButton;
	private Button clearPathsButton;
	private Button quitButton;

	void Awake()
	{
		// Ищем все элементы по именам (из твоего Hierarchy)
		inputX0 = GameObject.Find("Input_X0")?.GetComponent<TMP_InputField>();
		inputZ0 = GameObject.Find("Input_Z0")?.GetComponent<TMP_InputField>();
		inputDt = GameObject.Find("Input_dt")?.GetComponent<TMP_InputField>();
		inputW = GameObject.Find("Input_W")?.GetComponent<TMP_InputField>();
		inputNV = GameObject.Find("Input_NV")?.GetComponent<TMP_InputField>();
		controlLawDropdown = GameObject.Find("Dropdown_ControlLaw")?.GetComponent<TMP_Dropdown>();

		startButton = GameObject.Find("Button_StartSimulation")?.GetComponent<Button>();
		viewPathsButton = GameObject.Find("Button_ViewPaths")?.GetComponent<Button>();
		clearPathsButton = GameObject.Find("Button_ClearPaths")?.GetComponent<Button>();
		quitButton = GameObject.Find("Button_Quit")?.GetComponent<Button>();
	}

	void Start()
	{
		// Проверка, чтобы не ловить NullReference
		if (startButton != null) startButton.onClick.AddListener(OnStartSimulation);
		if (viewPathsButton != null) viewPathsButton.onClick.AddListener(OnViewPaths);
		if (clearPathsButton != null) clearPathsButton.onClick.AddListener(OnClearPaths);
		if (quitButton != null) quitButton.onClick.AddListener(OnQuit);
	}

	void OnStartSimulation()
	{
		Debug.Log("▶️ Запуск симуляции...");

		PlayerPrefs.SetInt("ViewMode", 0);

		float x0 = ParseOrDefault(inputX0, -5000f);
		float z0 = ParseOrDefault(inputZ0, 0f);
		float dt = ParseOrDefault(inputDt, 0.01f);
		float w = ParseOrDefault(inputW, 0f);
		float NV = ParseOrDefault(inputNV, 0f);
		int controlLaw = controlLawDropdown != null ? controlLawDropdown.value + 1 : 1;

		PlayerPrefs.SetFloat("X0", x0);
		PlayerPrefs.SetFloat("Z0", z0);
		PlayerPrefs.SetFloat("dt", dt);
		PlayerPrefs.SetFloat("w", w);
		PlayerPrefs.SetFloat("NV", NV);
		PlayerPrefs.SetInt("ControlLaw", controlLaw);
		PlayerPrefs.Save();

		SceneFader.FadeToScene("SimulationScene");
	}

	void OnViewPaths()
	{
		Debug.Log("📈 Просмотр сохранённых путей...");
		PlayerPrefs.SetInt("ViewMode", 1); // режим просмотра
		SceneFader.FadeToScene("SimulationScene");
	}

	void OnClearPaths()
	{
		PlayerPrefs.DeleteKey("SavedPaths");
		Debug.Log("🧹 Все траектории очищены");
	}

	void OnQuit()
	{
		Debug.Log("❌ Выход из игры");
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}

	private float ParseOrDefault(TMP_InputField input, float defaultValue)
	{
		if (input == null || string.IsNullOrWhiteSpace(input.text)) return defaultValue;
		return float.TryParse(input.text, out float result) ? result : defaultValue;
	}
}
