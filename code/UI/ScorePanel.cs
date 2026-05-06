using Sandbox;
using Sandbox.UI;

namespace CoinCollector;

/// <summary>
/// Displays the player's coin score in the top-left corner.
/// </summary>
public partial class ScorePanel : Panel
{
	private Label ScoreLabel;
	private int displayedScore = 0;

	public ScorePanel()
	{
		StyleSheet.Load( "UI/ScorePanel.scss" );
		ScoreLabel = Add.Label( "0", "score-value" );
		Add.Label( "COINS", "score-label" );
	}

	public override int BuildHash()
	{
		var pawn = Game.LocalClient?.Pawn as CoinCollectorPawn;
		return HashCode.Combine( pawn?.Score ?? 0 );
	}

	public override void Tick()
	{
		var pawn = Game.LocalClient?.Pawn as CoinCollectorPawn;
		var score = pawn?.Score ?? 0;

		// Smooth score counting
		if ( displayedScore != score )
		{
			displayedScore = score;
			ScoreLabel.Text = score.ToString();

			// Pulse animation on change
			AddClass( "scored" );
			_ = DelayedRemoveClass( "scored", 0.3f );
		}

		SetClass( "hidden", pawn == null );
	}

	async Task DelayedRemoveClass( string className, float delay )
	{
		await GameTask.DelaySeconds( delay );
		RemoveClass( className );
	}
}