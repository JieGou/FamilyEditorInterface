﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace FamilyEditorInterface
{
    /// <summary>
    /// A class with methods to execute requests made by the dialog user.
    /// </summary>
    /// 
    public class RequestHandler : IExternalEventHandler
    {
        // delegate that will expect a family parameter to act upon
        private delegate void FamilyOperation(FamilyManager fm, FamilyParameter fp);
        // The value of the latest request made by the modeless form 
        private Request m_request = new Request();
        // Encountered Error, notify the View Model to roll back (update)
        public event EventHandler EncounteredError;
                
        /// <summary>
        /// A public property to access the current request value
        /// </summary>
        public Request Request
        {
            get { return m_request; }
        }

        /// <summary>
        ///   A method to identify this External Event Handler
        /// </summary>
        public String GetName()
        {
            return "Family Interface";
        }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            return;
                        }
                    case RequestId.SlideParam:
                        {
                            ExecuteParameterChange(uiapp, "Slide First Parameter", Request.GetValue());
                            break;
                        }
                    case RequestId.DeleteId:
                        {
                            ExecuteParameterChange(uiapp, "Delete Parameter", Request.GetDeleteValue());
                            break;
                        }
                    case RequestId.ChangeParamName:
                        {
                            ExecuteParameterChange(uiapp, "Change Parameter Name", Request.GetRenameValue());
                            break;
                        }
                    case RequestId.RestoreAll:
                        {
                            ExecuteParameterChange(uiapp, "Restore All", Request.GetAllValues());
                            break;
                        }
                    case RequestId.TypeToInstance:
                        {
                            ExecuteParameterChange(uiapp, "Type To Instance", Request.GetTypeToInstanceValue(), "type or instance");
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch(Exception)
            {
                if (EncounteredError != null)
                    EncounteredError(this, null);
            }
            finally
            {
                Application.thisApp.WakeFormUp();
            }

            return;
        }

        #region Execute
        /// <summary>
        /// Executes on restore all values
        /// </summary>
        /// <param name="uiapp"></param>
        /// <param name="text"></param>
        /// <param name="values"></param>
        private void ExecuteParameterChange(UIApplication uiapp, String text, List<Tuple<string, string, double>> values)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (!doc.IsFamilyDocument)
            {
                Command.global_message =
                  "Please run this command in a family document.";
                TaskDialog.Show("Message", Command.global_message);
            }

            if ((uidoc != null))
            {
                using (TransactionGroup tg = new TransactionGroup(doc, "Parameter Change"))
                {
                    tg.Start();
                    foreach (var value in values)
                    {
                        using (Transaction trans = new Transaction(uidoc.Document))
                        {
                            FailureHandlingOptions failureHandlingOptions = trans.GetFailureHandlingOptions();
                            FailureHandler failureHandler = new FailureHandler();
                            failureHandlingOptions.SetFailuresPreprocessor(failureHandler);
                            failureHandlingOptions.SetClearAfterRollback(true);
                            trans.SetFailureHandlingOptions(failureHandlingOptions);

                            FamilyManager mgr = doc.FamilyManager;
                            FamilyParameter fp = mgr.get_Parameter(value.Item1);
                            // Since we'll modify the document, we need a transaction
                            // It's best if a transaction is scoped by a 'using' block
                            // The name of the transaction was given as an argument
                            if (trans.Start(text) == TransactionStatus.Started)
                            {
                                mgr.Set(fp, value.Item3);
                                //operation(mgr, fp);
                                doc.Regenerate();
                                if (!value.Item1.Equals(value.Item2)) mgr.RenameParameter(fp, value.Item2);
                                trans.Commit();
                                uidoc.RefreshActiveView();
                            }
                            if (failureHandler.ErrorMessage != "")
                            {
                                if (EncounteredError != null)
                                    EncounteredError(this, null);
                            }
                        }
                    }
                    tg.Assimilate();
                }
            }
        }
        /// <summary>
        ///   The main door-modification subroutine - called from every request 
        /// </summary>
        /// <remarks>
        ///   It searches the current selection for all doors
        ///   and if it finds any it applies the requested operation to them
        /// </remarks>
        /// <param name="uiapp">The Revit application object</param>
        /// <param name="text">Caption of the transaction for the operation.</param>
        /// <param name="operation">A delegate to perform the operation on an instance of a door.</param>
        /// 
        private void ExecuteParameterChange(UIApplication uiapp, String text, List<Tuple<string,double>> values)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (!doc.IsFamilyDocument)
            {
                Command.global_message =
                  "Please run this command in a family document.";
                TaskDialog.Show("Message", Command.global_message);
            }

            if ((uidoc != null))
            {
                using (TransactionGroup tg = new TransactionGroup(doc, "Parameter Change"))
                {
                    tg.Start();
                    foreach (var value in values)
                    {
                        using (Transaction trans = new Transaction(uidoc.Document))
                        {
                            FailureHandlingOptions failureHandlingOptions = trans.GetFailureHandlingOptions();
                            FailureHandler failureHandler = new FailureHandler();
                            failureHandlingOptions.SetFailuresPreprocessor(failureHandler);
                            failureHandlingOptions.SetClearAfterRollback(true);
                            trans.SetFailureHandlingOptions(failureHandlingOptions);

                            FamilyManager mgr = doc.FamilyManager;
                            FamilyParameter fp = mgr.get_Parameter(value.Item1);
                            // Since we'll modify the document, we need a transaction
                            // It's best if a transaction is scoped by a 'using' block
                            // The name of the transaction was given as an argument
                            if (trans.Start(text) == TransactionStatus.Started)
                            {
                                mgr.Set(fp, value.Item2);
                                //operation(mgr, fp);
                                doc.Regenerate();
                                trans.Commit();
                                uidoc.RefreshActiveView();
                                if (failureHandler.ErrorMessage != "")
                                {
                                    if (EncounteredError != null)
                                        EncounteredError(this, null);
                                }
                            }
                        }
                    }
                    tg.Assimilate();
                }
            }
        }
        /// <summary>
        /// Executes on type to instance change
        /// </summary>
        /// <param name="uiapp"></param>
        /// <param name="text"></param>
        /// <param name="values"></param>
        private void ExecuteParameterChange(UIApplication uiapp, String text, List<string> values, string type)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (!doc.IsFamilyDocument)
            {
                Command.global_message =
                  "Please run this command in a family document.";
                TaskDialog.Show("Message", Command.global_message);
            }

            if ((uidoc != null))
            {
                using (TransactionGroup tg = new TransactionGroup(doc, "Parameter Type To Instance"))
                {
                    tg.Start();
                    using (Transaction trans = new Transaction(uidoc.Document))
                    {
                        FailureHandlingOptions failureHandlingOptions = trans.GetFailureHandlingOptions();
                        FailureHandler failureHandler = new FailureHandler();
                        failureHandlingOptions.SetFailuresPreprocessor(failureHandler);
                        failureHandlingOptions.SetClearAfterRollback(true);
                        trans.SetFailureHandlingOptions(failureHandlingOptions);
                        // Since we'll modify the document, we need a transaction
                        // It's best if a transaction is scoped by a 'using' block
                        // The name of the transaction was given as an argument
                        if (trans.Start(text) == TransactionStatus.Started)
                        {
                            FamilyManager mgr = doc.FamilyManager;
                            foreach (var value in values)
                            {
                                FamilyParameter fp = mgr.get_Parameter(value);
                                if (fp.IsInstance)
                                {
                                    mgr.MakeType(fp);
                                }
                                else {
                                    mgr.MakeInstance(fp);
                                };
                            }
                        }
                        doc.Regenerate();
                        trans.Commit();
                        uidoc.RefreshActiveView();
                        if (failureHandler.ErrorMessage != "")
                        {
                            if (EncounteredError != null)
                                EncounteredError(this, null);
                        }
                    }
                    tg.Assimilate();
                }
            }
        }
        /// <summary>
        /// Executes on parameter delete
        /// </summary>
        /// <param name="uiapp"></param>
        /// <param name="text"></param>
        /// <param name="values"></param>
        private void ExecuteParameterChange(UIApplication uiapp, String text, List<string> values)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (!doc.IsFamilyDocument)
            {
                Command.global_message =
                  "Please run this command in a family document.";
                TaskDialog.Show("Message", Command.global_message);
            }

            if ((uidoc != null))
            {
                using (TransactionGroup tg = new TransactionGroup(doc, "Parameter Delete"))
                {
                    tg.Start();
                    using (Transaction trans = new Transaction(uidoc.Document))
                    {
                        FailureHandlingOptions failureHandlingOptions = trans.GetFailureHandlingOptions();
                        FailureHandler failureHandler = new FailureHandler();
                        failureHandlingOptions.SetFailuresPreprocessor(failureHandler);
                        failureHandlingOptions.SetClearAfterRollback(true);
                        trans.SetFailureHandlingOptions(failureHandlingOptions);
                        // Since we'll modify the document, we need a transaction
                        // It's best if a transaction is scoped by a 'using' block
                        // The name of the transaction was given as an argument
                        if (trans.Start(text) == TransactionStatus.Started)
                        {
                            FamilyManager mgr = doc.FamilyManager;
                            foreach (var value in values)
                            {
                                FamilyParameter fp = mgr.get_Parameter(value);
                                mgr.RemoveParameter(fp);
                            }
                        }
                        doc.Regenerate();
                        trans.Commit();
                        uidoc.RefreshActiveView();
                        if (failureHandler.ErrorMessage != "")
                        {
                            if (EncounteredError != null)
                                EncounteredError(this, null);
                        }
                    }
                    tg.Assimilate();
                }
            }
        }
        /// <summary>
        /// Executes on parameter name change 
        /// </summary>
        /// <param name="uiapp"></param>
        /// <param name="text"></param>
        /// <param name="values"></param>
        private void ExecuteParameterChange(UIApplication uiapp, String text, List<Tuple<string, string>> values)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (!doc.IsFamilyDocument)
            {
                Command.global_message =
                  "Please run this command in a family document.";
                TaskDialog.Show("Message", Command.global_message);
            }

            if ((uidoc != null))
            {
                using (Transaction trans = new Transaction(uidoc.Document, "Parameter Name Changed"))
                {
                    FailureHandlingOptions failureHandlingOptions = trans.GetFailureHandlingOptions();
                    FailureHandler failureHandler = new FailureHandler();
                    failureHandlingOptions.SetFailuresPreprocessor(failureHandler);
                    failureHandlingOptions.SetClearAfterRollback(true);
                    trans.SetFailureHandlingOptions(failureHandlingOptions);

                    // Since we'll modify the document, we need a transaction
                    // It's best if a transaction is scoped by a 'using' block
                    // The name of the transaction was given as an argument
                    if (trans.Start(text) == TransactionStatus.Started)
                    {
                        FamilyManager mgr = doc.FamilyManager;
                        foreach (var value in values)
                        {
                            if (value.Item1.Equals(value.Item2)) continue;
                            FamilyParameter fp = mgr.get_Parameter(value.Item1);
                            mgr.RenameParameter(fp, value.Item2);
                        }
                    }
                    doc.Regenerate();
                    trans.Commit();
                    uidoc.RefreshActiveView();
                    if (failureHandler.ErrorMessage != "")
                    {
                        if (EncounteredError != null)
                            EncounteredError(this, null);
                    }
                }
            }
        }
    }
    #endregion

    #region Failure Handler
    public class FailureHandler : IFailuresPreprocessor
    {
        public string ErrorMessage { set; get; }
        public string ErrorSeverity { set; get; }

        public FailureHandler()
        {
            ErrorMessage = "";
            ErrorSeverity = "";
        }

        public FailureProcessingResult PreprocessFailures(
          FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> failureMessages
              = failuresAccessor.GetFailureMessages();

            foreach (FailureMessageAccessor
              failureMessageAccessor in failureMessages)
            {
                // We're just deleting all of the warning level 
                // failures and rolling back any others

                FailureDefinitionId id = failureMessageAccessor
                  .GetFailureDefinitionId();

                try
                {
                    ErrorMessage = failureMessageAccessor
                      .GetDescriptionText();
                }
                catch
                {
                    ErrorMessage = "Unknown Error";
                }

                try
                {
                    FailureSeverity failureSeverity
                      = failureMessageAccessor.GetSeverity();

                    ErrorSeverity = failureSeverity.ToString();

                    if (failureSeverity == FailureSeverity.Warning)
                    {
                        failuresAccessor.DeleteWarning(
                          failureMessageAccessor);
                    }
                    else
                    {
                        return FailureProcessingResult
                          .ProceedWithRollBack;
                    }
                }
                catch
                {
                }
            }
            return FailureProcessingResult.Continue;
        }
    }
    #endregion
}
