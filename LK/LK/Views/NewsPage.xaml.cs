﻿using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LK.Views
{
    public partial class NewsPage : ContentPage
    {
        AuthenticationResult authResult;

        public NewsPage(AuthenticationResult ar)
        {
            InitializeComponent();
            authResult = ar;
        }

        
    }
}
