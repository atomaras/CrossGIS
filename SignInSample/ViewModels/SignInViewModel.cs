using System;
using System.Net;
using System.Windows.Input;
using ESRI.ArcGIS.Client;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SignInSample.ViewModels
{
    public class SignInViewModel : ViewModelBase
    {

        public SignInViewModel()
        {
            IdentityManager.Current.ChallengeMethodEx = DoSignInEx;

            SignInCommand = new RelayCommand(SignIn, CanSignIn);
        }

        private string user;
        public string Username
        {
            get { return user; }
            set
            {
                user = value;
                RaisePropertyChanged(() => Username);
                ((RelayCommand)SignInCommand).RaiseCanExecuteChanged();
            }
        }

        private string pwd;
        public string Password
        {
            get { return pwd; }
            set
            {
                pwd = value;
                RaisePropertyChanged(() => Password);
                ((RelayCommand)SignInCommand).RaiseCanExecuteChanged();
            }
        }

        private string url;
        public string Url
        {
            get { return url; }
            set
            {
                bool resetUserPass = url != value;
                url = value;
                RaisePropertyChanged(() => Url);
                if (resetUserPass)
                {
                    Username = null;
                    Password = null;
                }
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { Set(() => IsActive, ref _isActive, value); }
        }

        public ICommand SignInCommand { get; private set; }

        private bool CanSignIn()
        {
            return CurrentChallenge != null && 
                !String.IsNullOrEmpty(Username) && 
                !String.IsNullOrEmpty(Password);
        }

        private void SignIn()
        {
            var currentChallenge = CurrentChallenge;

            CurrentChallenge = null;

            if (currentChallenge.Infos.AuthenticationType == IdentityManager.AuthenticationType.NetworkCredential)
            {
                var networkCredential = new IdentityManager.Credential()
                                            {
                                                Credentials = new NetworkCredential(Username, Password)
                                            };
                currentChallenge.Callback(networkCredential, null);
            }
            else
            {
                //Token Authentication
                IdentityManager.Current.GenerateCredentialAsync(
                    currentChallenge.Infos.Url, 
                    Username, 
                    Password,
                    currentChallenge.Callback,
                    currentChallenge.GenerateTokenOptions);
            }
            IsActive = false;
        }


        private ChallengeRequest CurrentChallenge { get; set; }

        public void DoSignInEx(
            IdentityManager.CredentialRequestInfos infos,
            Action<IdentityManager.Credential, Exception> completeHandler,
            IdentityManager.GenerateTokenOptions generateTokenOptions)
        {
            Url = infos.Url;

            CurrentChallenge = new ChallengeRequest()
                                   {
                                       Infos = infos,
                                       Callback = completeHandler,
                                       GenerateTokenOptions = generateTokenOptions
                                   };
            IsActive = true;
        }
    }

    public class ChallengeRequest
    {
        public IdentityManager.CredentialRequestInfos Infos { get; set; }
        public Action<IdentityManager.Credential, Exception> Callback { get; set; }
        public IdentityManager.GenerateTokenOptions GenerateTokenOptions { get; set; }
    }
}
