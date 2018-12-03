// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Xamarin_SDK_Sample.iOS
{
    [Register ("InBoxViewController")]
    partial class InBoxViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem ArchiveMessageButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView InBoxTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem MarkAsReadButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIToolbar ToolBar { get; set; }

        [Action ("ArchiveMessageButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ArchiveMessageButton_Activated (UIKit.UIBarButtonItem sender);

        [Action ("MarkAsReadButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void MarkAsReadButton_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (ArchiveMessageButton != null) {
                ArchiveMessageButton.Dispose ();
                ArchiveMessageButton = null;
            }

            if (InBoxTableView != null) {
                InBoxTableView.Dispose ();
                InBoxTableView = null;
            }

            if (MarkAsReadButton != null) {
                MarkAsReadButton.Dispose ();
                MarkAsReadButton = null;
            }

            if (ToolBar != null) {
                ToolBar.Dispose ();
                ToolBar = null;
            }
        }
    }
}