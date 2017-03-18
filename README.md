# MVC-Payment-Authorize.net
Make easly Payment Processes using [Authorize.Net](https://Authorize.Net/)  Payment Gateway integrated with ASP.NET MVC.

This repository contains simple mvc form which demonstrate Payment Transactions using Authorize.Net .NET SDK


Use Authorize.Net SDK to integrate with the Authorize.Net AIM, SIM, ARB and CIM APIs for Payment Transactions, Recurring Billing and Customer Payment Profiles.

# Installation
To install AuthorizeNet, run the following command in the Package Manager Console
```
PM>Install-Package AuthorizeNet
```

Sign Up for a Sandbox account for test environment at [https://developer.authorize.net/hello_world/sandbox](https://developer.authorize.net/hello_world/sandbox/)

# Code

Create View Model class for your form inputs you need
````csharp
    public class PaymentModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Postcode { get; set; }
        public string Phone { get; set; }
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public string CardCode { get; set; } //CCV code
        public string Month { get; set; }
        public string Year { get; set; }
    }
````
Response View Model for transaction results
````csharp
    public class TransactionResponse
    {
        public AuthorizeNet.Api.Contracts.V1.messageTypeEnum resultCode { get; set; }
        public string transId { get; set; }
        public string responseCode { get; set; }
        public string messageCode { get; set; }
        public string authCode { get; set; }
        public string description { get; set; }


        public string errorCode { get; set; }
        public string errorText { get; set; }
    }
````




Create Payment Process Class which is containing Charge Carde method
````csharp
    public class PaymentProcesses
    {
        // These are default api login id and transaction key .
        // You can create your own keys by signing up for a sandbox account here: https://sandbox.authorize.net/
        const string apiLoginId = "8ceE8M2K"; 
        const string transactionKey = "59V9E7vS9vW8A4eB";


        public TransactionResponse ChargeCredit(PaymentModel payment)
        {
            // determine run Environment to SANDBOX for developemnt level
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = transactionKey,
            };

            var creditCard = new creditCardType
            {
                cardNumber = payment.CardNumber,//"Ex: x111111111111111",
                expirationDate = payment.Month + payment.Year,//"Ex: 0522"
                cardCode = payment.CardCode,//"Ex: 111"
            };

            var billingAddress = new customerAddressType
            {
                firstName = payment.FirstName,
                lastName = payment.LastName,
                city = payment.Address1,
                address = payment.Address2,
                zip = payment.Postcode,
                phoneNumber = payment.Phone
            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = creditCard };

            // Add line Items you pay to obtain these
            var lineItems = new lineItemType[2];
            lineItems[0] = new lineItemType { itemId = "1", name = "t-shirt", quantity = 2, unitPrice = new Decimal(1.00) };
            lineItems[1] = new lineItemType { itemId = "2", name = "snowboard", quantity = 1, unitPrice = new Decimal(1.00) };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card

                amount = 2,
                payment = paymentType,
                billTo = billingAddress,
                lineItems = lineItems
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the contoller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            var resCode = controller.GetResultCode();
            var resAll = controller.GetResults();
            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            //validate

            TransactionResponse result = new TransactionResponse();
            if (response != null)
            {
                result.resultCode = response.messages.resultCode;
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {
                        result.transId = response.transactionResponse.transId;
                        result.responseCode = response.transactionResponse.responseCode;
                        result.messageCode = response.transactionResponse.messages[0].code;
                        result.description = response.transactionResponse.messages[0].description;
                        result.authCode = response.transactionResponse.authCode;
                    }
                    else
                    {
                        if (response.transactionResponse.errors != null)
                        {
                            result.errorCode = response.transactionResponse.errors[0].errorCode;
                            result.errorText = response.transactionResponse.errors[0].errorText;
                        }
                    }
                }
                else
                {
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        result.errorCode = response.transactionResponse.errors[0].errorCode;
                        result.errorText = response.transactionResponse.errors[0].errorText;
                    }
                    else
                    {
                        result.errorCode = response.messages.message[0].code;
                        result.errorText = response.messages.message[0].text;
                    }
                }
            }
            else
            {
                result.errorCode = "NONE";
                result.errorText = "Failed Transaction,, Unkown Error";
                
            }
            return result;
        }
    }
````

# Referance 
[https://github.com/AuthorizeNet/sample-code-csharp](https://github.com/AuthorizeNet/sample-code-csharp)
