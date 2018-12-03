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
    [Register ("InBoxViewCell")]
    partial class InBoxViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Category { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Date { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView IconMsg { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView StatusMessage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Subject { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Category != null) {
                Category.Dispose ();
                Category = null;
            }

            if (Content != null) {
                Content.Dispose ();
                Content = null;
            }

            if (Date != null) {
                Date.Dispose ();
                Date = null;
            }

            if (IconMsg != null) {
                IconMsg.Dispose ();
                IconMsg = null;
            }

            if (StatusMessage != null) {
                StatusMessage.Dispose ();
                StatusMessage = null;
            }

            if (Subject != null) {
                Subject.Dispose ();
                Subject = null;
            }
        }
    }
}