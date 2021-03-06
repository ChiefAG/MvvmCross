﻿// MvxPhoneViewPresenter.cs

// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
//
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

namespace MvvmCross.WindowsPhone.Views
{
    using System;
    using System.Threading;

    using Microsoft.Phone.Controls;

    using MvvmCross.Core.ViewModels;
    using MvvmCross.Core.Views;
    using MvvmCross.Platform;
    using MvvmCross.Platform.Exceptions;
    using MvvmCross.Platform.Platform;

    public class MvxPhoneViewPresenter
        : MvxViewPresenter, IMvxPhoneViewPresenter
    {
        private readonly PhoneApplicationFrame _rootFrame;

        protected PhoneApplicationFrame RootFrame => this._rootFrame;

        public MvxPhoneViewPresenter(PhoneApplicationFrame rootFrame)
        {
            this._rootFrame = rootFrame;
        }

        public override void Show(MvxViewModelRequest request)
        {
            try
            {
                var requestTranslator = Mvx.Resolve<IMvxPhoneViewModelRequestTranslator>();
                var xamlUri = requestTranslator.GetXamlUriFor(request);
                this._rootFrame.Navigate(xamlUri);
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception exception)
            {
                MvxTrace.Trace("Error seen during navigation request to {0} - error {1}",
                               request.ViewModelType.Name, exception.ToLongString());
            }
        }

        public override void ChangePresentation(MvxPresentationHint hint)
        {
            if (this.HandlePresentationChange(hint)) return;

            if (hint is MvxClosePresentationHint)
            {
                this.Close((hint as MvxClosePresentationHint).ViewModelToClose);
                return;
            }

            MvxTrace.Warning("Hint ignored {0}", hint.GetType().Name);
        }

        public virtual void Close(IMvxViewModel viewModel)
        {
            var currentView = this._rootFrame.Content as IMvxView;
            if (currentView == null)
            {
                Mvx.Warning("Ignoring close for viewmodel - rootframe has no current page");
                return;
            }

            if (currentView.ViewModel != viewModel)
            {
                Mvx.Warning("Ignoring close for viewmodel - rootframe's current page is not the view for the requested viewmodel");
                return;
            }

            if (!this._rootFrame.CanGoBack)
            {
                Mvx.Warning("Ignoring close for viewmodel - rootframe refuses to go back");
                return;
            }

            this._rootFrame.GoBack();
        }
    }
}