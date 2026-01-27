namespace BlazyUI;

public class ToastOptions
{
    public int Duration { get; set; } = 5000;
    public ToastPosition Position { get; set; } = ToastPosition.BottomEnd;
    public bool ShowCloseButton { get; set; } = true;
    public BlazyAlertStyle Style { get; set; } = BlazyAlertStyle.Default;
}
