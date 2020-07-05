using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Windows.Input;

namespace PegasusMetaCreator
{
    public class AutoFillModel
    {
        public string SettingName { get; set; }
        public string AppendFront { get; set; }
        public string RegexFilter { get; set; }
        public string AppendEnd { get; set; }
        public bool UseRegexFullMatch { get; set; }
        public int SelectedRegexGroupe { get; set; }

        [JsonIgnore]
        public ICommand DeleteAutoFilter { get; set; }

        MainWindowViewModel mainWindowViewModel;

        public AutoFillModel(MainWindowViewModel mainWindowViewModel)
        {
            DeleteAutoFilter = new DelegateCommand(deleteAutoFilter);
            this.mainWindowViewModel = mainWindowViewModel;
        }

        private void deleteAutoFilter()
        {
            mainWindowViewModel.AutoFill.Remove(this);
            mainWindowViewModel.mainWindowModel.saveAutoFillJson();
        }
    }
}
