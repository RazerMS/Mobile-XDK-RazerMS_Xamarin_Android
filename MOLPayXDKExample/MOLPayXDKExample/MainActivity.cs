using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MOLPayXDKExample;
using Newtonsoft.Json;

namespace MainActivity
{
    [Activity(Label = "MOLPayXDKExample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == MOLPayActivity.MOLPayXDK && resultCode == Result.Ok)
            {
                Console.WriteLine("MOLPay result = " + data.GetStringExtra(MOLPayActivity.MOLPayTransactionResult));
                SetContentView(Resource.Layout.layout_molpay);
                TextView tw = (TextView)FindViewById(Resource.Id.resultTV);
                tw.Text = data.GetStringExtra(MOLPayActivity.MOLPayTransactionResult);
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Dictionary<string, object> paymentDetails = new Dictionary<string, object>();
            paymentDetails.Add(MOLPayActivity.mp_amount, "");
            paymentDetails.Add(MOLPayActivity.mp_username, "");
            paymentDetails.Add(MOLPayActivity.mp_password, "");
            paymentDetails.Add(MOLPayActivity.mp_merchant_ID, "");
            paymentDetails.Add(MOLPayActivity.mp_app_name, "");
            paymentDetails.Add(MOLPayActivity.mp_order_ID, "");
            paymentDetails.Add(MOLPayActivity.mp_verification_key, "");
            paymentDetails.Add(MOLPayActivity.mp_currency, "MYR");
            paymentDetails.Add(MOLPayActivity.mp_country, "MY"); 
            paymentDetails.Add(MOLPayActivity.mp_channel, "multi");
            paymentDetails.Add(MOLPayActivity.mp_bill_description, "description");
            paymentDetails.Add(MOLPayActivity.mp_bill_name, "name");
            paymentDetails.Add(MOLPayActivity.mp_bill_email, "example@email.com");
            paymentDetails.Add(MOLPayActivity.mp_bill_mobile, "+60123456789");
            paymentDetails.Add(MOLPayActivity.mp_channel_editing, false);
            paymentDetails.Add(MOLPayActivity.mp_editing_enabled, false);
            paymentDetails.Add(MOLPayActivity.mp_dev_mode, true);
            //paymentDetails.Add(MOLPayActivity.mp_is_escrow, "");
            //paymentDetails.Add(MOLPayActivity.mp_transaction_id, "");
            //paymentDetails.Add(MOLPayActivity.mp_request_type, "");
            //string[] binlock = new string[] { "", "" };
            //paymentDetails.Add(MOLPayActivity.mp_bin_lock, binlock);
            //paymentDetails.Add(MOLPayActivity.mp_bin_lock_err_msg, "");
            //paymentDetails.Add(MOLPayActivity.mp_custom_css_url, "file:///android_asset/custom.css");
            //paymentDetails.Add(MOLPayActivity.mp_preferred_token, "");
            //paymentDetails.Add(MOLPayActivity.mp_tcctype, "");
            //paymentDetails.Add(MOLPayActivity.mp_is_recurring, false);
            //paymentDetails.Add(MOLPayActivity.mp_sandbox_mode, true);
            //string[] allowedChannels = new string[] { "", "" };
            //paymentDetails.Add(MOLPayActivity.mp_allowed_channels, allowedChannels);
            //paymentDetails.Add(MOLPayActivity.mp_express_mode, false);
            //paymentDetails.Add(MOLPayActivity.mp_advanced_email_validation_enabled, false);
            //paymentDetails.Add(MOLPayActivity.mp_advanced_phone_validation_enabled, false);
            //paymentDetails.Add(MOLPayActivity.mp_bill_name_edit_disabled, true);
            //paymentDetails.Add(MOLPayActivity.mp_bill_email_edit_disabled, true);
            //paymentDetails.Add(MOLPayActivity.mp_bill_mobile_edit_disabled, true);
            //paymentDetails.Add(MOLPayActivity.mp_bill_description_edit_disabled, true);

            Intent intent = new Intent(this, typeof(MOLPayActivity));
            intent.PutExtra(MOLPayActivity.MOLPayPaymentDetails, JsonConvert.SerializeObject(paymentDetails));
            StartActivityForResult(intent, MOLPayActivity.MOLPayXDK);
        }
    }
}