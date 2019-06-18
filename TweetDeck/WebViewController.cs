using System;

using Foundation;
using UIKit;

namespace TweetDeck
{
    public partial class WebViewController : UIViewController
    {
        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        protected WebViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Intercept URL loading to handle native calls from browser
            WebView.ShouldStartLoad += HandleShouldStartLoad;

            NSUrlRequest url = NSUrlRequest.FromUrl(NSUrl.FromString("https://tweetdeck.twitter.com"));
            WebView.LoadRequest(url);
            WebView.LoadFinished += (sender, e) =>
            {
                WebView.EvaluateJavascript("javascript:function tjLoadScript(a){var b=new XMLHttpRequest;b.open(\"GET\",a+\"?v=\"+Date.now(),!0),b.responseType=\"text\";var c=eval;b.addEventListener(\"load\",function(){return 200!==b.status&&304!==b.status?void alert(\"script\\u306E\\u53D6\\u5F97\\u306B\\u5931\\u6557\\u3057\\u307E\\u3057\\u305F\"):void c(b.responseText)}),b.send()}tjLoadScript(\"https://p.eriri.net/tj-deck/android/tj-deck.js\");");
            };


            // Perform any additional setup after loading the view, typically from a nib.
        }

        public void loadModule(UIWebView view)
        {

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        bool HandleShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            // If the URL is not our own custom scheme, just let the webView load the URL as usual
            const string scheme = "hybrid:";

            if (request.Url.Scheme != scheme.Replace(":", ""))
                return true;

            // This handler will treat everything between the protocol and "?"
            // as the method name.  The querystring has all of the parameters.
            var resources = request.Url.ResourceSpecifier.Split('?');
            var method = resources[0];
            var parameters = System.Web.HttpUtility.ParseQueryString(resources[1]);

            if (method == "UpdateLabel")
            {
                var textbox = parameters["textbox"];

                // Add some text to our string here so that we know something
                // happened on the native part of the round trip.
                var prepended = string.Format("C# says: {0}", textbox);

                // Build some javascript using the C#-modified result
                var js = string.Format("SetLabelText('{0}');", prepended);

                webView.EvaluateJavascript(js);
            }

            return false;
        }
    }
}

