//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SlidingPanels
{
	[Register ("ExampleContentB")]
	partial class ExampleContentB
	{
		MonoTouch.UIKit.UIButton DoSomething { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel CenterText { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (DoSomething != null) {
				DoSomething.Dispose ();
				DoSomething = null;
			}

			if (CenterText != null) {
				CenterText.Dispose ();
				CenterText = null;
			}
		}	}
}

