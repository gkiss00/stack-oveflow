
using prbd_1920_xyy.Views;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace prbd_1920_xyy {
    public partial class LoginView : WindowBase {
        private string pseudo;
        public string Pseudo {
            get => pseudo;
            set => SetProperty<string>(ref pseudo, value, () => Validate());
        }

        private string password;
        public string Password {
            get => password;
            set => SetProperty<string>(ref password, value, () => Validate());
        }

        public ICommand Login { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand SignUp { get; set; }
        //*****************************************
        //*****************************************
        //CONSTRUCTOR
        //*****************************************
        //*****************************************
        public LoginView()
        {
            InitializeComponent();

            DataContext = this;

            Login = new RelayCommand(LoginAction,
                () => { return pseudo != null && password != null && !HasErrors; });

            Cancel = new RelayCommand(() => Close());

            SignUp = new RelayCommand(SignUpAction);
        }
        //*****************************************
        //*****************************************
        //ACTIONS
        //*****************************************
        //*****************************************
        private void LoginAction()
        {
            if (Validate())
            { // si aucune erreurs
                User user = null; // on recherche le membre
                foreach (User u in App.Model.Users)
                {
                    if (u.UserName.Equals(Pseudo))
                        user = u;
                }
                App.CurrentUser = user; // le membre connecté devient le membre courant
                ShowMainView(); // ouverture de la fenêtre principale
                Close(); // fermeture de la fenêtre de login
            }
        }

        private void SignUpAction()
        {
            ShowSignUpView();
            Close();
        }

        private static void ShowMainView()
        {
            var mainView = new MainView();
            mainView.Show();
            Application.Current.MainWindow = mainView;
        }

        private static void ShowSignUpView()
        {
            var signUpView = new SignupView();
            signUpView.Show();
            Application.Current.MainWindow = signUpView;
        }
        //*****************************************
        //*****************************************
        //VALIDATION AND PERMISSION
        //*****************************************
        //*****************************************
        public override bool Validate() {
            ClearErrors();
            User user = null;
            foreach(User u in App.Model.Users)
            {
                if (u.UserName.Equals(Pseudo))
                    user = u;
            } 
            if (!ValidateLogin(user) || !ValidatePwd(user))
                RaiseErrors();
            return !HasErrors;
        }

        private bool ValidateLogin(User user) {
            if (string.IsNullOrEmpty(Pseudo)) {
                AddError("Pseudo", Properties.Resources.Error_Required);
            }
            else {
                if (Pseudo.Length < 3) {
                    AddError("Pseudo", Properties.Resources.Error_LengthGreaterEqual3);
                }
                else {
                    if (user == null) {
                        AddError("Pseudo", Properties.Resources.Error_DoesNotExist);
                    }
                }
            }
            return !HasErrors;
        }

        private bool ValidatePwd(User user) {
            if (string.IsNullOrEmpty(Password)) {
                AddError("Password", Properties.Resources.Error_Required);
            }
            else if (!user.Password.Equals(Hash(Password))) {
                AddError("Password", Properties.Resources.Error_WrongPassword);
            }
            return !HasErrors;
        }
        //************************************
        //************************************
        //HASHED PASSWORD
        //************************************
        //************************************

        private string Hash(string pwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(pwd));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

    }
}
