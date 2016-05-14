using SQLitePCL;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace iSee
{
    public enum RegistResult
    {
        RegistOK,
        RegistFail,
        RegistCancel,
        Nothing
    }

    public sealed partial class RegisterContentDialog : ContentDialog
    {
        public RegistResult Result { get; private set; }

        public RegisterContentDialog()
        {
            this.InitializeComponent();
            this.Opened += RegisterContentDialog_Opened;
            this.Closing += RegisterContentDialog_Closing;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Ensure the user name and password fields aren't empty. If a required field
            // is empty, set args.Cancel = true to keep the dialog open.
            if (string.IsNullOrEmpty(userNameTextBox.Text))
            {
                args.Cancel = true;
                errorTextBlock.Text = "请输入用户名";
            }
            else if (string.IsNullOrEmpty(passwordTextBox.Password) || string.IsNullOrEmpty(repasswordTextBox.Password))
            {
                args.Cancel = true;
                errorTextBlock.Text = "请输入密码";
            }
            else if (passwordTextBox.Password != repasswordTextBox.Password)
            {
                args.Cancel = true;
                errorTextBlock.Text = "两次密码输入不相同";
            }
            else
            {
                if (signup_is_vaild(userNameTextBox.Text))
                {
                    this.Result = RegistResult.RegistOK;
                    insert_user(userNameTextBox.Text, passwordTextBox.Password);
                }
                else
                {
                    args.Cancel = true;
                }
            }

            // If you're performing async operations in the button click handler,
            // get a deferral before you await the operation. Then, complete the
            // deferral when the async operation is complete.

            ContentDialogButtonClickDeferral deferral = args.GetDeferral();
            /*
            if (await SomeAsyncSignInOperation())
            {
                this.Result = SignInResult.SignInOK;
            }
            else
            {
                this.Result = SignInResult.SignInFail;
            }
            */
            deferral.Complete();
        }

        public bool signup_is_vaild(string name)
        {
            //User select_user = null;
            using (var statement = App.conn.Prepare("SELECT Id, Name, Password FROM User WHERE Name = ?"))
            {
                statement.Bind(1, name);
                SQLiteResult result = statement.Step();

                while (SQLiteResult.ROW == result)
                {
                    errorTextBlock.Text = "用户名已存在";
                    return false;
                }
                return true;
            }
        }

        public void insert_user(string name, string password)
        {
            try
            {
                using (var add_user = App.conn.Prepare("INSERT INTO User(Name, Password) VALUES(?,?)"))
                {
                    add_user.Bind(1, name);
                    add_user.Bind(2, password);
                    add_user.Step();
                }
            }
            catch
            {
                var i = new MessageDialog("注册失败").ShowAsync();
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // User clicked Cancel.
            this.Result = RegistResult.RegistCancel;
        }

        void RegisterContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            this.Result = RegistResult.Nothing;

            // If the user name is saved, get it and populate the user name field.
            /*
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("userName"))
            {
                userNameTextBox.Text = roamingSettings.Values["userName"].ToString();
                //saveUserNameCheckBox.IsChecked = true;
            }
            */
        }

        void RegisterContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            // If sign in was successful, save or clear the user name based on the user choice.
            /*
            if (this.Result == RegistResult.RegistOK)
            {
                if (saveUserNameCheckBox.IsChecked == true)
                {
                    SaveUserName();
                }
                else
                {
                    ClearUserName();
                }
            }
            */

            // If the user entered a name and checked or cleared the 'save user name' checkbox, then clicked the back arrow,
            // confirm if it was their intention to save or clear the user name without signing in. 
            if (this.Result == RegistResult.Nothing && !string.IsNullOrEmpty(userNameTextBox.Text))
            {
                /*
                if (saveUserNameCheckBox.IsChecked == false)
                {
                    args.Cancel = true;
                    FlyoutBase.SetAttachedFlyout(this, (FlyoutBase)this.Resources["DiscardNameFlyout"]);
                    FlyoutBase.ShowAttachedFlyout(this);
                }
                else if (saveUserNameCheckBox.IsChecked == true && !string.IsNullOrEmpty(userNameTextBox.Text))
                {
                    args.Cancel = true;
                    FlyoutBase.SetAttachedFlyout(this, (FlyoutBase)this.Resources["SaveNameFlyout"]);
                    FlyoutBase.ShowAttachedFlyout(this);
                }
                */
            }
        }
        /*
        private void SaveUserName()
        {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["userName"] = userNameTextBox.Text;
        }

        private void ClearUserName()
        {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["userName"] = null;
            userNameTextBox.Text = string.Empty;
        }

        // Handle the button clicks from the flyouts.
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveUserName();
            FlyoutBase.GetAttachedFlyout(this).Hide();
        }
        
        private void DiscardButton_Click(object sender, RoutedEventArgs e)
        {
            ClearUserName();
            FlyoutBase.GetAttachedFlyout(this).Hide();
        }
        */
        // When the flyout closes, hide the sign in dialog, too.
        private void Flyout_Closed(object sender, object e)
        {
            this.Hide();
        }
    }
}