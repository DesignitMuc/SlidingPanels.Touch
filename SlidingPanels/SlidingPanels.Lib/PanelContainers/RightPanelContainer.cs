/// Copyright (C) 2013 Pat Laplante & Frank Caico
///
///	Permission is hereby granted, free of charge, to  any person obtaining a copy 
/// of this software and associated documentation files (the "Software"), to deal 
/// in the Software without  restriction, including without limitation the rights 
/// to use, copy,  modify,  merge, publish,  distribute,  sublicense, and/or sell 
/// copies of the  Software,  and  to  permit  persons  to   whom the Software is 
/// furnished to do so, subject to the following conditions:
///
///		The above  copyright notice  and this permission notice shall be included 
///     in all copies or substantial portions of the Software.
///
///		THE  SOFTWARE  IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
///     OR   IMPLIED,   INCLUDING  BUT   NOT  LIMITED   TO   THE   WARRANTIES  OF 
///     MERCHANTABILITY,  FITNESS  FOR  A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
///     IN NO EVENT SHALL  THE AUTHORS  OR COPYRIGHT  HOLDERS  BE  LIABLE FOR ANY 
///     CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT 
///     OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION  WITH THE SOFTWARE OR 
///     THE USE OR OTHER DEALINGS IN THE SOFTWARE.
/// -----------------------------------------------------------------------------

using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace SlidingPanels.Lib.PanelContainers
{
	/// <summary>
	/// Container class for Sliding Panels located on the right edge of the device screen
	/// </summary>
	public class RightPanelContainer : PanelContainer
	{
		#region Data Members

		/// <summary>
		/// starting X Coordinate of the top view
		/// </summary>
		private float _topViewStartXPosition = 0.0f;

		/// <summary>
		/// X coordinate where the user touched when starting a slide operation
		/// </summary>
		private float _touchPositionStartXPosition = 0.0f;

		/// <summary>
		/// Gets the panel position.
		/// </summary>
		/// <value>The panel position.</value>
		public RectangleF PanelPosition
		{
			get
			{
				var rect2 = UIApplication.SharedApplication.KeyWindow.Frame;
				var width = (InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight) ? Math.Max (rect2.Height, rect2.Width) : Math.Min (rect2.Height, rect2.Width);
				var height = (InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight) ? Math.Min (rect2.Height, rect2.Width) : Math.Max (rect2.Height, rect2.Width);


				var rect = new RectangleF 
				{
					X = width - Size.Width,
					Y = 0,
					Width = Size.Width,
					Height = height
				};

				View.Bounds = new RectangleF (rect.X, rect.Y, width, height);
				View.Frame = View.Bounds;

				return rect;
			}
		}

		#endregion

		#region Construction / Destruction

		/// <summary>
		/// Initializes a new instance of the <see cref="SlidingPanels.Lib.PanelContainers.RightPanelContainer"/> class.
		/// </summary>
		/// <param name="panel">Panel.</param>
		public RightPanelContainer (UIViewController panel) : base(panel, PanelType.RightPanel) {
			View.UserInteractionEnabled = true;
		}

		#endregion

		#region View Lifecycle

		/// <summary>
		/// Called after the Panel is loaded for the first time
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			PanelVC.View.Frame = PanelPosition;
		}

		/// <summary>
		/// Called whenever the Panel is about to become visible
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			PanelVC.View.Frame = PanelPosition;
		}

		void traverseToRoot(UIView view) {
			Console.WriteLine ("UserInteraction enabled: {0}", view.UserInteractionEnabled);
			if (view.Superview != null) {
				traverseToRoot (view.Superview);
			}
		}


		public override void ViewDidAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			PanelVC.View.Frame = PanelPosition;

			traverseToRoot (View);
		}

		#endregion

		#region Position Methods

		/// <summary>
		/// Returns a rectangle representing the location and size of the top view 
		/// when this Panel is showing
		/// </summary>
		/// <returns>The top view position when slider is visible.</returns>
		/// <param name="topViewCurrentFrame">Top view current frame.</param>
		public override RectangleF GetTopViewPositionWhenSliderIsVisible(RectangleF topViewCurrentFrame)
		{
			if (InterfaceOrientation == UIInterfaceOrientation.Portrait || InterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown) {
				topViewCurrentFrame.X = - Size.Width;
				return topViewCurrentFrame;
			} else {
				topViewCurrentFrame.X = 0;
				return topViewCurrentFrame;
			}
		}

		/// <summary>
		/// Returns a rectangle representing the location and size of the top view 
		/// when this Panel is hidden
		/// </summary>
		/// <returns>The top view position when slider is visible.</returns>
		/// <param name="topViewCurrentFrame">Top view current frame.</param>
		public override RectangleF GetTopViewPositionWhenSliderIsHidden(RectangleF topViewCurrentFrame)
		{
			topViewCurrentFrame.X = 0;
			return topViewCurrentFrame;
		}

		#endregion

		#region Sliding Methods

		/// <summary>
		/// Determines whether this instance can start sliding given the touch position and the 
		/// current location/size of the top view. 
		/// Note that touchPosition is in Screen coordinate.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="touchPosition">Touch position.</param>
		/// <param name="topViewCurrentFrame">Top view's current frame.</param>
		public override bool CanStartSliding(PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			if (!IsVisible)
			{
				return (touchPosition.X >= View.Bounds.Size.Width - EdgeTolerance && touchPosition.X <= View.Bounds.Size.Width);
			}
			else
			{
				return topViewCurrentFrame.Contains (touchPosition);
			}
		}

		/// <summary>
		/// Called when sliding has started on this Panel
		/// </summary>
		/// <param name="touchPosition">Touch position.</param>
		/// <param name="topViewCurrentFrame">Top view current frame.</param>
		public override void SlidingStarted (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			_touchPositionStartXPosition = touchPosition.X;
			_topViewStartXPosition = topViewCurrentFrame.X;
		}

		/// <summary>
		/// Called while the user is sliding this Panel
		/// </summary>
		/// <param name="touchPosition">Touch position.</param>
		/// <param name="topViewCurrentFrame">Top view current frame.</param>
		public override RectangleF Sliding (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			float screenWidth = View.Bounds.Size.Width;
			float panelWidth = Size.Width;
			float leftEdge = screenWidth - panelWidth;
			float translation = touchPosition.X - _touchPositionStartXPosition;

			RectangleF frame = topViewCurrentFrame;

			frame.X = _topViewStartXPosition + translation;

			float y = frame.X + frame.Width;

			if (y >= screenWidth) 
			{ 
				frame.X = 0; 
			}

			if (y <= leftEdge) 
			{ 
				frame.X = leftEdge - frame.Width; 
			}

			return frame;
		}

		/// <summary>
		/// Determines if a slide is complete
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="touchPosition">Touch position.</param>
		/// <param name="topViewCurrentFrame">Top view current frame.</param>
		public override bool SlidingEnded (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			float screenWidth = View.Bounds.Size.Width;
			float panelWidth = Size.Width;

			float y = topViewCurrentFrame.X + topViewCurrentFrame.Width;
			return (y < (screenWidth - (panelWidth / 2)));
		}

		#endregion

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation) {
			var o = UIDevice.CurrentDevice.Orientation;

			base.DidRotate (fromInterfaceOrientation);
		}


		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt) {
			Console.WriteLine ("Rightpaneltouch");
		}
	}
}

