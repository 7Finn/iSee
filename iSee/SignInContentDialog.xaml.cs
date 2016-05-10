﻿using SQLitePCL;
using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace iSee
{
    public enum SignInResult
    {
        SignInOK,
        SignInFail,
        SignInCancel,
        Nothing
    }

    public sealed partial class SignInContentDialog : ContentDialog
    {
        public SignInResult Result { get; private set; }

        public SignInContentDialog()
        {
            this.InitializeComponent();
            this.Opened += SignInContentDialog_Opened;
            this.Closing += SignInContentDialog_Closing;
        }

        public static User current_user = null;

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// 
        

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Ensure the user name and password fields aren't empty. If a required field
            // is empty, set args.Cancel = true to keep the dialog open.
            
            if (string.IsNullOrEmpty(userNameTextBox.Text))
            {
                args.Cancel = true;
                errorTextBlock.Text = "User name is required.";
            }
            else if (string.IsNullOrEmpty(passwordTextBox.Password))
            {
                args.Cancel = true;
                errorTextBlock.Text = "Password is required.";
            } else
            {
                if (signin_is_vaild(userNameTextBox.Text, passwordTextBox.Password))
                {
                    //ViewModel.AddUser(userNameTextBox.Text, passwordTextBox.Password);
                    this.Result = SignInResult.SignInOK;
                    //insert_user(userNameTextBox.Text, passwordTextBox.Password);
                } else
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
    

        public bool signin_is_vaild(string name, string password)
        {
            //List<User> listUser = new List<User>();
            var db = new SQLiteConnection("isee_user.db");
            User select_user = null;
            using (var statement = db.Prepare("SELECT Id, Name, Password FROM User WHERE Name = ?"))
            {
                statement.Bind(1, name);
                SQLiteResult result = statement.Step();
                
                while (SQLiteResult.ROW == result)
                {             
                    if (statement[2].ToString() != password)
                    {
                        errorTextBlock.Text = "用户名或密码错误";
                        return false;
                    }
                    else
                    {
                        select_user = new User(statement[0].ToString(), statement[1].ToString(), statement[2].ToString());
                        current_user = select_user;
                        return true;
                    }
                }
                
                errorTextBlock.Text = "用户名不存在";
                return false;
            }
        }

        /*private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrEmpty(userNameTextBox.Text))
            {
                args.Cancel = true;
                errorTextBlock.Text = "User name is required.";
            }
            else if (string.IsNullOrEmpty(passwordTextBox.Password))
            {
                args.Cancel = true;
                errorTextBlock.Text = "Password is required.";
            }
            else
            {
                if (signup_is_vaild(userNameTextBox.Text))
                {
                    this.Result = SignInResult.SignInOK;
                    insert_user(userNameTextBox.Text, passwordTextBox.Password);
                } else
                {

                }
            }
        }*/
        public bool signup_is_vaild(string name)
        {
            var db = new SQLiteConnection("isee_user.db");
            //User select_user = null;
            using (var statement = db.Prepare("SELECT Id, Name, Password FROM User WHERE Name = ?"))
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
            var db = new SQLiteConnection("isee_user.db");
            try
            {
                using (var add_user = db.Prepare("INSERT INTO User(Name, Password) VALUES(?,?)"))
                {
                    add_user.Bind(1, name);
                    add_user.Bind(2, password);
                    add_user.Step();
                }
            }
            catch
            {
                var i = new MessageDialog("sign up failed").ShowAsync();
            }
        }



        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // User clicked Cancel.
            this.Result = SignInResult.SignInCancel;
        }

        void SignInContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            this.Result = SignInResult.Nothing;

            // If the user name is saved, get it and populate the user name field.
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("userName"))
            {
                userNameTextBox.Text = roamingSettings.Values["userName"].ToString();
                saveUserNameCheckBox.IsChecked = true;
            }
        }

        void SignInContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            // If sign in was successful, save or clear the user name based on the user choice.
            if (this.Result == SignInResult.SignInOK)
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

            // If the user entered a name and checked or cleared the 'save user name' checkbox, then clicked the back arrow,
            // confirm if it was their intention to save or clear the user name without signing in. 
            if (this.Result == SignInResult.Nothing && !string.IsNullOrEmpty(userNameTextBox.Text))
            {
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
            }
        }

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

        // When the flyout closes, hide the sign in dialog, too.
        private void Flyout_Closed(object sender, object e)
        {
            this.Hide();
        }

    }
}