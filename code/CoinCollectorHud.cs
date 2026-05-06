using Sandbox;
using Sandbox.UI;

namespace CoinCollector;

/// <summary>
/// The HUD entity - this is the entry point for all UI in Coin Collector.
/// It creates and manages the HUD panels.
/// </summary>
[HideInEditor]
public partial class CoinCollectorHud : HudEntity<RootPanel>
{
	public CoinCollectorHud()
	{
		if ( !Game.IsClient ) return;

		RootPanel.Style.FlexWrap = Wrap.Wrap;
		RootPanel.Style.Position = PositionMode.Absolute;
		RootPanel.Style.Width = Length.Percent( 100 );
		RootPanel.Style.Height = Length.Percent( 100 );

		RootPanel.AddChild<ScorePanel>();
		RootPanel.AddChild<TimerPanel>();
		RootPanel.AddChild<AnnouncementPanel>();
		RootPanel.AddChild<ToastPanel>();
		RootPanel.AddChild<CrosshairPanel>();
	}

	/// <summary>
	/// Shows a brief floating toast notification to a specific client.
	/// </summary>
	[ClientRpc]
	public static void ShowToast( string message )
	{
		var hud = Game.RootPanel?.GetChild<ToastPanel>();
		hud?.ShowToast( message );
	}

	/// <summary>
	/// Shows an announcement (big text) to all clients or specific ones.
	/// </summary>
	[ClientRpc]
	public static void Announce( string title, string subtitle )
	{
		var hud = Game.RootPanel?.GetChild<AnnouncementPanel>();
		hud?.ShowAnnouncement( title, subtitle );
	}
}