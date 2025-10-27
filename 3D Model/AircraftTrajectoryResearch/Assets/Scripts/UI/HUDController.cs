using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
	[Header("References")]
	public FlightSimulationController flightController;

	[Header("UI Elements")]
	public TextMeshProUGUI rollText;
	public TextMeshProUGUI yawText;
	public TextMeshProUGUI fuelText;

	private void Update()
	{
		if (flightController == null || flightController.result == null)
			return;

		int i = flightController.CurrentIndexSafe();

		if (i < 0 || i >= flightController.result.G_p.Count)
			return;

		float psi = (float)flightController.result.Psi_g[i];   // рысканье
		float roll = (float)flightController.result.Gamma[i];   // рысканье
		float fuel = (float)flightController.result.G_p[i];     // остаток топлива

		rollText.text = $"<b>Roll:</b> {roll:F1}°";
		yawText.text = $"<b>Yaw:</b> {psi:F1}°";
		fuelText.text = $"<b>Fuel:</b> {fuel:F0} kg";
	}
}
