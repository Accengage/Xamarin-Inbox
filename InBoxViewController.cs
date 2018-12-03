using Foundation;
using System;
using UIKit;
using AccengageSDK;
using CoreGraphics;

namespace Xamarin_SDK_Sample.iOS
{
	public partial class InBoxViewController : UIViewController
	{
		AccengageInbox inbox;
		UIRefreshControl refreshControl;

		public InBoxViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NSNotificationCenter notficationCenter = NSNotificationCenter.DefaultCenter;

			// Register for inbox notication
			notficationCenter.AddObserver(AccengageIOS.Constants.BMA4SInBoxDataChanged, inboxChanged);

			// Register for cell notification
			notficationCenter.AddObserver(new NSString("RefreshInbox"), inboxRefresh);

			// Set Title
			// Ex : Inbox (3)
			if (inbox != null)
			{
				if (inbox.UnreadMessageCount == 0)
					Title = "Inbox";
				else
					Title = "Inbox (" + inbox.UnreadMessageCount.ToString() + ")";
			}

			// Initialize the refresh controll
			refreshControl = new UIRefreshControl();
			refreshControl.BackgroundColor = UIColor.FromWhiteAlpha(0, 0.1f);
			refreshControl.AddTarget((sender, e) => { reloadData(); }, UIControlEvent.ValueChanged);
			InBoxTableView.AddSubview(refreshControl);

			InvokeOnMainThread(delegate
			{
				refreshControl.BeginRefreshing();
				refreshControl.EndRefreshing();
			});

			// Add Edit Button
			UIBarButtonItem rigthButton = new UIBarButtonItem("edit", UIBarButtonItemStyle.Plain, startEditing);

			if (NavigationItem != null)
			{
				NavigationItem.RightBarButtonItem = rigthButton;
			}

			InBoxTableView.Delegate = new InBoxTableViewDelegate(this);
			reloadData();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			 // Desable edition
			activeEditionState(false);
			View.AddSubview(ToolBar);
		}

		void startEditing(object sender, EventArgs e)
		{
			activeEditionState(!InBoxTableView.Editing);
		}

		partial void MarkAsReadButton_Activated(UIBarButtonItem sender)
		{
			if (InBoxTableView.IndexPathsForSelectedRows != null)
			{
				foreach (NSIndexPath indexPath in InBoxTableView.IndexPathsForSelectedRows)
				{
					inbox.ObtainMessageAtIndex(indexPath.Row, (message, requestedIndex) =>
					{
						if (message.Read)
							message.Read = false;
						else
							message.Read = true;
					});
				}
			}
		}

		partial void ArchiveMessageButton_Activated(UIBarButtonItem sender)
		{
			if (InBoxTableView.IndexPathsForSelectedRows != null)
			{
				foreach (NSIndexPath indexPath in InBoxTableView.IndexPathsForSelectedRows)
				{
					inbox.ObtainMessageAtIndex(indexPath.Row, (message, requestedIndex) =>
					{
						if (message.Archived)
							message.Archived = false;
						else
							message.Archived = true;
					});
				}
			}
		}

		void reloadData()
		{
			Accengage.GetInbox((arg1) => {
				inbox = arg1;
				updateUIandData();
			});
		}

		// UI management

		void updateUIandData()
		{
			// update table content
			InBoxTableView.DataSource = new InBoxTableViewDataSource(inbox);
			InBoxTableView.ReloadData();

			//title updating
			if (inbox != null)
			{
				if (inbox.UnreadMessageCount == 0)
					Title = "Inbox";
				else
					Title = "Inbox (" + inbox.UnreadMessageCount.ToString() + ")";
			}

			// End the refreshing
			if (refreshControl != null)
			{
				string title = "Last update: " + DateTime.Now.ToString("MMM d, hh:mm");
				NSAttributedString attributedTitle = new NSAttributedString(title, null, UIColor.DarkGray);
				refreshControl.AttributedTitle = attributedTitle;
				refreshControl.EndRefreshing();
			}
		}

		void updateToolBarPosition()
		{
			CGRect frame = ToolBar.Frame;
			nfloat y = (float) View.Bounds.GetMaxY();
			UIEdgeInsets inset = InBoxTableView.ContentInset;

			if (InBoxTableView.Editing)
			{
				y -= ToolBar.Bounds.Size.Height;
				inset.Bottom += ToolBar.Bounds.Size.Height;
			}

			frame.Y = y;
			UIView.Animate(0.3, () => {
				ToolBar.Frame = frame;
				InBoxTableView.ContentInset = inset;
			});
		}

		void activeEditionState(bool actived)
		{
			InBoxTableView.SetEditing(actived, true);

			if (InBoxTableView.Editing)
			{
				NavigationItem.RightBarButtonItem.Title = "Done";
				NavigationItem.RightBarButtonItem.Style = UIBarButtonItemStyle.Done;
			}
			else
			{
				NavigationItem.RightBarButtonItem.Title = "Edit";
				NavigationItem.RightBarButtonItem.Style = UIBarButtonItemStyle.Plain;
			}

			updateToolBarPosition();
			updateUIandData();
		}

		void inboxChanged(NSNotification obj)
		{
			updateUIandData();
		}

		void inboxRefresh(NSNotification obj)
		{
			InBoxTableView.ReloadData();
		}

		class InBoxTableViewDataSource : UITableViewDataSource
		{
			AccengageInbox inbox;

			public InBoxTableViewDataSource(AccengageInbox value)
			{
				inbox = value;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				InBoxViewCell cell = (InBoxViewCell)tableView.DequeueReusableCell("InBoxViewCell", indexPath);

				cell.index = indexPath.Row;
				cell.setLoading();
				inbox.ObtainMessageAtIndex(indexPath.Row, (message, requestedIndex) =>
				{
					if (cell.index == (int) requestedIndex)
						cell.setMessage(message);
				});

				cell.Accessory = UITableViewCellAccessory.None;
				return cell;
			}

			public override nint RowsInSection(UITableView tableView, nint section)
			{
				return (nint)((inbox != null) ? inbox.Size : 0);
			}
		}

		class InBoxTableViewDelegate : UITableViewDelegate
		{
			InBoxViewController InBoxViewController;

			public InBoxTableViewDelegate(InBoxViewController value)
			{
				InBoxViewController = value;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				if (!tableView.Editing)
				{
					tableView.DeselectRow(indexPath, false);
					InBoxViewController.inbox.ObtainMessageAtIndex(indexPath.Row, (message, requestedIndex) =>
					{
						message.InteractWithDisplayHandler((content) =>
						{
							InBoxDetailsViewController controller = (InBoxDetailsViewController) InBoxViewController.Storyboard.InstantiateViewController("InBoxDetailsViewController");
							if (controller != null)
							{
								controller.message = message;
								controller.content = content;
								InBoxViewController.NavigationController.PushViewController(controller, true);
							}
						});
					});
				}
			}
		}
	}
}