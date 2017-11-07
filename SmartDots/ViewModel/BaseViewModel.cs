using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmartDots.Helpers;

namespace SmartDots.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        //private ICommand newCommand;
        //protected ICommand deleteCommand;
        //protected ICommand detailsCommand;
        //protected ICommand statusCommand;
        //private ICommand closeCommand;
        //protected ICommand saveCommand;
        //private MainWindowViewModel parentViewModel;
        private List<InputBinding> inputBindings;
        public event PropertyChangedEventHandler PropertyChanged;

        //public abstract ActiveModuleEnum ActiveModuleEnum { get; protected set; }

        //public ICommand NewCommand
        //{
        //    get
        //    {
        //        if (newCommand == null)
        //        {
        //            newCommand = new Command(
        //                p => true,
        //                p => this.New());
        //        }
        //        return newCommand;
        //    }
        //}

        //public ICommand CloseCommand
        //{
        //    get
        //    {
        //        if (closeCommand == null)
        //        {
        //            closeCommand = new Command(
        //                p => true,
        //                p => this.Close());
        //        }
        //        return closeCommand;
        //    }
        //}

        public virtual void New() { }

        public virtual void Delete() { }

        public virtual void Details() { }

        public virtual void Status() { }
        //public virtual void Close() { }
        public virtual void Save() { }

        public List<InputBinding> InputBindings
        {
            get { return inputBindings; }
            set { inputBindings = value; }
        }

        //public BaseViewModel()
        //{
        //    inputBindings = new List<InputBinding>();
        //    InputBindings.Add(new KeyBinding() { Command = CloseCommand, Key = Key.Escape });
        //    InputBindings.Add(new KeyBinding() { Command = NewCommand, Key = Key.N, Modifiers = ModifierKeys.Control });
        //}

        public void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
