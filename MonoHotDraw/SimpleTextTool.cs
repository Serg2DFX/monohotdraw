//
// MonoHotDraw. Diagramming library
//
// Authors:
//	Manuel Cerón <ceronman@gmail.com>
//
// Copyright (C) 2006, 2007, 2008 MonoUML Team (http://www.monouml.org)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using Cairo;
using Gdk;
using Gtk;
using System;

namespace MonoHotDraw {

	// TODO: Catch enter key
	public class SimpleTextTool: FigureTool	{

		public SimpleTextTool (IDrawingEditor editor, SimpleTextFigure fig, ITool dt) 
			: base (editor, fig, dt) {
			_entry = new Gtk.Entry ();
			_entry.HasFrame = false;
			_entry.Alignment = 0.5f;
			_entry.Changed += new System.EventHandler (ChangedHandler);
			_entry.ModifyFont (fig.PangoLayout.FontDescription.Copy ());
		}
		
		private void ChangedHandler (object sender, EventArgs args) {
			((SimpleTextFigure) Figure).Text = _entry.Text;
			CalculateSizeEntry ();
		}
		
		private void CalculateSizeEntry () {
			int padding = (int)(Figure as SimpleTextFigure).Padding;
			RectangleD r = Figure.DisplayBox;
			r.Inflate(-padding, -padding);
			
			// Drawing Coordinates must be translated to View's coordinates in order to 
			// Correctly put the widget in the DrawingView
			PointD point = View.DrawingToView(r.X, r.Y);
			int x = (int) Math.Round(point.X, 0);
			int y = (int) Math.Round(point.Y, 0);
			int w = (int) Math.Max (r.Width, 10.0) + 2; // for some reason Entry needs this 2 pixel space
			int h = (int) Math.Max (r.Height, 10.0);
			
			View.MoveWidget (_entry, x, y);
			_entry.SetSizeRequest (w, h);
		}

		public override void MouseDown (MouseEvent ev) {
			IDrawingView view = ev.View;
			SetAnchorCoords (ev.X, ev.Y);
			View = view;
			
			Gdk.EventType type = ev.GdkEvent.Type;
			
			if (type == EventType.TwoButtonPress) {
				_showingEntry = true;
				_entry.Text = (Figure as SimpleTextFigure).Text;
				
				View.AddWidget (_entry, 0,0);
				CalculateSizeEntry ();
				
				_entry.GrabFocus ();
				_entry.Show ();
				
				return;
			}			
			DefaultTool.MouseDown (ev);
		}

		public override void Activate () {
			_showingEntry = false;
			base.Activate ();
		}

		public override void Deactivate () {
			_entry.Changed -= new System.EventHandler (ChangedHandler);
			View.RemoveWidget (_entry);
			base.Deactivate ();
		}
		
		public override void MouseDrag (MouseEvent ev) {
			if (!_showingEntry) {
				DefaultTool.MouseDrag (ev);
			}
		}

		private Gtk.Entry _entry;
		private bool _showingEntry = false;
	}
}
