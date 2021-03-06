﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FamilyEditorInterface.WPF
{
    public class FamilyParameterViewModel : INotifyPropertyChanged
    {
        private Document doc;
        private string documentName;
        internal bool _isClosed;
        internal bool _enabled;
        public FamilyParameterView view;
        private SortedList<string, FamilyParameter> famParam;
        public EventHandler PresenterClosed;

        public ICommand ShuffleCommand { get; set; }
        public ICommand PrecisionCommand { get; set; }
        public ICommand DeleteUnusedCommand { get; set; }
        public ICommand VisibilityCommand { get; set; }

        private ObservableCollection<FamilyParameterModel> _valueParameters;
        private ObservableCollection<FamilyParameterModel> _builtInParameters;
        private ObservableCollection<FamilyParameterModel> _checkParameters;

        public ObservableCollection<FamilyParameterModel> ValueParameters
        {
            get { return _valueParameters; }
            set
            {
                _valueParameters = value;
                RaisePropertyChanged("ValueParameters");
            }
        }
        public ObservableCollection<FamilyParameterModel> BuiltInParameters
        {
            get { return _builtInParameters; }
            set
            {
                _builtInParameters = value;
                RaisePropertyChanged("BuiltInParameters");
            }
        }
        public ObservableCollection<FamilyParameterModel> CheckParameters
        {
            get { return _checkParameters; }
            set
            {
                _checkParameters = value;
                RaisePropertyChanged("CheckParameters");
            }
        }        
        public Document _Document
        {
            get
            {
                return doc;
            }
            set
            {
                if (doc != value)
                {
                    doc = value;
                    _DocumentName = doc.Title.Replace(".rfa","");
                }
            }
        }
        public string _DocumentName
        {
            get
            {
                return documentName;
            }
            set
            {
                if (documentName != value)
                {
                    documentName = value;
                    RaisePropertyChanged("_DocumentName");
                }
            }
        }

        public FamilyParameterViewModel(Document document)
        {
            Application.handler.EncounteredError += RollBackState;

            this._Document = document;

            ShuffleCommand = new RelayCommand(o => Shuffle("ShuffleButton"));
            PrecisionCommand = new RelayCommand(o => Precision("PrecisionButton"));
            DeleteUnusedCommand = new RelayCommand(o => DeleteUnused("DeleteUnusedButton"));
            VisibilityCommand = new RelayCommand(o => ChangeVisibility("ToggleVisibility"));

            this.PopulateModel();
            this._enabled = true;
        }

        // Force update in case of error
        private void RollBackState(object sender, EventArgs e)
        {
            PopulateModel();
        }

        private void PopulateModel()
        {
            ValueParameters = new ObservableCollection<FamilyParameterModel>();
            ValueParameters.CollectionChanged += ValueParameters_CollectionChanged;
            BuiltInParameters = new ObservableCollection<FamilyParameterModel>();
            CheckParameters = new ObservableCollection<FamilyParameterModel>();
            famParam = new SortedList<string, FamilyParameter>();
            
            FamilyManager familyManager = doc.FamilyManager;
            FamilyType familyType = familyManager.CurrentType;


            if (familyType == null)
            {
                familyType = CreateDefaultFamilyType(familyManager);
            }
            
            Utils.Init(this.doc);
            double value = 0.0;

            foreach (FamilyParameter fp in familyManager.Parameters)
            {
                if (!famEdit(fp, familyType)) continue;
                else
                {
                    if (!famParam.ContainsKey(fp.Definition.Name))
                        famParam.Add(fp.Definition.Name, fp);
                }
            }

            List<ElementId> eId = new List<ElementId>();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<Dimension> dimList = collector
                .OfCategory(BuiltInCategory.OST_Dimensions)
                .WhereElementIsNotElementType()
                .Cast<Dimension>()
                .ToList();

            List<FamilyParameter> paramUsed = new List<FamilyParameter>();

            foreach (Dimension dim in dimList)
            {
                try
                {
                    if (dim.FamilyLabel != null) paramUsed.Add(dim.FamilyLabel);
                }
                catch (Exception)
                {

                }
            }

            foreach (FamilyParameter fp in famParam.Values)
            {
                bool associated = !fp.AssociatedParameters.IsEmpty || paramUsed.Any(x => x.Definition.Name.Equals(fp.Definition.Name));
                bool builtIn = fp.Id.IntegerValue < 0;
                ///yes-no parameters
                if (fp.Definition.ParameterType.Equals(ParameterType.YesNo))
                {
                    if (fp.StorageType == StorageType.Integer) value = Convert.ToDouble(familyType.AsInteger(fp));

                    //eId.Add(fp.Id);

                    FamilyParameterModel newItem = new FamilyParameterModel(); // collect data yes-no
                    newItem.Precision = Properties.Settings.Default.Precision;
                    newItem.Name = fp.Definition.Name;
                    newItem.Value = value;
                    newItem.Type = fp.Definition.ParameterType.ToString();
                    newItem.Associated = associated;
                    newItem.BuiltIn = fp.Id.IntegerValue < 0;
                    newItem.Shared = fp.IsShared;
                    newItem.Visible = associated ? true : Properties.Settings.Default.AssociatedVisibility;
                    newItem.TypeOrInstance = fp.IsInstance ? "Instance" : "Type";

                    CheckParameters.Add(newItem);

                    continue;
                }
                ///slider parameters
                if (fp.StorageType == StorageType.Double) value = Convert.ToDouble(familyType.AsDouble(fp));
                else if (fp.StorageType == StorageType.Integer) value = Convert.ToDouble(familyType.AsInteger(fp));
                eId.Add(fp.Id);

                if (!builtIn)
                {
                    FamilyParameterModel newItem = new FamilyParameterModel();  // collect data slider, value != 0
                    newItem.Precision = Properties.Settings.Default.Precision;
                    newItem.Name = fp.Definition.Name;
                    newItem.Value = value;
                    newItem.Type = fp.Definition.ParameterType.ToString();
                    newItem.Associated = associated;
                    newItem.BuiltIn = fp.Id.IntegerValue < 0;
                    newItem.Shared = fp.IsShared;
                    newItem.Visible = associated ? true : Properties.Settings.Default.AssociatedVisibility;
                    newItem.TypeOrInstance = fp.IsInstance ? "Instance" : "Type";

                    ValueParameters.Add(newItem);
                }
                else
                {
                    FamilyParameterModel newItem = new FamilyParameterModel(); // collect data slider, value == 0
                    newItem.Precision = Properties.Settings.Default.Precision;
                    newItem.Name = fp.Definition.Name;
                    newItem.Value = value;
                    newItem.Type = fp.Definition.ParameterType.ToString();
                    newItem.Associated = associated;
                    newItem.BuiltIn = fp.Id.IntegerValue < 0;
                    newItem.Shared = fp.IsShared;
                    newItem.Visible = associated ? true : Properties.Settings.Default.AssociatedVisibility;
                    newItem.TypeOrInstance = fp.IsInstance ? "Instance" : "Type";

                    BuiltInParameters.Add(newItem);
                }
            }            
        }
        private void ValueParameters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (FamilyParameterModel item in e.NewItems)
                    item.PropertyChanged += MyType_PropertyChanged;

            if (e.OldItems != null)
                foreach (FamilyParameterModel item in e.OldItems)
                    item.PropertyChanged -= MyType_PropertyChanged;
        }
        void MyType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Name")
            {
                string _name = (sender as FamilyParameterModel).Name;
                string _oldName = (sender as FamilyParameterModel).OldName;
                Utils.MakeRequest(RequestId.ChangeParamName, new Tuple<string, string>(_oldName, _name));
            }
        }

        /// <summary>
        /// Check parameter conditions
        /// </summary>
        /// <param name="fp"></param>
        /// <param name="ft"></param>
        /// <returns></returns>
        private Boolean famEdit(FamilyParameter fp, FamilyType ft)
        {
            if (!fp.StorageType.ToString().Equals("Double") && !fp.StorageType.ToString().Equals("Integer"))
            {
                return false;
            }
            else if (fp.IsDeterminedByFormula || fp.Formula != null)
            {
                return false;
            }
            else if (fp.IsReporting)
            {
                return false;
            }
            else if (fp.IsDeterminedByFormula)
            {
                return false;
            }
            return true;
        }


        #region ViewModel Maintenance
        internal void Close()
        {
            view.Close();
            _isClosed = true;
        }
        /// <summary>
        /// Show the Window, Start the Form
        /// </summary>
        /// <param name="hWndRevit"></param>
        internal void Show(WindowHandle hWndRevit)
        {
            view = new FamilyParameterView(this);
            System.Windows.Interop.WindowInteropHelper x = new System.Windows.Interop.WindowInteropHelper(view);
            x.Owner = hWndRevit.Handle;
            view.Closed += FormClosed;
            try
            {
                view.Show();
            }
            catch(Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
        }
        /// <summary>
        /// Shuffle parameter values
        /// </summary>
        private void Shuffle(object sender)
        {
            SingleRandom random = SingleRandom.Instance;
            List<Tuple<string, double>> requestValues = new List<Tuple<string, double>>();

            foreach (var item in _valueParameters)
            {
                if (item.Value != 0)
                {
                    double v = item.Value;
                    double plus = (v + 0.25 * v);    // plus minus values - around the current value +-25%
                    double minus = (v - 0.25 * v);
                    double randValue = random.NextDouble() * (plus - minus) + minus;
                    item.SuppressUpdate();
                    item.Value = randValue;
                    requestValues.Add(new Tuple<string, double>(item.Name, randValue));
                }
            }

            foreach (var item in _builtInParameters)
            {
                if (item.Value != 0)
                {
                    double v = item.Value;
                    double plus = (v + 0.25 * v);    // plus minus values - around the current value +-25%
                    double minus = (v - 0.25 * v);
                    double randValue = random.NextDouble() * (plus - minus) + minus;
                    item.SuppressUpdate();
                    item.Value = randValue;
                    requestValues.Add(new Tuple<string, double>(item.Name, randValue));
                }
            }

            if (requestValues.Count > 0) MakeRequest(RequestId.SlideParam, requestValues);
        }
        /// <summary>
        /// Change Precision
        /// </summary>
        private void Precision(object sender)
        {
            Settings settings = new Settings();
            var holder = Properties.Settings.Default.Precision;
            if (settings.ShowDialog() == true)
            {
                if (Properties.Settings.Default.Precision != holder)
                    PopulateModel();
            }
        }
        /// <summary>
        /// Delete Unused Parameters
        /// </summary>
        /// <param name="sender"></param>
        private void DeleteUnused(object sender)
        {
            List<string> values = new List<string>();

            foreach(var item in ValueParameters)
            {
                if (!item.Associated) values.Add(item.Name);
            }

            foreach (var item in CheckParameters)
            {
                if (!item.Associated) values.Add(item.Name);
            }

            if (values.Count > 0)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("{0} Parameters will be removed.", values.Count.ToString()));
                MakeRequest(RequestId.DeleteId, values);
            }
        }
        /// <summary>
        /// Toggle Visibility of Parameters which are not associated
        /// </summary>
        /// <param name="sender"></param>
        private void ChangeVisibility(object sender)
        {
            Properties.Settings.Default.AssociatedVisibility = !Properties.Settings.Default.AssociatedVisibility;

            foreach (var item in ValueParameters)
                if (!item.Associated)
                    item.Visible = Properties.Settings.Default.AssociatedVisibility;

            foreach (var item in BuiltInParameters)
                if (!item.Associated)
                    item.Visible = Properties.Settings.Default.AssociatedVisibility;

            foreach (var item in CheckParameters)
                if (!item.Associated)
                    item.Visible = Properties.Settings.Default.AssociatedVisibility;
        }
        private void MakeRequest(RequestId request, List<string> values)
        {
            //MessageBox.Show("You are in the Control.Request event.");
            Application.handler.Request.DeleteValue(values);
            Application.handler.Request.Make(request);
            Application.exEvent.Raise();
        }
        private void MakeRequest(RequestId request, List<Tuple<string, double>> values)
        {
            //MessageBox.Show("You are in the Control.Request event.");
            Application.handler.Request.Value(values);
            Application.handler.Request.Make(request);
            Application.exEvent.Raise();
        }

        internal void Enable()
        {
            if(!_enabled)
            {
                _enabled = true;
                view.IsEnabled = true;
                view.Visibility = System.Windows.Visibility.Visible;
            }
        }

        internal void Disable()
        {
            if (_enabled)
            {
                _enabled = false;
                view.IsEnabled = false;
                view.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        internal void ThisDocumentChanged()
        {
            this.PopulateModel();
        }

        // Notify that the form is closed and effectivelly close shop
        private void FormClosed(object sender, EventArgs e)
        {
            this.Dispose();
        }

        internal void DocumentSwitched()
        {
            this.PopulateModel();
        }
        // Close down the presenter and raise the event
        internal void Dispose()
        {
            // This form is closed
            _isClosed = true;
            // Remove registered events
            view.Closed -= FormClosed;
            if (PresenterClosed != null)
                PresenterClosed(this, new EventArgs());
        }
        #endregion

        /// <summary>
        /// Creates default family type.
        /// </summary>
        /// <param name="familyManager">The family manager.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Aborted by user.</exception>
        private FamilyType CreateDefaultFamilyType(FamilyManager familyManager)
        {
            FamilyType familyType = null;

            TaskDialog td = new TaskDialog("No Family Type");
            td.MainInstruction = "Create Default Family Type.";
            string s = "This might be a new Family with no existing Parameters or Family Types." + Environment.NewLine + Environment.NewLine +
            "In order to use this plugin, you can either create a new Parameter/Family Type from the Family Types Dialog" +
                " and restart the plugin or create a Default Family Type by accepting this message." + Environment.NewLine + Environment.NewLine +
                    "You can always delete the Default Family Parameter later.";
            td.MainContent = s;
            td.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;

            TaskDialogResult tResult = td.Show();

            if (TaskDialogResult.Yes == tResult)
            {
                using (Transaction t = new Transaction(doc, "Create Family Type"))
                {
                    t.Start();
                    familyType = familyManager.NewType("Default");
                    t.Commit();
                }
            }
            else
            {
                throw new Exception("Aborted by user.");
            }

            return familyType;
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    /// <summary>
    /// The Command interface that will let us relay button events to view model methods
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter ?? "<N/A>");
        }

    }
}
