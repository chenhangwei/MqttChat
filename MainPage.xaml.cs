using Chat.ViewModels;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using Chat.Services;



namespace Chat
{
    public partial class MainPage : ContentPage
    {

     
   
       
        public MainPage(MainPageViewModel vm)
        {
         
            InitializeComponent();

            BindingContext=vm;
        }
      
    }   
  
}