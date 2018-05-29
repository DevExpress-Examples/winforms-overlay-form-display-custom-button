using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DxSample {
    public class TaskHelper : IDisposable {
        CancellationTokenSource tokenSource;
        CancellationToken cancellationToken;

        public TaskHelper() {
            this.tokenSource = new CancellationTokenSource();
            this.cancellationToken = tokenSource.Token;
        }
        public Task<string> StartTask(IProgress<int> progress) {
            return StartTaskCore(progress);
        }
        public void Cancel() {
            tokenSource.Cancel();
        }

        Task<string> StartTaskCore(IProgress<int> progress) {
            return Task.Run(() => CalculateResult(progress), cancellationToken);
        }
        string CalculateResult(IProgress<int> progress) {
            for(int n = 0; n <= 100; n++) {
                // do something helpful
                Thread.Sleep(60);

                cancellationToken.ThrowIfCancellationRequested();
                if(progress != null) progress.Report(n);
            }
            return "CalculatedValue";
        }

        #region IDisposable

        public void Dispose() {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing) {
            if(disposing) {
                if(tokenSource != null) tokenSource.Dispose();
            }
            this.tokenSource = null;
        }

        #endregion
    }
}
