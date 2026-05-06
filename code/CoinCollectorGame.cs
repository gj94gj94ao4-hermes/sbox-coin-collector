using Sandbox;

namespace CoinCollector;

/// <summary>
/// Coin Collector - A simple s&box game mode.
/// Players run around collecting coins that spawn randomly on the map.
/// The player with the most coins when time runs out wins!
/// </summary>
[Title( "Coin Collector" )]
[Icon( "currency_circle_dollar" )]
public partial class CoinCollectorGame : GameManager
{
	[Net] public int RoundDuration { get; set; } = 120; // seconds
	[Net] public TimeSince TimeSinceRoundStart { get; set; }
	[Net] public bool RoundActive { get; set; } = false;
	[Net] public int MaxCoins { get; set; } = 15;

	/// <summary>
	/// Track all active coins for efficient lookup instead of Entity.All scan.
	/// </summary>
	private List<Coin> ActiveCoins { get; set; } = new();

	private bool _restartPending = false;

	public CoinCollectorGame()
	{
		// Start a new round when the game begins
		StartRound();

		if ( Game.IsServer )
		{
			// Spawn initial coin set
			SpawnCoins();
		}
	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		var pawn = new CoinCollectorPawn();
		client.Pawn = pawn;
		pawn.Respawn();

		// Show welcome message
		CoinCollectorHud.ShowToast( To.Single( client ), "Welcome! Collect coins before time runs out!" );
	}

	public override void ClientDisconnect( IClient client, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( client, reason );
	}

	[GameEvent.Tick.Server]
	public void ServerTick()
	{
		if ( !RoundActive ) return;

		// Check round time
		if ( TimeSinceRoundStart > RoundDuration )
		{
			EndRound();
			return;
		}

		// Replenish coins if too few are on the map
		if ( ActiveCoins.Count < MaxCoins )
		{
			SpawnCoin();
		}
	}

	public void StartRound()
	{
		// Cancel any pending restart to avoid conflicts
		_restartPending = false;

		TimeSinceRoundStart = 0f;
		RoundActive = true;

		// Clean up old coins first
		CleanupCoins();

		// Reset all player scores
		foreach ( var client in Game.Clients )
		{
			if ( client.Pawn is CoinCollectorPawn pawn )
			{
				pawn.Score = 0;
				pawn.Respawn();
			}
		}

		// Spawn fresh coins
		SpawnCoins();

		CoinCollectorHud.Announce( To.Everyone, "GO!", "Collect coins!" );
	}

	public void EndRound()
	{
		RoundActive = false;

		// Find winner
		var winner = Game.Clients
			.OrderByDescending( c => (c.Pawn as CoinCollectorPawn)?.Score ?? 0 )
			.FirstOrDefault();

		var winnerScore = (winner?.Pawn as CoinCollectorPawn)?.Score ?? 0;
		var winnerName = winner?.Name ?? "Nobody";

		CoinCollectorHud.Announce( To.Everyone, "Time's Up!", $"{winnerName} wins with {winnerScore} coins!" );

		// Restart round after 5 seconds (guard against double-restart)
		if ( !_restartPending )
		{
			_restartPending = true;
			_ = RestartAfterDelay();
		}
	}

	async Task RestartAfterDelay()
	{
		await GameTask.DelaySeconds( 5 );

		// Check if a manual restart happened while we waited
		if ( !_restartPending ) return;

		StartRound();
	}

	private void CleanupCoins()
	{
		foreach ( var coin in ActiveCoins.ToList() )
		{
			coin.Delete();
		}
		ActiveCoins.Clear();
	}

	public void SpawnCoins()
	{
		for ( int i = 0; i < MaxCoins; i++ )
		{
			SpawnCoin();
		}
	}

	public void SpawnCoin()
	{
		// Safety cap: don't spawn more than 2x MaxCoins total
		if ( ActiveCoins.Count >= MaxCoins * 2 ) return;

		var coin = new Coin();
		var pos = GetRandomSpawnPosition();
		coin.Position = pos;
		ActiveCoins.Add( coin );
	}

	private Vector3 GetRandomSpawnPosition()
	{
		// Try to find a valid ground position
		for ( int i = 0; i < 10; i++ )
		{
			var x = Game.Random.Float( -800f, 800f );
			var y = Game.Random.Float( -800f, 800f );
			var pos = new Vector3( x, y, 200f );

			var tr = Trace.Ray( pos, pos + Vector3.Down * 400f )
				.IgnoreDynamic()
				.Run();

			if ( tr.Hit )
			{
				return tr.HitPosition + Vector3.Up * 15f;
			}
		}

		// Fallback
		return Vector3.Up * 50f;
	}

	/// <summary>
	/// Called when a player collects a coin.
	/// </summary>
	public void OnCoinCollected( CoinCollectorPawn collector, Coin coin )
	{
		if ( !RoundActive ) return;

		collector.Score += coin.Value;
		ActiveCoins.Remove( coin );
		coin.Delete();

		// Feedback to the collecting player
		CoinCollectorHud.ShowToast( To.Single( collector.Client ), $"+{coin.Value} coins!" );
	}

	[ConCmd.Server( "cc_restart" )]
	public static void RestartGame()
	{
		if ( Game.ActiveGameManager is CoinCollectorGame game )
		{
			game.StartRound();
		}
	}
}