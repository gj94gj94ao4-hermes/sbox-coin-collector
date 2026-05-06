using Sandbox;
using Sandbox.UI;

namespace CoinCollector;

/// <summary>
/// Small floating toast messages for feedback like "+1 coins!".
/// </summary>
public partial class ToastPanel : Panel
{
	public ToastPanel()
	{
		StyleSheet.Load( "UI/ToastPanel.scss" );
	}

	public void ShowToast( string message )
	{
		var toast = AddChild<ToastItem>( message );
		_ = FadeAndRemove( toast, 1.5f );
	}

	async Task FadeAndRemove( Panel panel, float delay )
	{
		await GameTask.DelaySeconds( delay );
		panel.Delete();
	}
}

public class ToastItem : Panel
{
	public ToastItem( string message )
	{
		Add.Label( message, "toast-text" );
		AddClass( "toast-item" );
	}
}