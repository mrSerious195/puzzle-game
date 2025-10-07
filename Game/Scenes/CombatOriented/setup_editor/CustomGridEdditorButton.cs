using Godot;
using System;

public partial class CustomGridEdditorButton : PanelContainer
{
    [Signal]
    public delegate void PressedEventHandler();
    [Export]
    private GridContainer _celContainer;
    [Export]
    private Vector2I HVseperator = new Vector2I(3,3) ; 
    private bool _enabled = true;
    public bool Enabled{ get => _enabled; set { _enabled = value; _celContainer.Visible = value; } }

    public void Setbutton(Color color, Vector2I outerbound, bool[] filledCells,bool isSelected)
    {
        if (isSelected)
        {
            this.Modulate = Color.Color8(255, 255, 255);
        }
        else
        {
        this.Modulate = Color.Color8(200, 200, 200);
       }
        _celContainer.Columns = outerbound.X;
        var gridchildren = _celContainer.GetChildren();
        for (int i = gridchildren.Count - 1; i >= 0; i--)
        {
            gridchildren[i].QueueFree();
        }
        float panelSide = (CustomMinimumSize.Y-HVseperator.Y*(outerbound.Y-1)) / outerbound.Y;
        if (panelSide*outerbound.X+HVseperator.X*(outerbound.X-1) > CustomMinimumSize.X)
        {
             panelSide = (CustomMinimumSize.X-HVseperator.X*(outerbound.X-1))/ outerbound.X;
        }
        Vector2 panelsize =new Vector2(panelSide,panelSide);
        for (int i = 0; i < filledCells.Length; i++)
        {
            float a = filledCells[i] ? 1 : 0;
            Color c = new Color(color.R, color.G, color.B, a);
            Panel p = new Panel()
            {
                Modulate = c,
                MouseFilter = MouseFilterEnum.Pass,
                CustomMinimumSize = panelsize
            };
            _celContainer.AddChild(p);
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (Enabled&&@event is InputEventMouseButton mousebuttonEvent)
        {
            if (mousebuttonEvent.ButtonMask == MouseButtonMask.Left && mousebuttonEvent.Pressed)
            {
                EmitSignal(SignalName.Pressed);
               
        }
       }
    }

}
