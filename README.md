<!--
# license: Copyright © 2011-2016 MOLPay Sdn Bhd. All Rights Reserved. 
-->

# molpay-mobile-xdk-xamarin-android

This is the complete and functional MOLPay Xamarin Android payment module that is ready to be implemented into Visual Studio as a MOLPayXDK module. An example application project (MOLPayXdkExample) is provided for MOLPayXDK framework integration reference.

## Recommended configurations

    - Microsoft Visual Studio Community 2015 (For Windows)

    - Minimum Android API level: 19 ++

    - Minimum Android target version: Android 4.4

## Installation

    Step 1 - On the Solution Explorer of Visual Studio, right click on your Xamarin Android project name and go to Add -> Existing Item..., on the window that pops up, select MOLPayActivity.cs and click Add.

    Step 2 - Drag and drop molpay-mobile-xdk-www folder (can be separately downloaded at https://github.com/MOLPay/molpay-mobile-xdk-www) into Assets folder of your Xamarin Android project.

    Step 3 - Drag and drop custom.css into Assets folder of your Xamarin Android project.

    Step 4 - Drag and drop layout_molpay.axml into Resources\layout\ folder of your Xamarin Android project. (Create one if the directory does not exist)

    Step 5 - Drag and drop menu_molpay.xml into Resources\menu\ folder of your Xamarin Android project. (Create one if the directory does not exist)

    Step 6 - Install Json.NET by going to Tools -> NuGet Package Manager -> Package Manager Console, paste this Install-Package Newtonsoft.Json into the console and press enter. You may refer to this website http://www.newtonsoft.com/json.

    Step 7 - Override the OnActivityResult function.
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

## Using namespaces

    using MOLPayXDKExample;
    using Newtonsoft.Json;

## Prepare the Payment detail object

    Dictionary<String, object> paymentDetails = new Dictionary<String, object>();
     // Mandatory String. A value more than '1.00'
    paymentDetails.Add(MOLPayActivity.mp_amount, "");

    // Mandatory String. Values obtained from MOLPay
    paymentDetails.Add(MOLPayActivity.mp_username, "");
    paymentDetails.Add(MOLPayActivity.mp_password, "");
    paymentDetails.Add(MOLPayActivity.mp_merchant_ID, "");
    paymentDetails.Add(MOLPayActivity.mp_app_name, "");
    paymentDetails.Add(MOLPayActivity.mp_verification_key, "");

    // Mandatory String. Payment values
    paymentDetails.Add(MOLPayActivity.mp_order_ID, "");
    paymentDetails.Add(MOLPayActivity.mp_currency, "");
    paymentDetails.Add(MOLPayActivity.mp_country, ""); 

    // Optional String.
    paymentDetails.Add(MOLPayActivity.mp_channel, ""); // Use 'multi' for all available channels option. For individual channel seletion, please refer to "Channel Parameter" in "Channel Lists" in the MOLPay API Spec for Merchant pdf. 
    paymentDetails.Add(MOLPayActivity.mp_bill_description, "");
    paymentDetails.Add(MOLPayActivity.mp_bill_name, "");
    paymentDetails.Add(MOLPayActivity.mp_bill_email, "");
    paymentDetails.Add(MOLPayActivity.mp_bill_mobile, "");
    paymentDetails.Add(MOLPayActivity.mp_channel_editing, false); // Option to allow channel selection.
    paymentDetails.Add(MOLPayActivity.mp_editing_enabled, false); // Option to allow billing information editing.

    // Optional for Escrow
    paymentDetails.Add(MOLPayActivity.mp_is_escrow, ""); // Optional for Escrow, put "1" to enable escrow

    // Optional for credit card BIN restrictions
    String[] binlock = new String[] { "", "" };
    paymentDetails.Add(MOLPay.mp_bin_lock, binlock); // Optional for credit card BIN restrictions
    paymentDetails.Add(MOLPay.mp_bin_lock_err_msg, ""); // Optional for credit card BIN restrictions

    // For transaction request use only, do not use this on payment process
    paymentDetails.Add(MOLPayActivity.mp_transaction_id, ""); // Optional, provide a valid cash channel transaction id here will display a payment instruction screen.
    paymentDetails.Add(MOLPayActivity.mp_request_type, ""); // Optional, set 'Status' when performing a transactionRequest

    // Optional for customizing MOLPay UI
    paymentDetails.Add(MOLPayActivity.mp_custom_css_url, "file:///android_asset/custom.css");

    // Optional, set the token id to nominate a preferred token as the default selection
    paymentDetails.Add(MOLPayActivity.mp_preferred_token, "");

## Start the payment module

    Intent intent = new Intent(this, typeof(MOLPayActivity));
    intent.PutExtra(MOLPayActivity.MOLPayPaymentDetails, JsonConvert.SerializeObject(paymentDetails));
    StartActivityForResult(intent, MOLPayActivity.MOLPayXDK);

## Support

Submit issue to this repository or email to our support@molpay.com

Merchant Technical Support / Customer Care : support@molpay.com<br>
Sales/Reseller Enquiry : sales@molpay.com<br>
Marketing Campaign : marketing@molpay.com<br>
Channel/Partner Enquiry : channel@molpay.com<br>
Media Contact : media@molpay.com<br>
R&D and Tech-related Suggestion : technical@molpay.com<br>
Abuse Reporting : abuse@molpay.com