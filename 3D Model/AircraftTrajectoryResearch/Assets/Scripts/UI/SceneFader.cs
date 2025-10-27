using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
	public Image fadeImage;
	public float fadeDuration = 1f;

	private CanvasGroup canvasGroup;
	private static SceneFader instance;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);

		if (fadeImage == null)
			fadeImage = GetComponentInChildren<Image>();

		// Добавляем CanvasGroup для блокировки кликов
		canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null)
			canvasGroup = gameObject.AddComponent<CanvasGroup>();

		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;

		// Подписываемся на загрузку новых сцен
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void Start()
	{
		StartCoroutine(FadeIn());
	}

	// 🟢 Этот метод вызывается Unity после каждой загрузки новой сцены
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		StartCoroutine(FadeIn());
	}

	public static void FadeToScene(string sceneName)
	{
		if (instance != null)
			instance.StartCoroutine(instance.FadeOutAndLoad(sceneName));
		else
			SceneManager.LoadScene(sceneName);
	}

	private IEnumerator FadeIn()
	{
		if (fadeImage == null) yield break;

		Color color = fadeImage.color;
		color.a = 1f;
		fadeImage.color = color;

		canvasGroup.blocksRaycasts = true;

		float t = 0f;
		while (t < fadeDuration)
		{
			t += Time.deltaTime;
			color.a = 1f - (t / fadeDuration);
			fadeImage.color = color;
			yield return null;
		}

		color.a = 0f;
		fadeImage.color = color;
		canvasGroup.blocksRaycasts = false;
	}

	private IEnumerator FadeOutAndLoad(string sceneName)
	{
		if (fadeImage == null)
		{
			SceneManager.LoadScene(sceneName);
			yield break;
		}

		canvasGroup.blocksRaycasts = true;

		Color color = fadeImage.color;
		float t = 0f;

		while (t < fadeDuration)
		{
			t += Time.deltaTime;
			color.a = t / fadeDuration;
			fadeImage.color = color;
			yield return null;
		}

		color.a = 1f;
		fadeImage.color = color;

		SceneManager.LoadScene(sceneName);
	}
}
