using Sandbox;
using Sandbox.UI;

namespace CoinCollector;

/// <summary>
/// Simple crosshair in the center of the screen.
/// </summary>
public partial class CrosshairPanel : Panel
{
	public CrosshairPanel()
	{
		StyleSheet.Load( "UI/CrosshairPanel.scss" );
	}
}