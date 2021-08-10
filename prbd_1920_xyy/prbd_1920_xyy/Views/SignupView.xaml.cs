using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace prbd_1920_xyy.Views
{
    public partial class SignupView : WindowBase
    {
        private string userName;
        public string UserName
        {
            get => userName;
            set => SetProperty<string>(ref userName, value, () => Validate());
        }

        private string password;
        public string Password
        {
            get => password;
            set => SetProperty<string>(ref password, value, () => Validate());
        }

        private string passwordConfirm;

        public string PasswordConfirm
        {
            get => passwordConfirm;
            set => SetProperty<string>(ref passwordConfirm, value, () => Validate());
        }

        private string fullName;

        public string FullName
        {
            get => fullName;
            set => SetProperty<string>(ref fullName, value, () => Validate());
        }

        private string email;

        public string Email
        {
            get => email;
            set => SetProperty<string>(ref email, value, () => Validate());
        }

        public ICommand Login { get; set; }
        public ICommand Signup { get; set; }
        public ICommand Cancel { get; set; }
        //****************************************************
        //****************************************************
        //CONSTRUCTOR
        //****************************************************
        //****************************************************
        public SignupView()
        {
            DataContext = this;

            Signup = new RelayCommand(SignupAction,
                () => {
                    return userName != null && password != null && passwordConfirm != null && fullName != null
                && email != null && !HasErrors;
                });

            Login = new RelayCommand(LoginAction);

            Cancel = new RelayCommand(() => Close());
            InitializeComponent();
        }
        //****************************************************
        //****************************************************
        //ACTIONS
        //****************************************************
        //****************************************************
        private void SignupAction()
        {
            if (Validate())
            {
                App.Model.CreateUser(UserName, Password, FullName, Email);
                App.Model.SaveChanges();
                var user = (from u in App.Model.Users
                            where u.UserName == UserName
                            select u).FirstOrDefault();
                App.CurrentUser = user;
                ShowMainView();
                Close();
            }
        }

        private void LoginAction()
        {
            ShowLoginView();
            Close();
        }
        //****************************************************
        //****************************************************
        //VALIDATION && PERMISSION
        //****************************************************
        //****************************************************
        public override bool Validate()
        {
            ClearErrors();
            if (!ValidateUserName() || !ValidatePassword() || !ValidatePasswordConfirm() || !ValidateFullName() || !ValidateEmail())
                RaiseErrors();
            return !HasErrors;
        }

        private bool ValidateUserName()
        {
            if (string.IsNullOrEmpty(UserName) || IsEmpty(UserName))
            {
                AddError("UserName", Properties.Resources.Error_Required);
            }
            else
            {
                if (UserName.Length < 3)
                {
                    AddError("UserName", Properties.Resources.Error_LengthGreaterEqual3);
                }
                else
                {
                    if ((from u in App.Model.Users where u.UserName == UserName select u).Count() > 0)
                    {
                        AddError("UserName", Properties.Resources.Error_UserNameTaken);
                    }
                }
            }
            return !HasErrors;
        }

        private bool ValidateFullName()
        {
            if (string.IsNullOrEmpty(FullName) || IsEmpty(FullName))
            {
                AddError("FullName", Properties.Resources.Error_Required);
            }
            else
            {
                if (FullName.Length < 3)
                {
                    AddError("FullName", Properties.Resources.Error_LengthGreaterEqual3);
                }
            }
            return !HasErrors;
        }

        private bool ValidatePassword()
        {
            var number = new Regex(@"[0-9]+");
            var upperChar = new Regex(@"[A-Z]+");
            if (string.IsNullOrEmpty(Password))
            {
                AddError("Password", Properties.Resources.Error_Required);
            }
            else
            {
                if (Password.Length < 8)
                {
                    AddError("Password", Properties.Resources.Error_LengthGreaterEqual8);
                }
                else if (!number.IsMatch(Password) || !upperChar.IsMatch(Password) || !CheckForSpe())
                {
                    AddError("Password", Properties.Resources.Error_Password_Format);
                }
            }
            return !HasErrors;
        }

        private bool ValidatePasswordConfirm()
        {
            if (string.IsNullOrEmpty(PasswordConfirm))
            {
                AddError("PasswordConfirm", Properties.Resources.Error_PasswordMisMatch);
            }
            else if (!PasswordConfirm.Equals(Password))
            {
                AddError("PasswordConfirm", Properties.Resources.Error_PasswordMisMatch);
            }
            return !HasErrors;
        }

        private bool ValidateEmail()
        {
            var isValid = new EmailAddressAttribute().IsValid(Email);
            if (!isValid)
            {
                AddError("Email", Properties.Resources.Error_WrongMailFormat);
            }
            return !HasErrors;
        }

        private static void ShowMainView()
        {
            var mainView = new MainView();
            mainView.Show();
            Application.Current.MainWindow = mainView;
        }

        private static void ShowLoginView()
        {
            var loginView = new LoginView();
            loginView.Show();
            Application.Current.MainWindow = loginView;
        }

        //****************************************************
        //****************************************************
        //UTILS
        //****************************************************
        //****************************************************
        private bool CheckForSpe()
        {
            string spe = "/['~`\\!@#$%^&*()_-+={}[]|;:\"<>,.?]";
            foreach (char c in Password)
            {
                foreach (char x in spe)
                {
                    if (c == x)
                        return (true);
                }
            }
            return (false);
        }

        private bool IsEmpty(string str)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] != ' ')
                    return (false);
            }
            return (true);
        }
    }
}
