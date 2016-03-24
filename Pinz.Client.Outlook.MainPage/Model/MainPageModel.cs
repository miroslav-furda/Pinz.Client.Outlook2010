using Com.Pinz.Client.ServiceConsumer.Service;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pinz.Client.Outlook.MainPage.Model
{
    public class MainPageModel
    {
        [Inject]
        public MainPageModel(UserNameClientCredentials credentials)
        {
            credentials.UserName = "test@test.com";
            credentials.Password = "test";
            credentials.UpdateCredentialsForAllFactories();
        }
    }
}
