using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls.Shapes;

namespace GoDoque.Canvas;

public partial class DiagramCanvas : ContentView
{
	public DiagramCanvas()
	{
		InitializeComponent();
	}

	private void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
	{
		
	}

	private void DropGestureRecognizer_DragOver(object sender, DragEventArgs e)
	{
		//change position of the border
    }

	private void DragGestureRecognizer_DragStarting(object sender, DragStartingEventArgs e)
	{
        Border shape = (sender as Element).Parent as Border;
        e.Data.Properties.Add("Square", new Border { WidthRequest = shape.Width, HeightRequest = shape.Height, }); //Save coordinates of the border
    }
}