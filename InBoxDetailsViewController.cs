using Foundation;
using System;
using UIKit;
using AccengageSDK;
using System.Collections.Generic;
using CoreGraphics;

namespace Xamarin_SDK_Sample.iOS
{
    public partial class InBoxDetailsViewController : UIViewController
    {

		public AccengageInboxMessage message;
		public AccengageInboxMessageContent content;

		public InBoxDetailsViewController (IntPtr handle) : base (handle)
        {
        }

		// View life cycle

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationController.NavigationBar.Translucent = false;

			// Setup tool bar with custom buttons
			setupToolBar();

			Subject.Text = message.Title;
			Sender.Text = "Expéditeur : " + message.From;

			// Category
			Category.Text = (message.Category != null) ? message.Category : "";
			Category.BackgroundColor = InBoxTools.colorForCategory(message.Category);

			ReceiptDate.Text = "Reçu : " + InBoxTools.labelTextForDate(message.Date);

			Details.Text = message.Text;

			switch (content.Type)
			{
				case AccengageInboxMessageContentType.Text:
					Webview.Hidden = true;
					TextView.Text = content.Body;
					break;
				case AccengageInboxMessageContentType.Web:
					TextView.Hidden = true;
					Loader.StartAnimating();
					Webview.Alpha = 0;
					Webview.ScrollView.Bounces = false;
					Webview.Delegate = new WebviewDelegate(Webview, Loader);
					Webview.LoadRequest(new NSUrlRequest(new NSUrl(content.Body)));
					break;
			}

			if (message.IconUrl.Length > 0)
			{
				NSMutableUrlRequest request = new NSMutableUrlRequest(new NSUrl(message.IconUrl));
				NSUrlConnection.SendAsynchronousRequest(request, NSOperationQueue.MainQueue, (response, data, error) =>
				{
					if (error == null)
						IconMsg.Image = UIImage.LoadFromData(data);
				});
			}
			else 
				IconMsg.Hidden = true; 

			NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidChangeStatusBarOrientationNotification, deviceOrientationDidChangeNotification);

			updateForDeviceOrientation();
		}

		// UI management

		void setupToolBar()
		{
			UIBarButtonItem space = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null, null);

			UIBarButtonItem markAsRead = createBarButton((message.Read) ? "Mark as unread" : "Mark as read", UIBarButtonItemStyle.Plain, markAsUnreadAction);

			UIBarButtonItem archive = createBarButton((message.Archived) ? "Restore" : "Delete", UIBarButtonItemStyle.Plain, archiveAction);

			List<UIBarButtonItem> buttons;
			if (content.Buttons.Count == 0)
				buttons = new List<UIBarButtonItem> { space, markAsRead, space, archive, space };
			else
				buttons = new List<UIBarButtonItem> { space };

			int i = 0;
			foreach (AccengageInboxButton button in content.Buttons)
			{
				UIBarButtonItem aButton = createBarButton(button.Title, UIBarButtonItemStyle.Plain, interact);
				aButton.Tag = i;
				buttons.Add(aButton);
				buttons.Add(space);
				i++;
			}

			ToolBar.SetItems(buttons.ToArray(), false);
		}

		void updateDetailsViewFrame()
		{
			CGRect detailViewFrame = DetailView.Frame;

			if (!TransitionButton.Selected)
				detailViewFrame.Height = 155;
			else
				detailViewFrame.Height = TransitionButton.Frame.Height;

			DetailView.Frame = detailViewFrame;
		}

		void updateToolbarFrame()
		{
			CGRect toolBarFrame = ToolBar.Frame;

			toolBarFrame.Y = View.Bounds.GetMaxY();

			if (!TransitionButton.Selected)
				toolBarFrame.Y -= toolBarFrame.Height;

			ToolBar.Frame = toolBarFrame;
		}

		void updateRichContentViewFrame()
		{
			CGRect richContentViewFrame = RichContentView.Frame;
			CGRect toolBarFrame = ToolBar.Frame;
			CGRect detailViewFrame = DetailView.Frame;

			richContentViewFrame.Y = detailViewFrame.Height + detailViewFrame.Y;
			richContentViewFrame.Height = toolBarFrame.Y - richContentViewFrame.Y;

			RichContentView.Frame = richContentViewFrame;
		}

		void updateDetailsAlpha()
		{
			foreach (UIView view in DetailCollection)
				view.Alpha = Convert.ToInt32(!TransitionButton.Selected);
		}

		void updateSubjectFrame()
		{
			CGRect subjectFrame = Subject.Frame;

			if (!TransitionButton.Selected)
			{
				subjectFrame.X = Sender.Frame.X;
				subjectFrame.Y = IconMsg.Frame.Y;
				subjectFrame.Height = Sender.Frame.Height;
			}
			else
			{
				subjectFrame.X = IconMsg.Frame.X;
				subjectFrame.Y = TransitionButton.Frame.Y;
				subjectFrame.Height = TransitionButton.Frame.Height;
			}

			Subject.Frame = subjectFrame;
		}

		void updateForDeviceOrientation()
		{
			UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;

			if (orientation == UIInterfaceOrientation.Portrait)
				showDetails(true);
			else if (orientation == UIInterfaceOrientation.LandscapeLeft || orientation == UIInterfaceOrientation.LandscapeRight)
				showDetails(false);
		}

		// Tools

		UIBarButtonItem createBarButton(string title, UIBarButtonItemStyle style, EventHandler action)
		{
			return new UIBarButtonItem(title, style, action);
		}

		// Webview delegate

		class WebviewDelegate : UIWebViewDelegate
		{
			UIWebView webview;
			UIActivityIndicatorView loader;

			public WebviewDelegate(UIWebView arg1, UIActivityIndicatorView arg2)
			{
				webview = arg1;
				loader = arg2;	
			}

			public override void LoadingFinished(UIWebView webView)
			{
				stopLoadingAnimation();
			}

			public override void LoadFailed(UIWebView webView, NSError error)
			{
				stopLoadingAnimation();
			}

			void stopLoadingAnimation()
			{
				if (loader.IsAnimating)
					webview.Alpha = 1;
				loader.StopAnimating();
			}
		}

		// Actions

		void markAsUnreadAction(object sender, EventArgs e)
		{
			if (message.Read)
				message.Read = false;
			else
				message.Read = true;

			setupToolBar();
		}

		void archiveAction(object sender, EventArgs e)
		{
			if (message.Archived)
				message.Archived = false;
			else
				message.Archived = true;

			setupToolBar();
		}

		void interact(object sender, EventArgs e)
		{
			UIBarButtonItem button = sender as UIBarButtonItem;
			var tag = (int) button.Tag;
			content.Buttons[tag].Interact();
		}

		partial void TransitionButton_TouchUpInside(UIButton sender)
		{
			showDetails(TransitionButton.Selected);
		}

		void showDetails(bool show)
		{
			if (show != TransitionButton.Selected)
				return;

			TransitionButton.Selected = !show;

			UIView.Animate(0.2, () =>
			{
				updateDetailsViewFrame();
				updateDetailsAlpha();
				updateToolbarFrame();
				updateRichContentViewFrame();
			});
		}

		// Orientation change notification

		void deviceOrientationDidChangeNotification(NSNotification notification)
		{
			UIInterfaceOrientation newOrientation = UIApplication.SharedApplication.StatusBarOrientation;
			UIInterfaceOrientation oldOrientation = (UIInterfaceOrientation) Int32.Parse(notification.UserInfo[UIApplication.StatusBarOrientationUserInfoKey].ToString());

			if ((newOrientation == UIInterfaceOrientation.Portrait) != (oldOrientation == UIInterfaceOrientation.LandscapeLeft || oldOrientation == UIInterfaceOrientation.LandscapeRight))
				updateForDeviceOrientation();
		}
    }
}