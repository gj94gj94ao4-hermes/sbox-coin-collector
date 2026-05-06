using Sandbox;
using Sandbox.UI;

namespace CoinCollector;

/// <summary>
/// Shows big announcements in the center of the screen (round start/end).
/// Auto-fades after a few seconds.
/// </summary>
public partial class AnnouncementPanel : Panel
{
	private Label TitleLabel;
	private Label SubtitleLabel;
	private TimeSince timeSinceShown = 999f;

	public AnnouncementPanel()
	{
		StyleSheet.Load( "UI/AnnouncementPanel.scss" );
		TitleLabel = Add.Label( "", "announce-title" );
		SubtitleLabel = Add.Label( "", "announce-subtitle" );
	}

	public void ShowAnnouncement( string title, string subtitle )
	{
		TitleLabel.Text = title;
		SubtitleLabel.Text = subtitle;
		timeSinceShown = 0f;
		SetClass( "visible", true );
	}

	public override void Tick()
	{
		if ( timeSinceShown > 3f )
		{
			SetClass( "visible", false );
		}
		else if ( timeSinceShown > 2f )
		{
			// Fade out
			SetClass( "fading", true );
		}
	}
}