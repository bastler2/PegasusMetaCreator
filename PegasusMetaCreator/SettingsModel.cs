using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PegasusMetaCreator
{
    public class SettingsModel
    {
        public string Name { get; set; }
        public string FieldType { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Content 
        { 
            get; 
            set; 
        }

        [Newtonsoft.Json.JsonIgnore]
        public int DefaultTextBoxHeight { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string TextWrapping { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool AcceptsReturn { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string VerticalScrollBarVisibility { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool RemoveEnabled { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool isEnabled { get; set; }

        public ICommand RemoveProperty { get; set; }

        MainWindowViewModel mainWindowViewModel;
        public SettingsModel(string settingType, string settingName, MainWindowViewModel mainWindowViewModel)
        {
            if (settingName == "file")
            {
                isEnabled = false;
                RemoveEnabled = false;
            }
            else if (settingName == "collection" || settingName == "game")
            {
                isEnabled = true;
                RemoveEnabled = false;
            }
            else
            {
                isEnabled = true;
                RemoveEnabled = true;
            }

            this.mainWindowViewModel = mainWindowViewModel;
            RemoveProperty = new DelegateCommand(removeProperty);
            if (settingType == "textbox")
            {
                DefaultTextBoxHeight = 22;
                TextWrapping = "NoWrap";
                AcceptsReturn = false;
                VerticalScrollBarVisibility = "Hidden";
            }
            else if (settingType == "textbox-L")
            {
                DefaultTextBoxHeight = 70;
                TextWrapping = "Wrap";
                AcceptsReturn = true;
                VerticalScrollBarVisibility = "Visible";
            }
            else if (settingType == "textbox-XL")
            {
                DefaultTextBoxHeight = 140;
                TextWrapping = "Wrap";
                AcceptsReturn = true;
                VerticalScrollBarVisibility = "Visible";
            }
        }

        private void removeProperty()
        {
            mainWindowViewModel.Settings.Remove(this);
            mainWindowViewModel.mainWindowModel.saveSettingsJson();
        }
    }
}
