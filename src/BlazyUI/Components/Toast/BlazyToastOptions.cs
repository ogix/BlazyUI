namespace BlazyUI;

public class BlazyToastOptions
{
    public int Duration { get; set; } = 5000;
    public BlazyToastPosition Position { get; set; } = BlazyToastPosition.BottomEnd;
    public bool ShowCloseButton { get; set; } = true;
    public BlazyAlertStyle Style { get; set; } = BlazyAlertStyle.Default;
}
