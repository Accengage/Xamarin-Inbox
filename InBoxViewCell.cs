using Foundation;
using System;
using UIKit;
using AccengageSDK;

namespace Xamarin_SDK_Sample.iOS
{
    public partial class InBoxViewCell : UITableViewCell
    {
		public int index;

        public InBoxViewCell (IntPtr handle) : base (handle)
        {
        }

		public void setMessage(AccengageInboxMessage msg)
		{
			Subject.Text = msg.Title;
			Content.Text = msg.Text;
			Date.Text = InBoxTools.labelTextForDate(msg.Date);

			string categorie = msg.Category;
			Category.Text = (categorie != null) ? categorie : "";
			Category.BackgroundColor = InBoxTools.colorForCategory(Category.Text);

			if (msg.Read)
			{
				Subject.TextColor = UIColor.FromWhiteAlpha(0.4f, 1.0f);
				Content.TextColor = UIColor.FromWhiteAlpha(0.4f, 1.0f);
				StatusMessage.BackgroundColor = UIColor.White;
			}
			else
			{
				Subject.TextColor = UIColor.Black;
				Content.TextColor = UIColor.Black;
				StatusMessage.BackgroundColor = UIColor.FromRGB(0, 121, 255);
			}

			if (msg.Archived)
				StatusMessage.BackgroundColor = UIColor.Red;

			string iconUrl = msg.IconUrl;

			if (iconUrl.Length > 0)
			{
				IconMsg.Hidden = false;
				var request = NSUrlRequest.FromUrl(new NSUrl(iconUrl));
				NSUrlConnection.SendAsynchronousRequest(request, NSOperationQueue.MainQueue,
														(response, data, error) =>
				{
					if (error == null)
						IconMsg.Image = UIImage.LoadFromData(data);
				});
			}
			else
				IconMsg.Hidden = true;
		}

		public void setLoading()
		{
			Subject.Text = "";
			Content.Text = "";
		}
    }
}