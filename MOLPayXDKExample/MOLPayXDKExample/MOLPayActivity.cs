using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Newtonsoft.Json;
using Android.Util;
using System.IO;
using Android.Media;

namespace MOLPayXDKExample
{
    [Activity(Label = "MOLPayXDKExample", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MOLPayActivity : Activity
    {
        public const int MOLPayXDK = 9999;
        public const String MOLPayPaymentDetails = "paymentDetails";
        public const String MOLPayTransactionResult = "transactionResult";
        public const String mp_amount = "mp_amount";
        public const String mp_username = "mp_username";
        public const String mp_password = "mp_password";
        public const String mp_merchant_ID = "mp_merchant_ID";
        public const String mp_app_name = "mp_app_name";
        public const String mp_order_ID = "mp_order_ID";
        public const String mp_currency = "mp_currency";
        public const String mp_country = "mp_country";
        public const String mp_verification_key = "mp_verification_key";
        public const String mp_channel = "mp_channel";
        public const String mp_bill_description = "mp_bill_description";
        public const String mp_bill_name = "mp_bill_name";
        public const String mp_bill_email = "mp_bill_email";
        public const String mp_bill_mobile = "mp_bill_mobile";
        public const String mp_channel_editing = "mp_channel_editing";
        public const String mp_editing_enabled = "mp_editing_enabled";
        public const String mp_transaction_id = "mp_transaction_id";
        public const String mp_request_type = "mp_request_type";
        public const String mp_is_escrow = "mp_is_escrow";
        public const String mp_bin_lock = "mp_bin_lock";
        public const String mp_bin_lock_err_msg = "mp_bin_lock_err_msg";
        public const String mp_custom_css_url = "mp_custom_css_url";
        public const String mp_preferred_token = "mp_preferred_token";
        public const String mp_tcctype = "mp_tcctype";
        public const String mp_is_recurring = "mp_is_recurring";
        public const String mp_sandbox_mode = "mp_sandbox_mode";
        public const String mp_allowed_channels = "mp_allowed_channels";
        public const String mp_express_mode = "mp_express_mode";

        private const String mpopenmolpaywindow = "mpopenmolpaywindow://";
        private const String mpcloseallwindows = "mpcloseallwindows://";
        private const String mptransactionresults = "mptransactionresults://";
        private const String mprunscriptonpopup = "mprunscriptonpopup://";
		private const String mppinstructioncapture = "mppinstructioncapture://";
        private const String molpayresulturl = "https://www.onlinepayment.com.my/MOLPay/result.php";
        private const String molpaynbepayurl = "https://www.onlinepayment.com.my/MOLPay/nbepay.php";
        private const String module_id = "module_id";
        private const String wrapper_version = "wrapper_version";
        private static MOLPayActivity molpayActivity;
        private static WebView mpMainUI, mpMOLPayUI, mpBankUI;
        private static TextView tw;
        private static Dictionary<String, object> paymentDetails;
        private static String transactionResults;
        private static Boolean isMainUILoaded = false;
        private static Boolean isClosingReceipt = false;

		public class Image
		{
			public string filename { get; set; }
			public string base64ImageUrlData { get; set; }
		}

        private static void CloseMolpay()
        {
            mpMainUI.LoadUrl("javascript:closemolpay()");
            if (isClosingReceipt)
            {
                isClosingReceipt = false;
                molpayActivity.PassTransactionResultBack(transactionResults);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            molpayActivity = this;

            Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);
            SetContentView(Resource.Layout.layout_molpay);

            String json = Intent.GetStringExtra(MOLPayPaymentDetails);
            paymentDetails = JsonConvert.DeserializeObject<Dictionary<String, object>>(json);
            paymentDetails.Add(module_id, "molpay-mobile-xdk-xamarin-android");
            paymentDetails.Add(wrapper_version, "0");

            mpMainUI = FindViewById<WebView>(Resource.Id.MPMainUI);
            mpMOLPayUI = FindViewById<WebView>(Resource.Id.MPMOLPayUI);
            tw = FindViewById<TextView>(Resource.Id.resultTV);

            mpMainUI.Settings.JavaScriptEnabled = true;
            mpMOLPayUI.Settings.JavaScriptEnabled = true;

            mpMOLPayUI.Visibility = ViewStates.Gone;
            tw.Visibility = ViewStates.Gone;

            mpMainUI.LoadUrl("file:///android_asset/molpay-mobile-xdk-www/index.html");
            mpMainUI.SetWebViewClient(new MPMainUIWebClient());
            
            mpMOLPayUI.SetWebViewClient(new MPMOLPayUIWebClient());
            mpMOLPayUI.SetWebChromeClient(new MPMOLPayUIWebChromeClient());
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_molpay, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.closeBtn:
                    CloseMolpay();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public class MPMainUIWebClient : WebViewClient
        {
            public override Boolean ShouldOverrideUrlLoading(WebView view, string url)
            {
                Console.WriteLine("MPMainUIWebClient shouldOverrideUrlLoading url = " + url);

				if (url != null && url.StartsWith(mpopenmolpaywindow))
				{
					String base64String = url.Replace(mpopenmolpaywindow, "");
					Console.WriteLine("MPMainUIWebClient mpopenmolpaywindow base64String = " + base64String);

					String dataString = Base64Decode(base64String);
					Console.WriteLine("MPMainUIWebClient mpopenmolpaywindow dataString = " + dataString);

					if (dataString.Length > 0)
					{
						Console.WriteLine("MPMainUIWebClient mpopenmolpaywindow success");
						mpMOLPayUI.LoadDataWithBaseURL("", dataString, "text/html", "UTF-8", "");
						mpMOLPayUI.Visibility = ViewStates.Visible;
					}
					else
					{
						Console.WriteLine("MPMainUIWebClient mpopenmolpaywindow empty dataString");
					}
				}
				else if (url != null && url.StartsWith(mpcloseallwindows))
				{
					if (mpBankUI != null)
					{
						mpBankUI.LoadUrl("about:blank");
						mpBankUI.Visibility = ViewStates.Gone;
						mpBankUI.ClearCache(true);
						mpBankUI.ClearHistory();
						mpBankUI = null;
					}
					mpMOLPayUI.LoadUrl("about:blank");
					mpMOLPayUI.Visibility = ViewStates.Gone;
					mpMOLPayUI.ClearCache(true);
					mpMOLPayUI.ClearHistory();
				}
				else if (url != null && url.StartsWith(mptransactionresults))
				{
					String base64String = url.Replace(mptransactionresults, "");
					Console.WriteLine("MPMainUIWebClient mptransactionresults base64String = " + base64String);

					String dataString = Base64Decode(base64String);
					Console.WriteLine("MPMainUIWebClient mptransactionresults dataString = " + dataString);

					try
					{
						Dictionary<String, object> jsonResult = JsonConvert.DeserializeObject<Dictionary<String, object>>(dataString);

						Console.WriteLine("MPMainUIWebClient jsonResult = " + jsonResult.ToString());

						Object requestType;
						jsonResult.TryGetValue("mp_request_type", out requestType);
						if (!jsonResult.ContainsKey("mp_request_type") || (String)requestType != "Receipt" || jsonResult.ContainsKey("error_code"))
						{
							molpayActivity.PassTransactionResultBack(dataString);
						}
						else
						{
							transactionResults = dataString;
							//molpayActivity.Window.ClearFlags(WindowManagerFlags.Secure);
							isClosingReceipt = true;
						}
					}
					catch (Exception)
					{
						molpayActivity.PassTransactionResultBack(dataString);
					}
				}
				else if (url != null && url.StartsWith(mprunscriptonpopup))
				{
					String base64String = url.Replace(mprunscriptonpopup, "");
					Console.WriteLine("MPMainUIWebClient mprunscriptonpopup base64String = " + base64String);

					String jsString = Base64Decode(base64String);
					Console.WriteLine("MPMainUIWebClient mprunscriptonpopup jsString = " + jsString);

					if (mpBankUI != null)
					{
						mpBankUI.LoadUrl("javascript:" + jsString);
						Console.WriteLine("mpBankUI loadUrl = " + "javascript:" + jsString);
					}
                }
                else if (url != null && url.StartsWith(mppinstructioncapture))
                {
                    String base64String = url.Replace(mppinstructioncapture, "");
                    Console.WriteLine("MPMainUIWebClient mppinstructioncapture base64String = " + base64String);

                    String jsString = Base64Decode(base64String);
                    Console.WriteLine("MPMainUIWebClient mppinstructioncapture jsString = " + jsString);

                    var json = JsonConvert.DeserializeObject<Image>(jsString);
                    byte[] imageAsBytes = Base64.Decode(json.base64ImageUrlData, Base64Flags.Default);
                    Bitmap bitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);

                    var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                    var filePath = System.IO.Path.Combine(sdCardPath.ToString(), json.filename);
                    var stream = new FileStream(filePath, FileMode.Create);

                    bool compress = bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                    stream.Close();

                    MediaScannerConnection.ScanFile(Application.Context, new String[] { filePath }, null, null);

                    if (compress)
                    {
                        Toast.MakeText(molpayActivity, "Image saved", ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(molpayActivity, "Image not saved", ToastLength.Short).Show();
                    }
                }

                return true;
            }

            public override void OnPageFinished(WebView view, String url)
            {
                if (!isMainUILoaded && url != "about:blank")
                {
                    isMainUILoaded = true;
                    
                    mpMainUI.LoadUrl("javascript:updateSdkData(" + JsonConvert.SerializeObject(paymentDetails) + ")");
                }
            }
        }

        public class MPMOLPayUIWebClient : WebViewClient
        {
            public override void OnPageStarted(WebView view, string url, Bitmap favicon)
            {
                Console.WriteLine("MPMOLPayUIWebClient onPageStarted url = " + url);

                if (url != null && url.StartsWith(molpayresulturl))
                {
                    NativeWebRequestUrlUpdates(url);
                }
            }
        }

        public class MPMOLPayUIWebChromeClient : WebChromeClient
        {
            public override Boolean OnCreateWindow(WebView view, bool dialog, bool userGesture, Message resultMsg)
            {
                Console.WriteLine("MPMOLPayUIWebChromeClient onCreateWindow resultMsg = " + resultMsg);

                RelativeLayout container = (RelativeLayout)molpayActivity.FindViewById(Resource.Id.MPContainer);

                mpBankUI = new WebView(molpayActivity);

                mpBankUI.Settings.JavaScriptEnabled = true;
                mpBankUI.Settings.AllowUniversalAccessFromFileURLs = true;
                mpBankUI.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                mpBankUI.Settings.SupportMultipleWindows();
                mpBankUI.SetWebViewClient(new MPBankUIWebClient());

                container.AddView(mpBankUI);
                WebView.WebViewTransport transport = (WebView.WebViewTransport)resultMsg.Obj;
                transport.WebView = mpBankUI;
                resultMsg.SendToTarget();
                return true;
            }
        }

        public class MPBankUIWebClient : WebViewClient
        {
            public override void OnPageStarted(WebView view, String url, Bitmap favicon)
            {
                Console.WriteLine("MPBankUIWebClient onPageStarted url = " + url);

                if (url != null && url.StartsWith(molpayresulturl))
                {
                    NativeWebRequestUrlUpdates(url);
                }
            }

            public override void OnPageFinished(WebView view, String url)
            {
                Console.WriteLine("MPBankUIWebClient onPageFinished url = " + url);
                NativeWebRequestUrlUpdatesOnFinishLoad(url);
            }
        }

        private static void NativeWebRequestUrlUpdates(String url)
        {
            Console.WriteLine("nativeWebRequestUrlUpdates url = " + url);

            Dictionary<String, String> data = new Dictionary<String, String>();
            data.Add("requestPath", url);
            
            mpMainUI.LoadUrl("javascript:nativeWebRequestUrlUpdates(" + JsonConvert.SerializeObject(data) + ")");
        }

        private static void NativeWebRequestUrlUpdatesOnFinishLoad(String url)
        {
            Console.WriteLine("nativeWebRequestUrlUpdatesOnFinishLoad url = " + url);

            Dictionary<String, String> data = new Dictionary<String, String>();
            data.Add("requestPath", url);
            
            mpMainUI.LoadUrl("javascript:nativeWebRequestUrlUpdatesOnFinishLoad(" + JsonConvert.SerializeObject(data) + ")");
        }

        private static String Base64Encode(String plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private static String Base64Decode(String base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private void PassTransactionResultBack(String dataString)
        {
            Intent result = new Intent();
            result.PutExtra(MOLPayTransactionResult, dataString);
            SetResult(Result.Ok, result);
            Finish();
        }
    }
}