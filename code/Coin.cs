using Sandbox;

namespace CoinCollector;

/// <summary>
/// A collectible coin entity. Spins and glows to attract players.
/// Has a Value property so we can have different coin types later.
/// </summary>
[Title( "Coin" )]
[Category( "Coin Collector" )]
[Icon( "currency_circle_dollar" )]
public partial class Coin : AnimatedEntity
{
	[Net] public int Value { get; set; } = 1;
	[Net] public TimeSince TimeSinceSpawn { get; set; }

	private float spinSpeed = 120f;
	private float bobSpeed = 2f;
	private float bobAmount = 3f;
	private Vector3 basePosition;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen_cointoss/cointoss_coin.vmdl" );
		EnableDrawing = true;
		EnableHitboxes = true;
		EnableAllCollisions = true;
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		basePosition = Position;
		TimeSinceSpawn = 0f;

		// Glow effect
		Transmit = TransmitType.Always;
	}

	[GameEvent.Tick.Client]
	public void ClientTick()
	{
		// Spinning animation
		Rotation = Rotation.RotateAroundAxis( Vector3.Up, spinSpeed * Time.Delta );

		// Bob up and down
		Position = basePosition + Vector3.Up * MathF.Sin( Time.Now * bobSpeed ) * bobAmount;

		// Scale pulse effect
		var scale = 1f + MathF.Sin( Time.Now * 3f ) * 0.05f;
		Scale = scale;
	}

	[GameEvent.Tick.Server]
	public void ServerTick()
	{
		// Optional: remove coins that have been around too long
		// if ( TimeSinceSpawn > 30f ) Delete();
	}
}