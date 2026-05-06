using Sandbox;
using Sandbox.UI;

namespace CoinCollector;

/// <summary>
/// Displays the round timer at the top center of the screen.
/// Turns red when time is running low.
/// </summary>
public partial class TimerPanel : Panel
{
	private Label TimeLabel;
	private Label WarningLabel;

	public TimerPanel()
	{
		StyleSheet.Load( "UI/TimerPanel.scss" );
		TimeLabel = Add.Label( "2:00", "time-display" );
		WarningLabel = Add.Label( "", "time-warning" );
	}

	public override int BuildHash()
	{
		var game = Game.ActiveGameManager as CoinCollectorGame;
		return HashCode.Combine( game?.TimeSinceRoundStart ?? 0, game?.RoundActive ?? false );
	}

	public override void Tick()
	{
		var game = Game.ActiveGameManager as CoinCollectorGame;
		if ( game == null )
		{
			SetClass( "hidden", true );
			return;
		}

		SetClass( "hidden", !game.RoundActive );

		var remaining = Math.Max( 0, game.RoundDuration - game.TimeSinceRoundStart );
		var minutes = (int)(remaining / 60f);
		var seconds = (int)(remaining % 60f);

		TimeLabel.Text = remaining > 0
			? $"{minutes}:{seconds:D2}"
			: "0:00";

		// Urgency styling
		var urgent = remaining < 30f;
		var critical = remaining < 10f;
		SetClass( "urgent", urgent );
		SetClass( "critical", critical );
	}
}