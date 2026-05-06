using Sandbox;

namespace CoinCollector;

/// <summary>
/// The player pawn - a controllable character that runs around collecting coins.
/// Uses the built-in walk controller for movement.
/// </summary>
public partial class CoinCollectorPawn : AnimatedEntity
{
	[Net] public int Score { get; set; } = 0;

	public ClothingContainer Clothing = new();

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen/citizen.vmdl" );
		Transmit = TransmitType.Always;
		EnableDrawing = true;
		EnableHitboxes = true;
		EnableAllCollisions = true;

		// Dress the citizen with random clothing
		Clothing.LoadDefault();
		Clothing.DressEntity( this );
	}

	public void Respawn()
	{
		Position = new Vector3( 0, 0, 10 );
		Rotation = Rotation.Identity;
		Score = 0;

		EnableDrawing = true;
		EnableHitboxes = true;
		EnableAllCollisions = true;

		// Give a walking controller
		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();
	}

	public override void Simulate( IClient client )
	{
		if ( Controller != null )
		{
			Controller.Simulate( client );
		}

		// Simple coin pickup via overlap test (server only)
		if ( Game.IsServer )
		{
			var overlaps = Entity.FindInSphere( Position, 30f );
			foreach ( var ent in overlaps )
			{
				if ( ent is Coin coin && ent != this )
				{
					var game = Game.ActiveGameManager as CoinCollectorGame;
					game?.OnCoinCollected( this, coin );
				}
			}
		}
	}

	public override void BuildInput()
	{
		// Input is handled by the WalkController automatically
		base.BuildInput();
	}

	public override void FrameSimulate( IClient client )
	{
		base.FrameSimulate( client );

		// Update the camera on client
		Camera.Rotation = ViewAngles.ToRotation();
		Camera.Position = Position + Vector3.Up * 65f;
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 70f );
	}
}