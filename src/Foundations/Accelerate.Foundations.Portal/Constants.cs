using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Portal
{
    public struct Constants
    {
        public struct Keys
        {
            public const string SessionAccountKey = "session:account:key";
            public const string SessionAccountObject = "session:account:object";
        }
        public struct Roles
        {
            public const string KYCAddressVerified = "KYC:Address:Verified";
            public const string KYCIdentityVerified = "KYC:Identity:Verified";
            public const string KYCFundingVerified = "KYC:Funding:Verified";
            public const string AccountIndividual = "Account:Customer:Individual";
            public const string AccountBusiness = "Account:Customer:Business";
            public const string AccountCreated = "Account:Created";

            public const string KYCComplete = "KYC:Completed";

            public const string RatesQuote = "Rates:Quote:Access";

            public const string TransactionsTransaction = "Transactions:Transaction:Access";

            public const string FundingBankAccount = "Funding:BankAccount:Access";
            public const string FundingWallet = "Funding:Wallet:Access";
        }
        public struct AdminPaths
        {
            public const string AccountsPath = "/Admin/Accounts";
            public const string AccountsLabel = "Accounts";
            public const string TransactionsPath = "/Admin/Transactions";
            public const string TransactionsLabel = "Transactions";
            public const string RatesPath = "/Admin/Rates";
            public const string RatesLabel = "Rates";
            public const string JobsPath = "/Admin/Jobs";
            public const string JobsLabel = "Jobs";
            public const string ActionsPath = "/Admin/Actions";
            public const string ActionsLabel = "Actions";
            public const string UsersPath = "/Admin/Users";
            public const string UsersLabel = "Users";
        }
        public struct Paths
        {
            public const string SearchPath = "/Search";
            public const string SearchLabel = "Search";
            public const string AboutPath = "/About";
            public const string AboutLabel = "About";
            
            // Financial

            public const string AccountsPath = "/Accounts";
            public const string AccountsLabel = "Accounts";

            public const string OnboardingPath = "/Onboarding";
            public const string OnboardingLabel = "Onboarding";

            public const string TransactionsPath = "/Transactions";
            public const string TransactionsLabel = "Transactions";

            public const string FundingPath = "/Funding";
            public const string FundingLabel = "Funding";

            public const string SettlementsPath = "/Settlements";
            public const string SettlementsLabel = "Settlements";

            public const string ProfilePath = "/Profile";
            public const string ProfileLabel = "Profile";
            public const string NotificationsPath = "/Account/Notifications";
            public const string NotificationsLabel = "Notifications";
            public const string MediaPath = "/Account/Media";
            public const string MediaLabel = "Media";
            public const string LogoutPath = "/authentication/Logout";
            public const string LogoutLabel = "Logout";
            public const string LoginPath = "/authentication/Login";
            public const string LoginLabel = "Login";
            public const string LockedPath = "/authentication/lockout";
        }
    }
}