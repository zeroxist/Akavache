using System;
using MonoMac.Foundation;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace Akavache.Mac
{
    public class ServiceLocationRegistration : IWantsToRegisterStuff
    {
        public void Register()
        {
            RxApp.Register(typeof(MacFilesystemProvider), typeof(IFilesystemProvider));
            BlobCache.ApplicationName = NSBundle.MainBundle.BundleIdentifier;
        }
    }

    public class MacFilesystemProvider : IFilesystemProvider
    {
        readonly SimpleFilesystemProvider _inner = new SimpleFilesystemProvider();

        public IObservable<Stream> SafeOpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share, IScheduler scheduler)
        {
            return _inner.SafeOpenFileAsync(path, mode, access, share, scheduler);
        }

        public IObservable<Unit> CreateRecursive(string path)
        {
            return _inner.CreateRecursive(path);
        }

        public IObservable<Unit> Delete(string path)
        {
            return _inner.Delete(path);
        }

        public string GetDefaultLocalMachineCacheDirectory()
        {
            return CreateAppDirectory(NSSearchPathDirectory.CachesDirectory);
        }

        public string GetDefaultRoamingCacheDirectory()
        {
            return CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory);
        }

        public string GetDefaultSecretCacheDirectory()
        {
            return CreateAppDirectory(NSSearchPathDirectory.ApplicationSupportDirectory, "SecretCache");
        }

        string CreateAppDirectory(NSSearchPathDirectory targetDir, string subDir = "BlobCache")
        {
            NSError err;

            var fm = new NSFileManager();
            var url = fm.GetUrl(targetDir, NSSearchPathDomain.All, null, true, out err);
            var ret = Path.Combine(url.RelativePath, BlobCache.ApplicationName, subDir);
            if (!Directory.Exists(ret)) _inner.CreateRecursive(ret).Wait();

            return ret;
        }
    }
}

